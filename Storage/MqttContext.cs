// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MqttContext.cs" company="Haemmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   Defines the MqttContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Storage
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;

    using Storage.Database;

    /// <inheritdoc cref="IdentityDbContext" />
    /// <summary>
    ///     Base class for the database context.
    /// </summary>
    public class MqttContext : IdentityDbContext<User, Role, long, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        /// <summary>
        ///     The connection settings.
        /// </summary>
        private readonly DatabaseConnectionSettings connectionSettings;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MqttContext" /> class.
        /// </summary>
        /// <param name="connectionSettingsAccessor">The connection settings accessor.</param>
        // ReSharper disable once UnusedMember.Global
        public MqttContext(IOptions<DatabaseConnectionSettings> connectionSettingsAccessor)
        {
            this.connectionSettings = connectionSettingsAccessor.Value;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MqttContext" /> class.
        /// </summary>
        /// <param name="connectionSettings">The connection settings</param>
        public MqttContext(DatabaseConnectionSettings connectionSettings)
        {
            this.connectionSettings = connectionSettings;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MqttContext" /> class.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public MqttContext()
        {
            this.connectionSettings = new DatabaseConnectionSettings();
        }

        /// <summary>
        ///     Gets the connection string.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public string ConnectionString => this.connectionSettings.ToConnectionString();

        /// <summary>
        ///     Gets or sets the database versions.
        /// </summary>
        public DbSet<DbVersion> DbVersions { get; set; }

        /// <inheritdoc cref="IdentityDbContext" />
        /// <summary>
        ///     Configures the database.
        /// </summary>
        /// <param name="optionsBuilder">
        ///     A builder used to create or modify options for this context. Databases (and other extensions)
        ///     typically define extension methods on this object that allow you to configure the context.
        /// </param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseNpgsql(
                $"Host={this.connectionSettings.Host};Database={this.connectionSettings.Database};Username={this.connectionSettings.Username};Password={this.connectionSettings.Password};Port={this.connectionSettings.Port}");
        }

        /// <inheritdoc cref="IdentityDbContext" />
        /// <summary>
        ///     Configures the schema needed for the identity framework.
        /// </summary>
        /// <param name="builder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>(
                b =>
                {
                    // Primary key
                    b.HasKey(u => u.Id);

                    // Indexes for "normalized" username and email, to allow efficient lookups
                    b.HasIndex(u => u.NormalizedUserName).HasName("UserNameIndex").IsUnique();
                    b.HasIndex(u => u.NormalizedEmail).HasName("EmailIndex");

                    // Maps to the Users table
                    b.ToTable("Users");

                    // A concurrency token for use with the optimistic concurrency checking
                    b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

                    // Limit the size of columns to use efficient database types
                    b.Property(u => u.UserName).HasMaxLength(256);
                    b.Property(u => u.NormalizedUserName).HasMaxLength(256);
                    b.Property(u => u.Email).HasMaxLength(256);
                    b.Property(u => u.NormalizedEmail).HasMaxLength(256);

                    // The relationships between User and other entity types
                    // Note that these relationships are configured with no navigation properties

                    // Each User can have many UserClaims
                    b.HasMany<UserClaim>().WithOne().HasForeignKey(uc => uc.UserId).IsRequired();

                    // Each User can have many UserLogins
                    b.HasMany<UserLogin>().WithOne().HasForeignKey(ul => ul.UserId).IsRequired();

                    // Each User can have many UserTokens
                    b.HasMany<UserToken>().WithOne().HasForeignKey(ut => ut.UserId).IsRequired();

                    // Each User can have many entries in the UserRole join table
                    b.HasMany<UserRole>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
                });

            builder.Entity<UserClaim>(
                b =>
                {
                    // Primary key
                    b.HasKey(uc => uc.Id);

                    // Maps to the UserClaims table
                    b.ToTable("UserClaims");
                });

            builder.Entity<UserLogin>(
                b =>
                {
                    // Composite primary key consisting of the LoginProvider and the key to use
                    // with that provider
                    b.HasKey(l => new { l.LoginProvider, l.ProviderKey });

                    // Limit the size of the composite key columns due to common DB restrictions
                    b.Property(l => l.LoginProvider).HasMaxLength(128);
                    b.Property(l => l.ProviderKey).HasMaxLength(128);

                    // Maps to the UserLogins table
                    b.ToTable("UserLogins");
                });

            builder.Entity<UserToken>(
                b =>
                {
                    // Composite primary key consisting of the UserId, LoginProvider and Name
                    b.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

                    // Limit the size of the composite key columns due to common DB restrictions
                    b.Property(t => t.LoginProvider).HasMaxLength(256);
                    b.Property(t => t.Name).HasMaxLength(256);

                    // Maps to the UserTokens table
                    b.ToTable("UserTokens");
                });

            builder.Entity<Role>(
                b =>
                {
                    // Primary key
                    b.HasKey(r => r.Id);

                    // Index for "normalized" role name to allow efficient lookups
                    b.HasIndex(r => r.NormalizedName).HasName("RoleNameIndex").IsUnique();

                    // Maps to the Roles table
                    b.ToTable("Roles");

                    // A concurrency token for use with the optimistic concurrency checking
                    b.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();

                    // Limit the size of columns to use efficient database types
                    b.Property(u => u.Name).HasMaxLength(256);
                    b.Property(u => u.NormalizedName).HasMaxLength(256);

                    // The relationships between Role and other entity types
                    // Note that these relationships are configured with no navigation properties

                    // Each Role can have many entries in the UserRole join table
                    b.HasMany<UserRole>().WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();

                    // Each Role can have many associated RoleClaims
                    b.HasMany<RoleClaim>().WithOne().HasForeignKey(rc => rc.RoleId).IsRequired();
                });

            builder.Entity<RoleClaim>(
                b =>
                {
                    // Primary key
                    b.HasKey(rc => rc.Id);

                    // Maps to the RoleClaims table
                    b.ToTable("RoleClaims");
                });

            builder.Entity<UserRole>(
                b =>
                {
                    // Primary key
                    b.HasKey(r => new { r.UserId, r.RoleId });

                    // Maps to the UserRoles table
                    b.ToTable("UserRoles");
                });

            builder.Entity<DbVersion>(
                b =>
                {
                    // Primary key
                    b.HasKey(r => new { r.Id });

                    // Maps to the DbVersions table
                    b.ToTable("DbVersions");
                });
        }
    }
}