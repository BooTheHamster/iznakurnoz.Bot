using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
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
            : base(botTelegramClient, _supportedCommands)
        {
        }

        public Task<string> HandleCommand(Message message, string command, IEnumerable<string> arguments)
        {
            var localAll = Process.GetProcesses();

            foreach (var process in localAll)
            {
                if (process.ProcessName.Contains("kodi-standalone"))
                {
                    // "Убивается" процесс Kodi чтобы он перезапустился заново.
                    process.Kill();
                    return GetAsTextResult("Kodi перезапущен.");
                }
            }

            return GetAsTextResult("Не найдено запущенного экземпляра Kodi.");
        }
    }
}