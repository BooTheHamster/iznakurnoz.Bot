using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using iznakurnoz.Bot.Services;
using Iznakurnoz.Bot.Configuration;
using Iznakurnoz.Bot.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace Iznakurnoz.Bot
{
    internal class BotService : IHostedService, IDisposable
    {
        private const string CommandPrefix = "/";
        private const int StartWaitDelayInMilliseconds = 10000;
        private readonly ILogger _logger;
        private IBotTelegramClientControl _botClientControl;
        private readonly IBotTelegramClient _botClient;
        private BotConfig _config;
        private IDictionary<string, IBotCommandHandler> _botCommandHandlers = new Dictionary<string, IBotCommandHandler>();
        private readonly IEnumerable<IBotDocumentHandler> _botDocumentHandlers;

        public BotService(
            ILogger<BotService> logger,
            IOptionsMonitor<BotConfig> configMonitor,
            IEnumerable<IBotCommandHandler> botCommandHandlers,
            IEnumerable<IBotDocumentHandler> botDocumentHandlers,
            IBotTelegramClientControl botClientControl,
            IBotTelegramClient botClient)
        {
            _logger = logger;
            _botClientControl = botClientControl;
            _botClient = botClient;
            _config = configMonitor.CurrentValue;
            _botDocumentHandlers = botDocumentHandlers.ToArray();

            foreach (var botCommand in botCommandHandlers)
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

                    _botCommandHandlers.Add(commandText.ToLower(), botCommand);
                }
            }

            configMonitor.OnChange(OnOptionChanged);
            _botClientControl.OnMessageReceived += BotOnMessageReceived;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting bot.");
            return GetStartTask(0);

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping bot.");
            _botClientControl.Stop();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _botClientControl = null;
            _logger.LogInformation("Disposing ...");
        }

        private Task TryStartBot()
        {
            if (!ConfigurationChecker.CheckConfig(_config, _logger))
            {
                return GetStartTask();
            }

            if (string.IsNullOrWhiteSpace(_config.AuthToken))
            {
                _logger.LogInformation("AuthToken not defined. Wait configuration ...");
                return GetStartTask();
            }

            _botClientControl.SetConfig(_config);
            if (!_botClientControl.Start())
            {
                return GetStartTask();
            }

            return Task.CompletedTask;
        }

        private Task GetStartTask(int delay = StartWaitDelayInMilliseconds)
        {
            return Task.Delay(delay).ContinueWith(task => TryStartBot());
        }

        private void BotOnMessageReceived(object sender, MessageEventArgs messageEvent)
        {
            if (messageEvent.Message.Document != null)
            {
                HandleDocument(messageEvent.Message);
                return;
            }

            HandleCommand(messageEvent.Message);
        }

        private void HandleDocument(Message message)
        {
            _logger.LogInformation($"{message.Document.FileName}");

            foreach (var documentHandler in _botDocumentHandlers)
            {
                try
                {
                    if (documentHandler.HandleDocument(message))
                    {
                        return;
                    }
                }
                catch (Exception error)
                {
                    _logger.LogError(error, $"Document handler ${documentHandler.GetType().Name} error");
                }
            }
        }

        private void HandleCommand(Message message)
        {
            _logger.LogInformation($"{message.Text}");

            if (ParseCommand(message.Text, out var command, out var arguments)
                && _botCommandHandlers.TryGetValue(command, out var handler))
            {
                try
                {
                    handler.HandleCommand(message, command, arguments);
                }
                catch (Exception error)
                {
                    _logger.LogError(error, $"Command {message.Text} execution error");
                }
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