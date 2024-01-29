using System.Collections.Generic;
using System.Threading.Tasks;
using iznakurnoz.Bot.Services.TransmissionService;
using Iznakurnoz.Bot.Interfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace iznakurnoz.Bot.CommandHandlers
{
    /// <summary>
    /// Обработчик команды запуска закачки торрентов.
    /// </summary>
    internal class TorrentStartCommandHandler : BaseCommandHandler, IBotCommandHandler
    {
        private static readonly IEnumerable<string> LocalSupportedCommands = new[]
        {
            "ts",

        };
        private readonly TransmissionService _transmissionService;

        public TorrentStartCommandHandler(TransmissionService transmissionService)
            : base(LocalSupportedCommands)
        {
            _transmissionService = transmissionService;
        }

        public Task<string> HandleCommand(Message message, string command, IEnumerable<string> arguments)
        {
            var torrentIds = new List<int>();

            foreach (var argument in arguments)
            {
                if (int.TryParse(argument, out var id))
                {
                    torrentIds.Add(id);
                }
            }

            return _transmissionService.StartTorrents(torrentIds.ToArray());
        }
    }
}