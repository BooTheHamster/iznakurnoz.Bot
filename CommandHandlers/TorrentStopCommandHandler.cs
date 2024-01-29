using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using iznakurnoz.Bot.Services.TransmissionService;
using Iznakurnoz.Bot.Interfaces;
using Telegram.Bot.Types;

namespace iznakurnoz.Bot.CommandHandlers
{
    /// <summary>
    /// Обработчик команды остановки закачки торрентов.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    internal class TorrentStopCommandHandler : BaseCommandHandler, IBotCommandHandler
    {
        private static readonly IEnumerable<string> LocalSupportedCommands = new[]
        {
            "tstop",

        };
        private readonly TransmissionService _transmissionService;

        public TorrentStopCommandHandler(TransmissionService transmissionService)
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

            return _transmissionService.StopTorrents(torrentIds.ToArray());
        }
    }
}