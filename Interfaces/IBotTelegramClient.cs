using System.IO;
using System.Threading.Tasks;
using Iznakurnoz.Bot.Configuration;
using Telegram.Bot.Types;

namespace Iznakurnoz.Bot.Interfaces
{
    /// <summary>
    /// Клиент телеграма для бота.
    /// </summary>
    internal interface IBotTelegramClient
    {
        /// <summary>
        /// Отправка сообщения в чат.
        /// </summary>
        /// <param name="chat">Чат.</param>
        /// <param name="message">Сообщение.</param>
        void SendTextMessage(Chat chat, string message);

        /// <summary>
        /// Загрузка файла.
        /// </summary>
        /// <param name="fileId">Идентификатор файла.</param>
        /// <returns>Файл.</returns>
        Task<Stream> GetFile(string fileId);

        /// <summary>
        /// Конфигурация бота.
        /// </summary>
        /// <value></value>
        BotConfig Config { get; }
    }
}