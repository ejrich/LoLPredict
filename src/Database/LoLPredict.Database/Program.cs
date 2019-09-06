using System;
using System.Linq;
using System.Reflection;
using DbUp;
using Microsoft.Extensions.Configuration;

namespace LoLPredict.Database
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var connectionString = args.FirstOrDefault();

            if (connectionString == null)
            {
                Console.WriteLine("Couldn't find the command line argument, using the default configuration");

                var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

                connectionString = config["ConnectionString"];
            }

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
