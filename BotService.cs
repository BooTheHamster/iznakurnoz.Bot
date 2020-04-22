using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Iznakurnoz.Bot.Configuration;
using Iznakurnoz.Bot.Interfaces;
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
        private const string CommandPrefix = "/";
        private const int StartWaitDelayInMilliseconds = 10000;
        private readonly ILogger _logger;
        private BotConfig _config;
        private TelegramBotClient _client;
        private IDictionary<string, IBotCommandHandler> _botCommands = new Dictionary<string, IBotCommandHandler>();

        public BotService(
            ILogger<BotService> logger,
            IOptionsMonitor<BotConfig> config,
            IEnumerable<IBotCommandHandler> botCommands)
        {
            _logger = logger;
            _config = config.CurrentValue;
            
            foreach (var botCommand in botCommands)
            {
                foreach (var command in botCommand.SupportedCommands)
                {                    
                    if (string.IsNullOrWhiteSpace(command))
                    {
                        continue;
                    }

                    var commandText = command;
                    if (!IsCommand(commandText))
                    {
                        commandText = CommandPrefix + commandText;
                    }

                    _botCommands.Add(commandText.ToLower(), botCommand);
                }
            }

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

            if (ParseCommand(messageEvent.Message.Text, out var command, out var arguments)
                && _botCommands.TryGetValue(command, out var handler))
            {
                var result = handler.HandleCommand(command, arguments);
                _client.SendTextMessageAsync(messageEvent.Message.Chat, result, ParseMode.Html);
            }
        }

        private bool ParseCommand(string message, out string command, out IEnumerable<string> arguments)
        {
            command = null;
            arguments = null;

            if (string.IsNullOrWhiteSpace(message))
            {
                return false;
            }

            var messageWords = message.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

            if (messageWords.Count == 0)
            {
                return false;
            }

            // Команды делаем не чувствительными к регистру.
            command = messageWords.First().ToLower();

            if (!IsCommand(command))
            {
                command = null;
                return false;
            }

            messageWords.RemoveAt(0);
            arguments = messageWords;

            return true;
        }

        private bool IsCommand(string text)
        {
            return text.StartsWith(CommandPrefix);
        }

        private void OnOptionChanged(BotConfig config, string name)
        {
            _config = config;
            _logger.LogInformation($"Bot configuration reloaded");
        }
    }
}