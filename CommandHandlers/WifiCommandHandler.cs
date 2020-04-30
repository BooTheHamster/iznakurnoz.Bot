using System;
using System.Collections.Generic;
using Iznakurnoz.Bot.Interfaces;
using Telegram.Bot.Types;

namespace iznakurnoz.Bot.CommandHandlers
{
    /// <summary>
    /// Обработчик команды wifi.
    /// </summary>
    internal class WifiCommandHandler : BaseCommandHandler, IBotCommandHandler
    {
        private static IEnumerable<string> _supportedCommands = new[]
        {
            "wifi"
        };

        /// <summary>
        /// Карта соответствия настроек и методов-обработчиков настроек.
        /// </summary>
        private readonly IDictionary<string, Action<Message, IEnumerator<string>>> _optionHandlers;
        private readonly RouterRequestService _routerRequestService;

        public WifiCommandHandler(
            IBotTelegramClient botTelegramClient,
            RouterRequestService routerRequestService)
            : base(botTelegramClient)
        {
            _routerRequestService = routerRequestService;
        }

        public IEnumerable<string> SupportedCommands => _supportedCommands;

        public WifiCommandHandler(IBotTelegramClient botTelegramClient)
            : base(botTelegramClient)
        {
            _optionHandlers = new Dictionary<string, Action<Message, IEnumerator<string>>>()
            {
                { "state", StateOptionHandler }
            };
        }

        public void HandleCommand(Message message, string command, IReadOnlyCollection<string> arguments)
        {
            var argumentEnumerator = arguments.GetEnumerator();

            if (!argumentEnumerator.MoveNext())
            {
                StateOptionHandler(message, null);
                return;
            }

            if (_optionHandlers.TryGetValue(argumentEnumerator.Current, out var handler))
            {
                handler(message, argumentEnumerator);
            }
        }

        async private void StateOptionHandler(Message message, IEnumerator<string> parameters)
        {
            var result = await _routerRequestService.GetWirelessFilterPage();

            if (string.IsNullOrWhiteSpace(result))
            {
                result = "Не удалось получить информацию о состоянии фильтра.";
            }

            BotClient.SendTextMessage(message.Chat, result);
        }

        async private void MacListOptionHandler(Message message, IEnumerator<string> parameters)
        {
        }
    }
}