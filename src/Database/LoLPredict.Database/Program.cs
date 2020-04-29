using System;
using System.Reflection;
using DbUp;
using Microsoft.Extensions.Configuration;

namespace LoLPredict.Database
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var connectionString = string.Join(" ", args);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

                connectionString = config["ConnectionString"];
            }

            Console.WriteLine($"Using connection string: {connectionString}");

            EnsureDatabase.For.SqlDatabase(connectionString);

            var upgradeEngine =
                DeployChanges.To
                    .SqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                    .LogToConsole()
                    .Build();

            var result = upgradeEngine.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
                return -1;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
            return 0;
        }
    }
}
