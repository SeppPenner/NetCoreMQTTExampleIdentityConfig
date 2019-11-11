using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MQTTnet.AspNetCore;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Newtonsoft.Json;
using Serilog;
using Storage;
using Storage.Database;
using Storage.Enumerations;
using Storage.Mappings;
using TopicCheck;

namespace NetCoreMQTTExampleIdentityConfig
{
    /// <summary>
    ///     The startup class.
    /// </summary>
    public class Startup
    {
        /// <summary>
        ///     The <see cref="PasswordHasher{TUser}" />.
        /// </summary>
        private static readonly PasswordHasher<User> Hasher = new PasswordHasher<User>();

        /// <summary>
        ///     The database context.
        /// </summary>
        private static MqttContext _databaseContext;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        ///     Gets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        ///     Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        // ReSharper disable once UnusedMember.Global
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.EnvironmentName == Environments.Development)
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            // Use authentication.
            app.UseAuthentication();

            // Use response compression.
            app.UseResponseCompression();

            // Use swagger stuff
            app.UseOpenApi();
            app.UseSwaggerUi3();

            // Use HTTPS.
            app.UseHttpsRedirection();
        }

        /// <summary>
        ///     Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Added the custom configuration options
            services.Configure<DatabaseConnectionSettings>(Configuration.GetSection("DatabaseConnectionSettings"));
            services.Configure<MqttSettings>(Configuration.GetSection("MqttSettings"));

            // Load database connection settings
            var databaseConnection =
                Configuration.GetSection("DatabaseConnectionSettings").Get<DatabaseConnectionSettings>()
                ?? new DatabaseConnectionSettings();

            // Load MQTT configuration settings
            var mqttSettings = Configuration.GetSection("MqttSettings").Get<MqttSettings>();

            // Configure database context
            _databaseContext = new MqttContext(databaseConnection);

            // Added the identity stuff and the database connection
            services.AddDbContext<MqttContext>(options => options.UseNpgsql(databaseConnection.ToConnectionString()));

            services.AddIdentity<User, Role>().AddEntityFrameworkStores<MqttContext>().AddDefaultTokenProviders();

            // Add response compression
            services.AddResponseCompression();

            // Add AutoMapper
            services.AddAutoMapper(typeof(UserClaimsProfile), typeof(UserProfile));

            // Add swagger
            // Add swagger document for the API
            services.AddOpenApiDocument(
                config =>
                {
                    var version = Assembly.GetExecutingAssembly().GetName().Version;
                    config.DocumentName = $"NetCoreMQTTExampleIdentityConfig {version}";
                    config.PostProcess = document =>
                    {
                        document.Info.Version = $"{version}";
                        document.Info.Title = "NetCoreMQTTExampleIdentityConfig";
                        document.Info.Description = "NetCoreMQTTExampleIdentityConfig";
                    };
                });

            // Read certificate
            var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var certificate = new X509Certificate2(
                Path.Combine(currentPath, "certificate.pfx"),
                "test",
                X509KeyStorageFlags.Exportable);

            // Add MQTT stuff
            services.AddHostedMqttServer(
                builder => builder
#if DEBUG
                    .WithDefaultEndpoint().WithDefaultEndpointPort(1883)
#else
                    .WithoutDefaultEndpoint()
#endif
                    .WithEncryptedEndpoint().WithEncryptedEndpointPort(mqttSettings.Port)
                    .WithEncryptionCertificate(certificate.Export(X509ContentType.Pfx))
                    .WithEncryptionSslProtocol(SslProtocols.Tls12).WithConnectionValidator(
                        c =>
                        {
                            var currentUser = _databaseContext.Users.FirstOrDefault(u => u.UserName == c.Username);

                            if (currentUser == null)
                            {
                                c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                                LogMessage(c, true);
                                return;
                            }

                            if (c.Username != currentUser.UserName)
                            {
                                c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                                LogMessage(c, true);
                                return;
                            }

                            var hashingResult = Hasher.VerifyHashedPassword(
                                currentUser,
                                currentUser.PasswordHash,
                                c.Password);

                            if (hashingResult == PasswordVerificationResult.Failed)
                            {
                                c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                                LogMessage(c, true);
                                return;
                            }

                            if (!currentUser.ValidateClientId)
                            {
                                c.ReasonCode = MqttConnectReasonCode.Success;
                                c.SessionItems.Add(c.ClientId, currentUser);
                                return;
                            }

                            if (string.IsNullOrWhiteSpace(currentUser.ClientIdPrefix))
                            {
                                if (c.ClientId != currentUser.ClientId)
                                {
                                    c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                                    LogMessage(c, true);
                                    return;
                                }

                                c.SessionItems.Add(currentUser.ClientId, currentUser);
                            }
                            else
                            {
                                c.SessionItems.Add(currentUser.ClientIdPrefix, currentUser);
                            }

                            c.ReasonCode = MqttConnectReasonCode.Success;
                            LogMessage(c, false);
                        }).WithSubscriptionInterceptor(
                        c =>
                        {
                            var clientIdPrefix = GetClientIdPrefix(c.ClientId);
                            User currentUser;
                            bool userFound;

                            if (string.IsNullOrWhiteSpace(clientIdPrefix))
                            {
                                userFound = c.SessionItems.TryGetValue(c.ClientId, out var currentUserObject);
                                currentUser = currentUserObject as User;
                            }
                            else
                            {
                                userFound = c.SessionItems.TryGetValue(clientIdPrefix, out var currentUserObject);
                                currentUser = currentUserObject as User;
                            }

                            if (!userFound || currentUser == null)
                            {
                                c.AcceptSubscription = false;
                                LogMessage(c, false);
                                return;
                            }

                            var topic = c.TopicFilter.Topic;

                            // Get blacklist
                            var subscriptionBlackList = _databaseContext.UserClaims.FirstOrDefault(
                                uc => uc.UserId == currentUser.Id
                                      && uc.ClaimType == ClaimType.SubscriptionBlacklist.ToString());

                            var blacklist =
                                JsonConvert.DeserializeObject<List<string>>(subscriptionBlackList?.ClaimValue)
                                ?? new List<string>();

                            // Get whitelist
                            var subscriptionWhitelist = _databaseContext.UserClaims.FirstOrDefault(
                                uc => uc.UserId == currentUser.Id
                                      && uc.ClaimType == ClaimType.SubscriptionWhitelist.ToString());

                            var whitelist =
                                JsonConvert.DeserializeObject<List<string>>(subscriptionWhitelist?.ClaimValue)
                                ?? new List<string>();

                            // Check matches
                            if (blacklist.Contains(topic))
                            {
                                c.AcceptSubscription = false;
                                LogMessage(c, false);
                                return;
                            }

                            if (whitelist.Contains(topic))
                            {
                                c.AcceptSubscription = true;
                                LogMessage(c, true);
                                return;
                            }

                            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
                            foreach (var forbiddenTopic in blacklist)
                            {
                                var doesTopicMatch = TopicChecker.Regex(forbiddenTopic, topic);
                                if (!doesTopicMatch) continue;

                                c.AcceptSubscription = false;
                                LogMessage(c, false);
                                return;
                            }

                            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
                            foreach (var allowedTopic in whitelist)
                            {
                                var doesTopicMatch = TopicChecker.Regex(allowedTopic, topic);
                                if (!doesTopicMatch) continue;

                                c.AcceptSubscription = true;
                                LogMessage(c, true);
                                return;
                            }

                            c.AcceptSubscription = false;
                            LogMessage(c, false);
                        }).WithApplicationMessageInterceptor(
                        c =>
                        {
                            var clientIdPrefix = GetClientIdPrefix(c.ClientId);
                            User currentUser;
                            bool userFound;

                            if (string.IsNullOrWhiteSpace(clientIdPrefix))
                            {
                                userFound = c.SessionItems.TryGetValue(c.ClientId, out var currentUserObject);
                                currentUser = currentUserObject as User;
                            }
                            else
                            {
                                userFound = c.SessionItems.TryGetValue(clientIdPrefix, out var currentUserObject);
                                currentUser = currentUserObject as User;
                            }

                            if (!userFound || currentUser == null)
                            {
                                c.AcceptPublish = false;
                                return;
                            }

                            var topic = c.ApplicationMessage.Topic;

                            // Get blacklist
                            var subscriptionBlackList = _databaseContext.UserClaims.FirstOrDefault(
                                uc => uc.UserId == currentUser.Id
                                      && uc.ClaimType == ClaimType.PublishBlacklist.ToString());

                            var blacklist =
                                JsonConvert.DeserializeObject<List<string>>(subscriptionBlackList?.ClaimValue)
                                ?? new List<string>();

                            // Get whitelist
                            var subscriptionWhitelist = _databaseContext.UserClaims.FirstOrDefault(
                                uc => uc.UserId == currentUser.Id
                                      && uc.ClaimType == ClaimType.PublishWhitelist.ToString());

                            var whitelist =
                                JsonConvert.DeserializeObject<List<string>>(subscriptionWhitelist?.ClaimValue)
                                ?? new List<string>();

                            // Check matches
                            if (blacklist.Contains(topic))
                            {
                                c.AcceptPublish = false;
                                return;
                            }

                            if (whitelist.Contains(topic))
                            {
                                c.AcceptPublish = true;
                                LogMessage(c);
                                return;
                            }

                            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
                            foreach (var forbiddenTopic in blacklist)
                            {
                                var doesTopicMatch = TopicChecker.Regex(forbiddenTopic, topic);
                                if (!doesTopicMatch) continue;

                                c.AcceptPublish = false;
                                return;
                            }

                            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
                            foreach (var allowedTopic in whitelist)
                            {
                                var doesTopicMatch = TopicChecker.Regex(allowedTopic, topic);
                                if (!doesTopicMatch) continue;

                                c.AcceptPublish = true;
                                LogMessage(c);
                                return;
                            }

                            c.AcceptPublish = false;
                        }));

            services.AddMqttConnectionHandler();

            // Add the MVC stuff
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        /// <summary>
        ///     Gets the client id prefix for a client id if there is one or <c>null</c> else.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <returns>The client id prefix for a client id if there is one or <c>null</c> else.</returns>
        private static string GetClientIdPrefix(string clientId)
        {
            var clientIdPrefixes =
                _databaseContext.Users.Where(u => u.ClientIdPrefix != null).Select(u => u.ClientIdPrefix);

            foreach (var clientIdPrefix in clientIdPrefixes)
                if (clientId.StartsWith(clientIdPrefix))
                    return clientIdPrefix;

            return null;
        }

        /// <summary> 
        ///     Logs the message from the MQTT subscription interceptor context. 
        /// </summary> 
        /// <param name="context">The MQTT subscription interceptor context.</param> 
        /// <param name="successful">A <see cref="bool"/> value indicating whether the subscription was successful or not.</param> 
        private static void LogMessage(MqttSubscriptionInterceptorContext context, bool successful)
        {
            Log.Information(successful ? $"New subscription: ClientId = {context.ClientId}, TopicFilter = {context.TopicFilter}" : $"Subscription failed for clientId = {context.ClientId}, TopicFilter = {context.TopicFilter}");
        }

        /// <summary>
        ///     Logs the message from the MQTT message interceptor context.
        /// </summary>
        /// <param name="context">The MQTT message interceptor context.</param>
        private static void LogMessage(MqttApplicationMessageInterceptorContext context)
        {
            Log.Information(
                $"Message: ClientId = {context.ClientId}, Topic = {context.ApplicationMessage.Topic},"
                + $" Payload = {Encoding.UTF8.GetString(context.ApplicationMessage.Payload)}, QoS = {context.ApplicationMessage.QualityOfServiceLevel},"
                + $" Retain-Flag = {context.ApplicationMessage.Retain}");
        }

        /// <summary> 
        ///     Logs the message from the MQTT connection validation context. 
        /// </summary> 
        /// <param name="context">The MQTT connection validation context.</param> 
        /// <param name="showPassword">A <see cref="bool"/> value indicating whether the password is written to the log or not.</param> 
        private static void LogMessage(MqttConnectionValidatorContext context, bool showPassword)
        {
            if (showPassword)
            {
                Log.Information(
                    $"New connection: ClientId = {context.ClientId}, Endpoint = {context.Endpoint},"
                    + $" Username = {context.Username}, Password = {context.Password},"
                    + $" CleanSession = {context.CleanSession}");
            }
            else
            {
                Log.Information(
                    $"New connection: ClientId = {context.ClientId}, Endpoint = {context.Endpoint},"
                    + $" Username = {context.Username}, CleanSession = {context.CleanSession}");
            }
        }
    }
}