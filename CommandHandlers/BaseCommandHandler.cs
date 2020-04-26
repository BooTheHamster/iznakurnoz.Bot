using Iznakurnoz.Bot.Interfaces;

namespace iznakurnoz.Bot.CommandHandlers
{
    /// <summary>
    /// Базовый класс для обработчиков команд.
    /// </summary>
    internal abstract class BaseCommandHandler
    {
        protected IBotTelegramClient BotClient { get; }

        protected BaseCommandHandler(IBotTelegramClient botTelegramClient)
        {
            BotClient = botTelegramClient;
        }
    }
}