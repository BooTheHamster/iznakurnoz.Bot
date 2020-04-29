using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using iznakurnoz.Bot.CommandHandlers;
using iznakurnoz.Bot.DocumentHandlers;
using iznakurnoz.Bot.Interfaces;
using iznakurnoz.Bot.Services;
using Iznakurnoz.Bot.Configuration;
using Iznakurnoz.Bot.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Iznakurnoz.Bot
{

    class Program
    {
        private const string AppSettingsFilename = "iznakurnozbot.conf";

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
                    services.AddSingleton<IDataStorage, DataStorage>();
                    services.AddSingleton<IConfigProvider, ConfigProvider>();
                    services.AddSingleton<BotTelegramClient, BotTelegramClient>();
                    services.AddSingleton<IBotTelegramClient>(s => s.GetRequiredService<BotTelegramClient>());
                    services.AddSingleton<IBotTelegramClientControl>(s => s.GetRequiredService<BotTelegramClient>());

                    services.AddSingleton<IBotCommandHandler, HiCommandHandler>();
                    services.AddSingleton<IBotCommandHandler, KodiCommandHandler>();
                    services.AddSingleton<IBotCommandHandler, WifiCommandHandler>();
                    services.AddSingleton<IBotDocumentHandler, TorrentDocumentHandler>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    ConfigureLogging(logging);
                });

            await builder.RunConsoleAsync();
        }

        private static void ConfigureLogging(ILoggingBuilder logging)
        {
            logging.AddConsole();
        }

        private static void ConfigureHost(IConfigurationBuilder builder, string[] args)
        {
            var configDirectoryPath = FilePathProvider.GetConfigDirectoryPath();

            if (!Directory.Exists(configDirectoryPath))
            {
                Directory.CreateDirectory(configDirectoryPath);
            }  

            var configFilePath = Path.Combine(configDirectoryPath, AppSettingsFilename);

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
                .AddJsonFile(configFilePath, optional: false, reloadOnChange: true)
                .Build();

            builder.AddEnvironmentVariables();

            if (args != null)
            {
                builder.AddCommandLine(args);
            }
        }
    }
}