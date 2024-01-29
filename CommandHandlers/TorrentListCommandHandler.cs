using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using iznakurnoz.Bot.Services.TransmissionService;
using iznakurnoz.Bot.Services.TransmissionService.Interfaces;
using Iznakurnoz.Bot.Interfaces;
using Telegram.Bot.Types;

namespace iznakurnoz.Bot.CommandHandlers
{

    /// <summary>
    /// Обработчик команды получения списка текущих торрентов.
    /// </summary>
    internal class TorrentListCommandHandler : BaseCommandHandler, IBotCommandHandler
    {
        private static readonly IEnumerable<string> LocalSupportedCommands = new[]
        {
            "tl",

        };
        private readonly TransmissionService _transmissionService;

        public TorrentListCommandHandler(TransmissionService transmissionService)
            : base(LocalSupportedCommands)
        {
            _transmissionService = transmissionService;
        }

        public async Task<string> HandleCommand(Message message, string command, IEnumerable<string> arguments)
        {
            var torrents = await _transmissionService.GetTorrentsList();

            if (torrents == null || torrents.Length == 0)
            {
                return "Нет активных торрентов";
            }

            var builder = new StringBuilder();
            builder.AppendLine($"<code>Число торрентов: {torrents.Length}</code>");

            foreach (var torrent in torrents)
            {
                var icon = GetIcon(torrent);

                builder.Append($"{icon}<code> {torrent.Id}. {torrent.Name}");

                if (!torrent.IsComplete)
                {
                    builder.Append($" {torrent.PercentDone * 100:##.##}%");
                }

                if (!string.IsNullOrWhiteSpace(torrent.ErrorString))
                {
                    builder.Append($" Ошибка: \"{torrent.ErrorString}\"");
                }

                builder.AppendLine("</code>");
            }

            return builder.ToString();
        }

        private static string GetIcon(ITorrentInformation torrent)
        {
            if (!string.IsNullOrWhiteSpace(torrent.ErrorString))
            {
                return "\u274c";
            }

            string icon;
            if (torrent.IsComplete)
            {
                icon = "\u2714";
            }
            else if (torrent.PercentDone > 0)
            {
                switch (torrent.Status)
                {
                    case TorrentStatus.DownloadWait:
                        {
                            icon = "\u23F3";
                            break;
                        }

                    case TorrentStatus.Downloading:
                        {
                            icon = "\u3030";
                            break;
                        }

                    case TorrentStatus.Stopped:
                        {
                            icon = "\u25fc";
                            break;
                        }

                    default:
                        {
                            icon = "";
                            break;
                        }
                }

            }
            else
            {
                icon = "\u25ab";
            }

            return icon;
        }
    }
}