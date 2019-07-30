
namespace DatabaseSetup
{
    using Storage;
    using Storage.Database;
    using System;

    /// <summary>
    /// A program to setup the database.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main method of the program.
        /// </summary>
        /// <param name="args">Some parameters, currently unused.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine("Setting up the database...");
            var context = new DbContext(new DatabaseConnectionSettings { Host = "localhost", Database = "mqtt", Port = 5432, Username = "mqtt", Password = "mqtt" });

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
        /// Seeds the database with some data. Use this method to add custom data as needed.
        /// </summary>
        /// <param name="context">The <see cref="DbContext"/> to use.</param>
        private static void SeedData(DbContext context)
        {
            context.Users.Add(new User()
            {
                UserName = "Hans"
            });

            context.SaveChanges();

            context.UserClaims.Add(new UserClaim()
            {
                ClaimType = "",
                ClaimValue = "",
                UserId = 1
            });

            context.SaveChanges();
        }
    }
}
