using System.Collections.Generic;
using Iznakurnoz.Bot.Interfaces;
using Telegram.Bot.Types;

namespace iznakurnoz.Bot.CommandHandlers
{
    /// <summary>
    /// Обработчик команды "hi".
    /// </summary>
    internal class HiCommandHandler : BaseCommandHandler, IBotCommandHandler
    {
        private static IEnumerable<string> _supportedCommands = new[]
        {
            "hi"
        };

        public HiCommandHandler(IBotTelegramClient botTelegramClient) 
            : base(botTelegramClient)
        {
        }

        public IEnumerable<string> SupportedCommands => _supportedCommands;

        public void HandleCommand(Message message, string command, IEnumerable<string> arguments)
        {
            BotClient.SendTextMessage(message.Chat, $"<code>hi {message.From.FirstName}! C#</code>");
        }
    }
}