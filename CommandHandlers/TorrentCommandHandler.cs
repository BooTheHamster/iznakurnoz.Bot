using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Iznakurnoz.Bot.Interfaces;
using Telegram.Bot.Types;

namespace iznakurnoz.Bot.CommandHandlers
{
    /// <summary>
    /// Обработчик команд для работы с торрентами.
    /// </summary>
    internal class TorrentCommandHandler : BaseCommandHandler, IBotCommandHandler
    {
        private static readonly IEnumerable<string> LocalSupportedCommands = new[]
        {
            "torrent"
        };
        private readonly TorrentListCommandHandler _torrentListCommandHandler;
        
        /// <summary>
        /// Карта соответствия настроек и методов-обработчиков настроек.
        /// </summary>
        private readonly IDictionary<string, OptionHandlerDelegate> _optionHandlers;

        public TorrentCommandHandler(
            TorrentListCommandHandler torrentListCommandHandler)
            : base(LocalSupportedCommands)
        {
            _torrentListCommandHandler = torrentListCommandHandler;
            _optionHandlers = new Dictionary<string, OptionHandlerDelegate>()
            {
                { "l", TorrentListOptionHandler },
                { "list", TorrentListOptionHandler },
            };           
        }

        public Task<string> HandleCommand(Message message, string command, IEnumerable<string> arguments)
        {
            var argumentEnumerator = arguments.GetEnumerator();

            if (!argumentEnumerator.MoveNext() || argumentEnumerator.Current == null)
            {
                return GetCommandAgrumentError();
            }

            return _optionHandlers.TryGetValue(argumentEnumerator.Current, out var handler)
                ? handler(message, argumentEnumerator)
                : GetSilentResult();
        }

        private Task<string> TorrentListOptionHandler(Message message, IEnumerator<string> arguments)
        {
            return _torrentListCommandHandler.HandleCommand(message, string.Empty, Enumerable.Empty<string>());
        }
    }
}