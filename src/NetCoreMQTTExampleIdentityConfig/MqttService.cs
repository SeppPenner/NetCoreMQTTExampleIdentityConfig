// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MqttService.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The main service class of the <see cref="MqttService" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NetCoreMQTTExampleIdentityConfig;

/// <inheritdoc cref="BackgroundService"/>
/// <summary>
///     The main service class of the <see cref="MqttService" />.
/// </summary>
public class MqttService : BackgroundService
{
    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger logger;

    /// <summary>
    /// The service name.
    /// </summary>
    private readonly string serviceName;

    /// <summary>
    /// The certificate.
    /// </summary>
    private readonly X509Certificate2 certificate;

    /// <summary>
    ///     The scope factory used to get a database context per callback.
    /// </summary>
    private readonly IServiceScopeFactory scopeFactory;

    /// <summary>
    /// The MQTT server. Only available after <see cref="StartMqttServer"/> has run.
    /// </summary>
    private MqttServer? mqttServer;

    /// <summary>
    /// The bytes divider. (Used to convert from bytes to kilobytes and so on).
    /// </summary>
    private static double BytesDivider => 1048576.0;

    /// <summary>
    /// The <see cref="PasswordHasher{TUser}" />.
    /// </summary>
    private static readonly IPasswordHasher<User> Hasher = new PasswordHasher<User>();

    /// <summary>
    /// The data limit cache for throttling for monthly data.
    /// </summary>
    private static readonly MemoryCache DataLimitCacheMonth = MemoryCache.Default;

    /// <summary>
    /// The client identifiers of the currently connected clients. Used as a set, the value is never read.
    /// </summary>
    private static readonly ConcurrentDictionary<string, byte> clientIds = new();

    /// <summary>
    /// Gets or sets the MQTT service configuration.
    /// </summary>
    public MqttSettings MqttServiceConfiguration { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MqttService"/> class.
    /// </summary>
    /// <param name="mqttServiceConfiguration">The MQTT service configuration.</param>
    /// <param name="serviceName">The service name.</param>
    /// <param name="certificate">The certificate.</param>
    /// <param name="scopeFactory">The scope factory used to get a database context per callback.</param>
    public MqttService(MqttSettings mqttServiceConfiguration, string serviceName, X509Certificate2 certificate, IServiceScopeFactory scopeFactory)
    {
        this.MqttServiceConfiguration = mqttServiceConfiguration;
        this.serviceName = serviceName;
        this.certificate = certificate;
        this.scopeFactory = scopeFactory;

        // Create the logger.
        this.logger = LoggerConfig.GetLoggerConfiguration(nameof(MqttService))
            .WriteTo.Sink((ILogEventSink)Log.Logger)
            .CreateLogger();
    }

    /// <inheritdoc cref="BackgroundService"/>
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!this.MqttServiceConfiguration.IsValid())
        {
            throw new Exception("The configuration is invalid");
        }

        this.logger.Information("Starting service");
        await this.StartMqttServer();
        this.logger.Information("Service started");
        await base.StartAsync(cancellationToken);
    }

    /// <inheritdoc cref="BackgroundService"/>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (this.mqttServer is not null)
        {
            await this.mqttServer.StopAsync();
        }

        await base.StopAsync(cancellationToken);
    }

    /// <inheritdoc cref="BackgroundService"/>
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Log some memory information.
                this.LogMemoryInformation();
                await Task.Delay(this.MqttServiceConfiguration.DelayInMilliSeconds, cancellationToken);
            }
            catch (Exception ex)
            {
                this.logger.Error("An error occurred: {Exception}", ex);
            }
        }
    }

    /// <summary>
    /// Checks whether a user has used the maximum of its publishing limit for the month or not.
    /// </summary>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="sizeInBytes">The message size in bytes.</param>
    /// <param name="monthlyByteLimit">The monthly byte limit.</param>
    /// <returns>A value indicating whether the user will be throttled or not.</returns>
    private bool IsUserThrottled(string clientId, long sizeInBytes, long monthlyByteLimit)
    {
        var foundUserInCache = DataLimitCacheMonth.GetCacheItem(clientId);

        if (foundUserInCache == null)
        {
            DataLimitCacheMonth.Add(clientId, sizeInBytes, DateTimeOffset.Now.EndOfCurrentMonth());

            if (sizeInBytes < monthlyByteLimit)
            {
                return false;
            }

            this.logger.Information("The client with client id {@ClientId} is now locked until the end of this month because it already used its data limit.", clientId);
            return true;
        }

        try
        {
            var currentValue = Convert.ToInt64(foundUserInCache.Value);
            currentValue = checked(currentValue + sizeInBytes);
            DataLimitCacheMonth[clientId] = currentValue;

            if (currentValue >= monthlyByteLimit)
            {
                this.logger.Information("The client with client id {@ClientId} is now locked until the end of this month because it already used its data limit.", clientId);
                return true;
            }
        }
        catch (OverflowException)
        {
            this.logger.Information("OverflowException thrown.");
            this.logger.Information("The client with client id {@ClientId} is now locked until the end of this month because it already used its data limit.", clientId);
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Gets the client id prefix for a client id if there is one or <c>null</c> else.
    /// </summary>
    /// <param name="databaseContext">The database context of the current callback.</param>
    /// <param name="clientId">The client id.</param>
    /// <returns>The client id prefix for a client id if there is one or <c>null</c> else.</returns>
    private static string GetClientIdPrefix(MqttContext databaseContext, string clientId)
    {
        var clientIdPrefixes =
            databaseContext.Users.Where(u => u.ClientIdPrefix != null).Select(u => u.ClientIdPrefix);

        foreach (var clientIdPrefix in clientIdPrefixes)
        {
            if (clientId.StartsWith(clientIdPrefix))
            {
                return clientIdPrefix;
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// Validates the MQTT connection.
    /// </summary>
    /// <param name="args">The arguments.</param>
    private Task ValidateConnectionAsync(ValidatingConnectionEventArgs args)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(args.UserName))
            {
                args.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                this.LogMessage(args, false);
                return Task.CompletedTask;
            }

            if (clientIds.ContainsKey(args.ClientId))
            {
                args.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
                this.logger.Warning("A client with client id {ClientId} is already connected", args.ClientId);
                return Task.CompletedTask;
            }

            using var scope = this.scopeFactory.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<MqttContext>();

            var currentUser = databaseContext.Users.FirstOrDefault(u => u.UserName == args.UserName);

            if (currentUser is null)
            {
                args.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                this.LogMessage(args, false);
                return Task.CompletedTask;
            }

            if (args.UserName != currentUser.UserName)
            {
                args.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                this.LogMessage(args, false);
                return Task.CompletedTask;
            }

            if (string.IsNullOrWhiteSpace(currentUser.PasswordHash))
            {
                args.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                this.LogMessage(args, false);
                return Task.CompletedTask;
            }

            var hashingResult = Hasher.VerifyHashedPassword(
                currentUser,
                currentUser.PasswordHash,
                args.Password);

            if (hashingResult == PasswordVerificationResult.Failed)
            {
                args.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                this.LogMessage(args, false);
                return Task.CompletedTask;
            }

            if (!currentUser.ValidateClientId)
            {
                if (!clientIds.TryAdd(args.ClientId, 0))
                {
                    args.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
                    this.logger.Warning("A client with client id {ClientId} is already connected", args.ClientId);
                    return Task.CompletedTask;
                }

                args.ReasonCode = MqttConnectReasonCode.Success;
                args.SessionItems.Add(args.ClientId, currentUser);
                this.LogMessage(args, true);
                return Task.CompletedTask;
            }

            if (string.IsNullOrWhiteSpace(currentUser.ClientIdPrefix))
            {
                if (args.ClientId != currentUser.ClientId)
                {
                    args.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
                    this.LogMessage(args, false);
                    return Task.CompletedTask;
                }

                args.SessionItems.Add(currentUser.ClientId, currentUser);
            }
            else
            {
                args.SessionItems.Add(currentUser.ClientIdPrefix, currentUser);
            }

            if (!clientIds.TryAdd(args.ClientId, 0))
            {
                args.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
                this.logger.Warning("A client with client id {ClientId} is already connected", args.ClientId);
                return Task.CompletedTask;
            }

            args.ReasonCode = MqttConnectReasonCode.Success;
            this.LogMessage(args, true);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            this.logger.Error("An error occurred: {Exception}.", ex);
            return Task.FromException(ex);
        }
    }

    /// <summary>
    /// Validates the MQTT subscriptions.
    /// </summary>
    /// <param name="args">The arguments.</param>
    private Task InterceptSubscriptionAsync(InterceptingSubscriptionEventArgs args)
    {
        try
        {
            using var scope = this.scopeFactory.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<MqttContext>();

            var clientIdPrefix = GetClientIdPrefix(databaseContext, args.ClientId);
            User? currentUser = null;

            if (string.IsNullOrWhiteSpace(clientIdPrefix))
            {
                if (args.SessionItems.Contains(args.ClientId))
                {
                    currentUser = args.SessionItems[args.ClientId] as User;
                }
            }
            else
            {
                if (args.SessionItems.Contains(clientIdPrefix))
                {
                    currentUser = args.SessionItems[clientIdPrefix] as User;
                }
            }

            if (currentUser is null)
            {
                args.ProcessSubscription = false;
                this.LogMessage(args, false);
                return Task.CompletedTask;
            }

            var topic = args.TopicFilter.Topic;

            // Get blacklist
            var subscriptionBlackList = databaseContext.UserClaims.FirstOrDefault(
            uc => uc.UserId == currentUser.Id
                  && uc.ClaimType == ClaimType.SubscriptionBlacklist.ToString());

            var blacklist = subscriptionBlackList?.ClaimValue is null ? [] : JsonConvert.DeserializeObject<List<string>>(subscriptionBlackList.ClaimValue) ?? [];

            // Get whitelist
            var subscriptionWhitelist = databaseContext.UserClaims.FirstOrDefault(
            uc => uc.UserId == currentUser.Id
                  && uc.ClaimType == ClaimType.SubscriptionWhitelist.ToString());

            var whitelist = subscriptionWhitelist?.ClaimValue is null ? [] : JsonConvert.DeserializeObject<List<string>>(subscriptionWhitelist.ClaimValue) ?? [];

            // Check matches
            if (blacklist.Contains(topic))
            {
                args.ProcessSubscription = false;
                this.LogMessage(args, false);
                return Task.CompletedTask;
            }

            if (whitelist.Contains(topic))
            {
                args.ProcessSubscription = true;
                this.LogMessage(args, true);
                return Task.CompletedTask;
            }

            foreach (var forbiddenTopic in blacklist)
            {
                var doesTopicMatch = TopicChecker.Regex(forbiddenTopic, topic);
                if (!doesTopicMatch)
                {
                    continue;
                }

                args.ProcessSubscription = false;
                this.LogMessage(args, false);
                return Task.CompletedTask;
            }

            foreach (var allowedTopic in whitelist)
            {
                var doesTopicMatch = TopicChecker.Regex(allowedTopic, topic);
                if (!doesTopicMatch)
                {
                    continue;
                }

                args.ProcessSubscription = true;
                this.LogMessage(args, true);
                return Task.CompletedTask;
            }

            args.ProcessSubscription = false;
            this.LogMessage(args, false);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            this.logger.Error("An error occurred: {Exception}.", ex);
            return Task.FromException(ex);
        }
    }

    /// <summary>
    /// Validates the MQTT application messages.
    /// </summary>
    /// <param name="args">The arguments.</param>
    private Task InterceptApplicationMessagePublishAsync(InterceptingPublishEventArgs args)
    {
        try
        {
            using var scope = this.scopeFactory.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<MqttContext>();

            var clientIdPrefix = GetClientIdPrefix(databaseContext, args.ClientId);
            User? currentUser = null;

            if (string.IsNullOrWhiteSpace(clientIdPrefix))
            {
                if (args.SessionItems.Contains(args.ClientId))
                {
                    currentUser = args.SessionItems[args.ClientId] as User;
                }
            }
            else
            {
                if (args.SessionItems.Contains(clientIdPrefix))
                {
                    currentUser = args.SessionItems[clientIdPrefix] as User;
                }
            }

            if (currentUser is null)
            {
                args.ProcessPublish = false;
                return Task.CompletedTask;
            }

            var topic = args.ApplicationMessage.Topic;

            if (currentUser.ThrottleUser)
            {
                var payload = args.ApplicationMessage.Payload;

                if (currentUser.MonthlyByteLimit is not null)
                {
                    if (this.IsUserThrottled(args.ClientId, payload.Length, currentUser.MonthlyByteLimit.Value))
                    {
                        args.ProcessPublish = false;
                        return Task.CompletedTask;
                    }
                }
            }

            // Get blacklist
            var publishBlackList = databaseContext.UserClaims.FirstOrDefault(
            uc => uc.UserId == currentUser.Id
                  && uc.ClaimType == ClaimType.PublishBlacklist.ToString());

            var blacklist = publishBlackList?.ClaimValue is null ? [] : JsonConvert.DeserializeObject<List<string>>(publishBlackList.ClaimValue) ?? [];

            // Get whitelist
            var publishWhitelist = databaseContext.UserClaims.FirstOrDefault(
            uc => uc.UserId == currentUser.Id
                  && uc.ClaimType == ClaimType.PublishWhitelist.ToString());

            var whitelist = publishWhitelist?.ClaimValue is null ? [] : JsonConvert.DeserializeObject<List<string>>(publishWhitelist.ClaimValue) ?? [];

            // Check matches
            if (blacklist.Contains(topic))
            {
                args.ProcessPublish = false;
                return Task.CompletedTask;
            }

            if (whitelist.Contains(topic))
            {
                args.ProcessPublish = true;
                this.LogMessage(args);
                return Task.CompletedTask;
            }

            foreach (var forbiddenTopic in blacklist)
            {
                var doesTopicMatch = TopicChecker.Regex(forbiddenTopic, topic);
                if (!doesTopicMatch)
                {
                    continue;
                }

                args.ProcessPublish = false;
                return Task.CompletedTask;
            }

            foreach (var allowedTopic in whitelist)
            {
                var doesTopicMatch = TopicChecker.Regex(allowedTopic, topic);
                if (!doesTopicMatch)
                {
                    continue;
                }

                args.ProcessPublish = true;
                this.LogMessage(args);
                return Task.CompletedTask;
            }

            args.ProcessPublish = false;
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            this.logger.Error("An error occurred: {Exception}.", ex);
            return Task.FromException(ex);
        }
    }

    /// <summary>
    /// Handles the client connected event.
    /// </summary>
    /// <param name="args">The arguments.</param>
    private Task ClientDisconnectedAsync(ClientDisconnectedEventArgs args)
    {
        clientIds.TryRemove(args.ClientId, out _);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Starts the MQTT server.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
    private async Task StartMqttServer()
    {
        var optionsBuilder = new MqttServerOptionsBuilder()
#if DEBUG
            .WithDefaultEndpoint().WithDefaultEndpointPort(1883)
#else
            .WithoutDefaultEndpoint()
#endif
            .WithEncryptedEndpoint().WithEncryptedEndpointPort(this.MqttServiceConfiguration.Port)
            .WithEncryptionCertificate(this.certificate)
            .WithEncryptionSslProtocol(SslProtocols.Tls12);

        this.mqttServer = new MqttServerFactory().CreateMqttServer(optionsBuilder.Build());
        this.mqttServer.ValidatingConnectionAsync += this.ValidateConnectionAsync;
        this.mqttServer.InterceptingSubscriptionAsync += this.InterceptSubscriptionAsync;
        this.mqttServer.InterceptingPublishAsync += this.InterceptApplicationMessagePublishAsync;
        this.mqttServer.ClientDisconnectedAsync += this.ClientDisconnectedAsync;
        await this.mqttServer.StartAsync();
    }

    /// <summary> 
    ///     Logs the message from the MQTT subscription interceptor context. 
    /// </summary> 
    /// <param name="args">The arguments.</param> 
    /// <param name="successful">A <see cref="bool"/> value indicating whether the subscription was successful or not.</param> 
    private void LogMessage(InterceptingSubscriptionEventArgs args, bool successful)
    {
#pragma warning disable Serilog004 // Constant MessageTemplate verifier
        this.logger.Information(
            successful
                ? "New subscription: ClientId = {ClientId}, TopicFilter = {TopicFilter}"
                : "Subscription failed for clientId = {ClientId}, TopicFilter = {TopicFilter}",
            args.ClientId,
            args.TopicFilter);
#pragma warning restore Serilog004 // Constant MessageTemplate verifier
    }

    /// <summary>
    ///     Logs the message from the MQTT message interceptor context.
    /// </summary>
    /// <param name="args">The arguments.</param>
    private void LogMessage(InterceptingPublishEventArgs args)
    {
        var payload = Encoding.UTF8.GetString(args.ApplicationMessage.Payload);

        this.logger.Information(
            "Message: ClientId = {ClientId}, Topic = {Topic}, Payload = {Payload}, QoS = {Qos}, Retain-Flag = {RetainFlag}",
            args.ClientId,
            args.ApplicationMessage?.Topic,
            payload,
            args.ApplicationMessage?.QualityOfServiceLevel,
            args.ApplicationMessage?.Retain);
    }

    /// <summary>
    ///     Logs the message from the MQTT connection validation context. The password is never written to the log, a
    ///     rejected client usually sends the password of a human being.
    /// </summary>
    /// <param name="args">The arguments.</param>
    /// <param name="successful">A <see cref="bool"/> value indicating whether the connection was successful or not.</param>
    private void LogMessage(ValidatingConnectionEventArgs args, bool successful)
    {
#pragma warning disable Serilog004 // Constant MessageTemplate verifier
        this.logger.Information(
            successful
                ? "New connection: ClientId = {ClientId}, Endpoint = {@Endpoint}, Username = {UserName}, CleanSession = {CleanSession}"
                : "Connection failed for clientId = {ClientId}, Endpoint = {@Endpoint}, Username = {UserName}, CleanSession = {CleanSession}",
            args.ClientId,
            args.RemoteEndPoint,
            args.UserName,
            args.CleanSession);
#pragma warning restore Serilog004 // Constant MessageTemplate verifier
    }

    /// <summary>
    /// Logs the heartbeat message with some memory information.
    /// </summary>
    private void LogMemoryInformation()
    {
        var totalMemory = GC.GetTotalMemory(false);
        var memoryInfo = GC.GetGCMemoryInfo();
        var divider = BytesDivider;
        Log.Information(
            "Heartbeat for service {ServiceName}: Total {Total}, heap size: {HeapSize}, memory load: {MemoryLoad}.",
            this.serviceName, $"{(totalMemory / divider):N3}", $"{(memoryInfo.HeapSizeBytes / divider):N3}", $"{(memoryInfo.MemoryLoadBytes / divider):N3}");
    }
}
