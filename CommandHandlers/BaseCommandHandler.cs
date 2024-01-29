using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace iznakurnoz.Bot.CommandHandlers
{
    /// <summary>
    /// Базовый класс для обработчиков команд.
    /// </summary>
    internal abstract class BaseCommandHandler
    {
        protected delegate Task<string> OptionHandlerDelegate(Message message, IEnumerator<string> arguments);

        public IEnumerable<string> SupportedCommands { get; }

        protected BaseCommandHandler(IEnumerable<string> supportedCommands)
        {
            SupportedCommands = supportedCommands;
        }

        protected static Task<string> GetSilentResult()
        {
            return Task.FromResult<string>(null);
        }

        protected static Task<string> GetAsTextResult(string message)
        {
            return Task.FromResult(message);
        }

        protected static Task<string> GetCommandAgrumentError()
        {
            return GetAsTextResult("Не заданы обязательные параметры команды");
        }
    }
}