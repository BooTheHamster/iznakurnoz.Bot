using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Iznakurnoz.Bot.Interfaces
{
    /// <summary>
    /// Обработчик команд бота.
    /// </summary>
    public interface IBotCommandHandler
    {
        /// <summary>
        /// Список поддерживаемых комманд.
        /// </summary>
        IEnumerable<string> SupportedCommands { get;}

        /// <summary>
        /// Обработка команды.
        /// </summary>
        /// <param name="command">Сообщение.</param>
        /// <param name="command">Команда.</param>
        /// <param name="arguments">Параметры команды.</param>
        /// <returns>Ответное сообщение после обработки команды которое бот отправит. Если сообщение не нужно необходимо вернуть null в качестве текста сообщения.</returns>
        Task<string> HandleCommand(Message message, string command, IEnumerable<string> arguments);
    }
}