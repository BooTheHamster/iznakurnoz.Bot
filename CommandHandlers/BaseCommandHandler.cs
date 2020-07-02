using System.Collections.Generic;
using System.Threading.Tasks;
using Iznakurnoz.Bot.Interfaces;
using Telegram.Bot.Types;

namespace iznakurnoz.Bot.CommandHandlers
{
    /// <summary>
    /// Базовый класс для обработчиков команд.
    /// </summary>
    internal abstract class BaseCommandHandler
    {
        public delegate Task<string> OptionHandlerDelegate(Message message, IEnumerator<string> arguments);

        private IEnumerable<string> _supportedCommands;

        protected IBotTelegramClient BotClient { get; }

        public IEnumerable<string> SupportedCommands => _supportedCommands;

        protected BaseCommandHandler(
            IBotTelegramClient botTelegramClient,
            IEnumerable<string> supportedCommands)
        {
            BotClient = botTelegramClient;
            _supportedCommands = supportedCommands;
        }

        protected Task<string> GetSilentResult()
        {
            return Task.FromResult<string>(null);
        }

        protected Task<string> GetAsTextResult(string message)
        {
            return Task.FromResult(message);
        }

        protected Task<string> GetCommandAgrumentError()
        {
            return GetAsTextResult("Не заданы обязательные параметры команды");
        }
    }
}