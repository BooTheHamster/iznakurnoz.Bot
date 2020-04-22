using System.Collections.Generic;

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
        /// <param name="command">Команда.</param>
        /// <param name="arguments">Параметры команды.</param>
        /// <returns>Ответное сообщение после обработки команды которое бот отправит.</returns>
        string HandleCommand(string command, IEnumerable<string> arguments);
    }
}