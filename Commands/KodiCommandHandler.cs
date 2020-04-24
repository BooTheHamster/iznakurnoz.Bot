using System.Collections.Generic;
using System.Diagnostics;
using Iznakurnoz.Bot.Interfaces;

namespace iznakurnoz.Bot.Commands
{
    /// <summary>
    /// Обработчик команды "/kodi".
    /// 
    /// Обработчик требует запуска бота от имени администратора.
    /// </summary>
    public class KodiCommandHandler : IBotCommandHandler
    {
        private static IEnumerable<string> _supportedCommands = new[]
        {
            "kodi"
        };

        public IEnumerable<string> SupportedCommands => _supportedCommands;

        public string HandleCommand(string command, IEnumerable<string> arguments)
        {
            var localAll = Process.GetProcesses();

            foreach (var process in localAll)
            {
                if (process.ProcessName.Contains("kodi-standalone"))
                {
                    process.Kill();
                    break;
                }
            }

            return "Kodi restarted ...";
        }
    }

}