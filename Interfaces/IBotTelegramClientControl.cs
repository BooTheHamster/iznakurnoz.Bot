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
        /// Конфигурирование клиента.
        /// </summary>
        /// <param name="config">Настройки бота.</param>
        void SetConfig(BotConfig config);

        /// <summary>
        /// Запуск клиента на прием сообщений.
        /// </summary>
        /// <returns></returns>
        bool Start();

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