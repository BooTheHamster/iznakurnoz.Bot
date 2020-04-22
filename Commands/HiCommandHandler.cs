using System.Collections.Generic;
using Iznakurnoz.Bot.Interfaces;

namespace iznakurnoz.Bot.Commands
{
    /// <summary>
    /// Обработчик команды "hi".
    /// </summary>
    public class HiCommandHandler : IBotCommandHandler
    {
        private static IEnumerable<string> _supportedCommands = new []
        {
            "hi"
        };

        public IEnumerable<string> SupportedCommands => _supportedCommands;

        public string HandleCommand(string command, IEnumerable<string> arguments)
        {
            return "<code>hi! C#</code>";
        }
    }

    /// <summary>
    /// Обработчик команды "hi".
    /// </summary>
    public class HiCommandHandler2 : IBotCommandHandler
    {
        private static IEnumerable<string> _supportedCommands = new []
        {
            "hi2"
        };

        public IEnumerable<string> SupportedCommands => _supportedCommands;

        public string HandleCommand(string command, IEnumerable<string> arguments)
        {
            return "<code>hi2! C#</code>";
        }
    }    
}