﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Hämmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   The main program class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NetCoreMQTTExampleIdentityConfig
{
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using MQTTnet.AspNetCore.Extensions;

    using Serilog;

    /// <summary>
    ///     The main program class.
    /// </summary>
    public class Program
    {
        /// <summary>
        ///     Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>A <see cref="Task"/> representing any asynchronous operation.</returns>
        public static Task Main(string[] args)
        {
            var currentLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug().WriteTo.File(
                    // ReSharper disable once AssignNullToNotNullAttribute
                    Path.Combine(currentLocation, @"log\NetCoreMQTTExampleIdentityConfig_.txt"),
                    rollingInterval: RollingInterval.Day).CreateLogger();

            return CreateWebHostBuilder(args).Build().RunAsync();
        }

        /// <summary>
        ///     Creates the web host builder.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The <see cref="IWebHostBuilder" />.</returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel(o =>
                {
                    o.ListenAnyIP(1883, l => l.UseMqtt());
                    o.ListenAnyIP(5000);
                })
                .UseStartup<Startup>();
        }
    }
}