using System;
using Iznakurnoz.Bot.Configuration;
using Telegram.Bot.Args;

namespace Iznakurnoz.Bot.Interfaces
{
    /// <summary>
    /// Управление телеграм клиентом бота.
    /// </summary>
    internal interface IBotTelegramClientControl
    {
        /// <summary>
        /// Запуск клиента на прием сообщений.
        /// </summary>
        /// <param name="config">Настройки бота.</param>
        /// <returns>Истина, если бот запущен.</returns>
        bool Start(BotConfig config);

        /// <summary>
        /// Остановить клиент бота.
        /// </summary>
        void Stop();

        /// <summary>
        /// Событие формируемое при получении клиентом сообщения.
        /// </summary>
        event EventHandler<MessageEventArgs> OnMessageReceived;
    }
}