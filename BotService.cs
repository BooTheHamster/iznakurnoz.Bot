using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MihaZupan;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;

namespace Iznakurnoz.Bot
{
    internal class BotService : IHostedService, IDisposable
    {
        private const int StartWaitDelayInMilliseconds = 10000;
        private readonly ILogger _logger;
        private BotConfig _config;
        private TelegramBotClient _client;

        public BotService(
            ILogger<BotService> logger,
            IOptionsMonitor<BotConfig> config)
        {
            _logger = logger;
            _config = config.CurrentValue;
            config.OnChange(OnOptionChanged);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting bot.");
            return GetStartTask(0);

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping bot.");
            _client.StopReceiving();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _client = null;
            _logger.LogInformation("Disposing ...");
        }

        private static bool CheckConfig(BotConfig config, ILogger logger)
        {
            var validationErrors = new List<string>();

            if (config == null)
            {
                validationErrors.Add("No configuration found.");
            }
            else
            {

                if (string.IsNullOrWhiteSpace(config.AuthToken))
                {
                    validationErrors.Add("AuthToken not defined.");
                }
            }

            if (config.ProxySettings == null)
            {
                validationErrors.Add("No proxy settings found.");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(config.ProxySettings.Address))
                {
                    validationErrors.Add("Proxy server address not defined.");
                }

                if (config.ProxySettings.Port <= 0)
                {
                    validationErrors.Add("Proxy port not defined.");
                }

                if (string.IsNullOrWhiteSpace(config.ProxySettings.Username))
                {
                    validationErrors.Add("Proxy username not defined.");
                }

                if (string.IsNullOrWhiteSpace(config.ProxySettings.Password))
                {
                    validationErrors.Add("Proxy password not defined.");
                }
            }

            if (validationErrors.Count > 0)
            {
                validationErrors.Add("Awaiting configuration ...");

                foreach (var line in validationErrors)
                {
                    logger.LogInformation(line);
                }

                return false;
            }

            return true;
        }

        private Task TryStartBot()
        {
            if (!CheckConfig(_config, _logger))
            {
                return GetStartTask();
            }

            if (string.IsNullOrWhiteSpace(_config.AuthToken))
            {
                _logger.LogInformation("AuthToken not defined. Wait configuration ...");
                return GetStartTask();
            }

            try
            {
                var proxy = new HttpToSocks5Proxy(
                    _config.ProxySettings.Address,
                    _config.ProxySettings.Port,
                    _config.ProxySettings.Username,
                    _config.ProxySettings.Password);
                _client = new TelegramBotClient(_config.AuthToken, proxy);
            }
            catch (Exception error)
            {
                _logger.LogError($"TelegramBotClient create error: {error.Message}");
                return GetStartTask();
            }

            _client.OnMessage += BotOnMessageReceived;
            _client.OnMessageEdited += BotOnMessageReceived;
            _client.StartReceiving();

            return Task.CompletedTask;
        }

        private Task GetStartTask(int delay = StartWaitDelayInMilliseconds)
        {
            return Task.Delay(delay).ContinueWith(task => TryStartBot());
        }

        private void BotOnMessageReceived(object sender, MessageEventArgs messageEvent)
        {
            _logger.LogInformation($"{messageEvent.Message.Text}");
            _client.SendTextMessageAsync(messageEvent.Message.Chat, "<code>hi! C#</code>", ParseMode.Html);
        }

        private void OnOptionChanged(BotConfig config, string name)
        {
            _config = config;
            _logger.LogInformation($"Bot configuration reloaded");
        }
    }
}