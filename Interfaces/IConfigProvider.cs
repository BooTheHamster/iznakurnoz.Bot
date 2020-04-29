using System;
using Iznakurnoz.Bot.Configuration;

namespace Iznakurnoz.Bot.Interfaces
{
    /// <summary>
    /// Провайдер текущей конфигурации бота.
    /// </summary>
    internal interface IConfigProvider
    {
        /// <summary>
        /// Событие об изменении настроек бота.
        /// </summary>
        event EventHandler<BotConfig> Changed;
        
        /// <summary>
        /// Текущая конфигурация бота.
        /// </summary>
        BotConfig CurrentConfig { get;}
    }
}