using System.Collections.Generic;
using System.Threading.Tasks;
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
            : base(botTelegramClient, _supportedCommands)
        {
        }

        public Task<string> HandleCommand(Message message, string command, IEnumerable<string> arguments)
        {
            return GetAsTextResult($"<code>hi {message.From.FirstName}! C#</code>");
        }
    }
}