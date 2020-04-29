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
            IEnumerable<IBotCommandHandler> botCommandHandlers,
            IEnumerable<IBotDocumentHandler> botDocumentHandlers,
            IBotTelegramClientControl botClientControl,
            IBotTelegramClient botClient,
            IConfigProvider configProvider)
        {
            _logger = logger;
            _botClientControl = botClientControl;
            _botClient = botClient;
            _config = configProvider.CurrentConfig;
            configProvider.Changed += OnConfigChanged;
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

            _botClientControl.OnMessageReceived += BotOnMessageReceived;
        }

        private void OnConfigChanged(object sender, BotConfig config)
        {
            _config = config;

            if (!ConfigurationChecker.CheckConfig(_config, _logger))
            {
                return;
            }

            GetStartTask(0);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting bot.");

            if (!ConfigurationChecker.CheckConfig(_config, _logger))
            {
                return Task.CompletedTask;
            }

            return GetStartTask(0);

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping bot.");
            StopClient();

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            StopClient();
            _logger.LogInformation("Disposing ...");
        }

        private void StopClient()
        {
            _botClientControl?.Stop();
            _botClientControl = null;
        }

        private Task TryStartBot()
        {
            if (_botClientControl.Start(_config))            
            {
                return Task.CompletedTask;
            }

            return GetStartTask(StartWaitDelayInMilliseconds);
        }

        private Task GetStartTask(int delay)
        {
            return Task.Delay(delay).ContinueWith(task => TryStartBot());
        }

        private void BotOnMessageReceived(object sender, MessageEventArgs messageEvent)
        {
            if (messageEvent.Message.From.Id != _config.AdminId)
            {
                return;
            }

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

        private bool ParseCommand(string message, out string command, out IReadOnlyCollection<string> arguments)
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
    }
}