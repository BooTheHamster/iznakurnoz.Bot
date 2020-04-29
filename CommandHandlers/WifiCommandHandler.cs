using System.Collections.Generic;
using Iznakurnoz.Bot.Interfaces;
using Telegram.Bot.Types;

namespace iznakurnoz.Bot.CommandHandlers
{
    internal class WifiCommandHandler : BaseCommandHandler, IBotCommandHandler
    {
        private static IEnumerable<string> _supportedCommands = new[]
        {
            "wifi"
        };

        public WifiCommandHandler(IBotTelegramClient botTelegramClient) 
            : base(botTelegramClient)
        {
        }

        public IEnumerable<string> SupportedCommands => _supportedCommands;

        public void HandleCommand(Message message, string command, IReadOnlyCollection<string> arguments)
        {
            if (arguments.Count == 0)
            {
                BotClient.SendTextMessage(message.Chat, "Ok");
                return;
            }

        }
    }
}