using System.Collections.Generic;
using Iznakurnoz.Bot.Interfaces;

namespace iznakurnoz.Bot.CommandHandlers
{
    /// <summary>
    /// Обработчик команды "hi".
    /// </summary>
    internal class HiCommandHandler : IBotCommandHandler
    {
        private static IEnumerable<string> _supportedCommands = new[]
        {
            "hi"
        };

        public IEnumerable<string> SupportedCommands => _supportedCommands;

        public string HandleCommand(string command, IEnumerable<string> arguments)
        {
            return "<code>hi! C#</code>";
        }
    }
}