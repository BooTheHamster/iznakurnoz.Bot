using System.Collections.Generic;
using System.Diagnostics;
using Iznakurnoz.Bot.Interfaces;
using Telegram.Bot.Types;

namespace iznakurnoz.Bot.CommandHandlers
{
    /// <summary>
    /// Обработчик команды "/kodi".
    /// 
    /// Обработчик требует запуска бота от имени администратора.
    /// </summary>
    internal class KodiCommandHandler : BaseCommandHandler, IBotCommandHandler
    {
        private static IEnumerable<string> _supportedCommands = new[]
        {
            "kodi"
        };

        public KodiCommandHandler(IBotTelegramClient botTelegramClient) 
            : base(botTelegramClient)
        {
        }

        public IEnumerable<string> SupportedCommands => _supportedCommands;

        public void HandleCommand(Message message, string command, IEnumerable<string> arguments)
        {
            var localAll = Process.GetProcesses();

            foreach (var process in localAll)
            {
                if (process.ProcessName.Contains("kodi-standalone"))
                {
                    // "Убивается" процесс Kodi чтобы он перезапустился заново.
                    process.Kill();
                    BotClient.SendTextMessage(message.Chat, "Kodi перезапущен.");
                    break;
                }
            }

            BotClient.SendTextMessage(message.Chat, "Не найдено запущенного экземпляра Kodi.");
        }
    }
}