using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Iznakurnoz.Bot
{
    class Program
    {
        private const string appSettingsFilename = "iznakurnozbot.conf";

        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    ConfigureHost(config, args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions();
                    services.Configure<BotConfig>(hostContext.Configuration.GetSection("config"));
                    services.AddSingleton<IHostedService, BotService>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("logging"));
                    logging.AddConsole();
                });

            await builder.RunConsoleAsync();
        }

        private static string GetAppSettingsFilePath()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appDataFolder, appSettingsFilename);
        }

        private static void ConfigureHost(IConfigurationBuilder builder, string[] args)
        {
            var configFilePath = GetAppSettingsFilePath();

            if (!File.Exists(configFilePath))
            {
                var assembly = Assembly.GetEntryAssembly();
                using (var resource = assembly.GetManifestResourceStream("iznakurnoz.Bot.Resources.iznakurnozbot.conf"))
                {
                    using (var textStream = new FileStream(configFilePath, FileMode.Create))
                    {
                        resource.Seek(0, SeekOrigin.Begin);
                        resource.CopyTo(textStream);
                        textStream.Flush();
                        textStream.Close();
                    }
                }
            }

            builder
                .AddJsonFile(configFilePath, optional : false, reloadOnChange : true)
                .Build();

            builder.AddEnvironmentVariables();

            if (args != null)
            {
                builder.AddCommandLine(args);
            }
        }
    }
}