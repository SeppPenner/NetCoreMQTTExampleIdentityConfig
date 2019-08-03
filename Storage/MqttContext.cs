
namespace Storage
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Storage.Database;

    /// <inheritdoc cref="IdentityDbContext"/>
    /// <summary>
    /// Base class for the database context.
    /// </summary>
    public class MqttContext : IdentityDbContext<User, Role, long, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        /// <summary>
        /// The connection settings.
        /// </summary>
        private readonly DatabaseConnectionSettings connectionSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="MqttContext"/> class.
        /// </summary>
        /// <param name="connectionSettingsAccessor">The connection settings accessor.</param>
        public MqttContext(IOptions<DatabaseConnectionSettings> connectionSettingsAccessor)
        {
            this.connectionSettings = connectionSettingsAccessor.Value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MqttContext"/> class.
        /// </summary>
        /// <param name="connectionSettings">The connection settings</param>
        public MqttContext(DatabaseConnectionSettings connectionSettings)
        {
            this.connectionSettings = connectionSettings;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MqttContext"/> class.
        /// </summary>
        public MqttContext()
        {
            this.connectionSettings = new DatabaseConnectionSettings();
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        public string ConnectionString => this.connectionSettings.ToConnectionString();

        /// <inheritdoc cref="MqttContext"/>
        /// <summary>
        ///     Configures the database.
        /// </summary>
        /// <param name="optionsBuilder">
        ///     A builder used to create or modify options for this context. Databases (and other extensions)
        ///     typically define extension methods on this object that allow you to configure the context.
        /// </param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseNpgsql($"Host={this.connectionSettings.Host};Database={this.connectionSettings.Database};Username={this.connectionSettings.Username};Password={this.connectionSettings.Password};Port={this.connectionSettings.Port}");
        }
    }
}