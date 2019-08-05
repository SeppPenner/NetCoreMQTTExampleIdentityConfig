
namespace NetCoreMQTTExampleIdentityConfig
{
    using AutoMapper;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Storage;
    using Storage.Database;
    using Storage.Mappings;

    /// <summary>
    /// The startup class.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Added the custom configuration options
            services.Configure<DatabaseConnectionSettings>(this.Configuration.GetSection("DatabaseConnectionSettings"));
            services.Configure<MqttSettings>(this.Configuration.GetSection("MqttSettings"));

            // Load database connection settings
            var databaseConnection =
                this.Configuration.GetSection("DatabaseConnectionSettings").Get<DatabaseConnectionSettings>() ?? new DatabaseConnectionSettings();

            // Added the identity stuff and the database connection
            services.AddDbContext<MqttContext>(
                options => options.UseNpgsql(databaseConnection.ToConnectionString()));

            services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<MqttContext>()
                .AddDefaultTokenProviders();

            // Add response compression
            services.AddResponseCompression();

            // Add AutoMapper
            services.AddAutoMapper(typeof(UserClaimsProfile), typeof(UserProfile));

            // Add the MVC stuff
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Use authentication.
            app.UseAuthentication();

            // Use response compression.
            app.UseResponseCompression();

            // Use HTTPS.
            app.UseHttpsRedirection();

            // Use MVC.
            app.UseMvc();
        }
    }
}
