using Telegram.Bot.Types;

namespace Iznakurnoz.Bot.Interfaces
{
    /// <summary>
    /// Обработчик файлов бота.
    /// </summary>
    public interface IBotDocumentHandler
    {
        /// <summary>
        /// Обработка документа.
        /// </summary>
        /// <param name="message">Сообщение, содержащее файл.</param>
        /// <returns>Истина, если файл обработан.</returns>
        bool HandleDocument(Message message);
    }
}