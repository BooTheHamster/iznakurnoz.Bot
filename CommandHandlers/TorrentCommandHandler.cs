using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Iznakurnoz.Bot.Interfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace iznakurnoz.Bot.CommandHandlers
{
    /// <summary>
    /// Обработчик команд для работы с торрентами.
    /// </summary>
    internal class TorrentCommandHandler : BaseCommandHandler, IBotCommandHandler
    {
        private static IEnumerable<string> _supportedCommands = new[]
        {
            "torrent"
        };
        private readonly TorrentListCommandHandler _torrentListCommandHandler;
        private readonly ILogger<TorrentCommandHandler> _logger;
        
        /// <summary>
        /// Карта соответствия настроек и методов-обработчиков настроек.
        /// </summary>
        private readonly IDictionary<string, OptionHandlerDelegate> _optionHandlers;

        public TorrentCommandHandler(
            IBotTelegramClient botTelegramClient,
            TorrentListCommandHandler torrentListCommandHandler,
            ILogger<TorrentCommandHandler> logger)
            : base(botTelegramClient, _supportedCommands)
        {
            _torrentListCommandHandler = torrentListCommandHandler;
            _logger = logger;
            _optionHandlers = new Dictionary<string, OptionHandlerDelegate>()
            {
                { "l", TorrentListOptionHandler },
                { "list", TorrentListOptionHandler },
            };           
        }

        public Task<string> HandleCommand(Message message, string command, IEnumerable<string> arguments)
        {
            var argumentEnumerator = arguments.GetEnumerator();

            if (!argumentEnumerator.MoveNext())
            {
                return GetCommandAgrumentError();
            }

            if (_optionHandlers.TryGetValue(argumentEnumerator.Current, out var handler))
            {
                return handler(message, argumentEnumerator);
            }

            return GetSilentResult();
        }

        private Task<string> TorrentListOptionHandler(Message message, IEnumerator<string> arguments)
        {
            return _torrentListCommandHandler.HandleCommand(message, string.Empty, Enumerable.Empty<string>());
        }
    }
}