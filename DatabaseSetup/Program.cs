using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Storage;
using Storage.Database;

namespace DatabaseSetup
{
    /// <summary>
    ///     A program to setup the database.
    /// </summary>
    public static class Program
    {
        /// <summary>
        ///     The main method of the program.
        /// </summary>
        /// <param name="args">Some parameters, currently unused.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Setting up the database...");
            var context = new MqttContext(new DatabaseConnectionSettings
                {Host = "localhost", Database = "mqtt", Port = 5432, Username = "postgres", Password = "postgres"});

            Console.WriteLine("Delete database...");
            context.Database.EnsureDeleted();

            Console.WriteLine("Create database...");
            context.Database.EnsureCreated();

            Console.WriteLine("Adding seed data...");
            SeedData(context);

            Console.WriteLine("Press any key to close this window...");
            Console.ReadKey();
        }

        /// <summary>
        ///     Seeds the database with some data. Use this method to add custom data as needed.
        /// </summary>
        /// <param name="context">The <see cref="MqttContext" /> to use.</param>
        private static void SeedData(MqttContext context)
        {
            var version = new DbVersion {Version = "1.0.0.0", VersionName = "Sicario", CreatedAt = DateTimeOffset.Now};
            context.DbVersions.Add(version);
            context.SaveChanges();

            var user = new User
            {
                UserName = "Hans",
                AccessFailedCount = 0,
                ConcurrencyStamp = new Guid().ToString(),
                CreatedAt = DateTimeOffset.Now,
                Email = "hans@hans.de",
                EmailConfirmed = true,
                LockoutEnabled = false,
                NormalizedEmail = "HANS@HANS.DE",
                NormalizedUserName = "HANS",
                PhoneNumber = "01234567890",
                SecurityStamp = new Guid().ToString(),
                TwoFactorEnabled = false,
                PhoneNumberConfirmed = true
            };

            context.Users.Add(user);

            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, "Hans");

            context.SaveChanges();

            context.UserClaims.Add(new UserClaim
            {
                ClaimType = "SubscriptionBlacklist",
                ClaimValue = JsonConvert.SerializeObject(new List<string> {"a", "b/+", "c/#"}),
                UserId = 1,
                CreatedAt = DateTimeOffset.Now
            });

            context.UserClaims.Add(new UserClaim
            {
                ClaimType = "SubscriptionWhitelist",
                ClaimValue = JsonConvert.SerializeObject(new List<string> {"d", "e/+", "f/#"}),
                UserId = 1,
                CreatedAt = DateTimeOffset.Now
            });

            context.UserClaims.Add(new UserClaim
            {
                ClaimType = "PublishBlacklist",
                ClaimValue = JsonConvert.SerializeObject(new List<string> {"a", "b/+", "c/#"}),
                UserId = 1,
                CreatedAt = DateTimeOffset.Now
            });

            context.UserClaims.Add(new UserClaim
            {
                ClaimType = "PublishWhitelist",
                ClaimValue = JsonConvert.SerializeObject(new List<string> {"d", "e/+", "f/#"}),
                UserId = 1,
                CreatedAt = DateTimeOffset.Now
            });

            context.SaveChanges();
        }
    }
}