using System.Collections.Generic;
using System.Text;
using iznakurnoz.Bot.Services.TransmissionService;
using iznakurnoz.Bot.Services.TransmissionService.Interfaces;
using Iznakurnoz.Bot.Interfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace iznakurnoz.Bot.CommandHandlers
{
    /// <summary>
    /// Обработчик команды "tl".
    /// </summary>
    internal class TorrentListCommandHandler : BaseCommandHandler, IBotCommandHandler
    {
        private static IEnumerable<string> _supportedCommands = new[]
        {
            "tl",
        };
        private readonly TransmissionService _transmissionService;
        private readonly ILogger<TorrentListCommandHandler> _logger;

        public TorrentListCommandHandler(
            IBotTelegramClient botTelegramClient,
            TransmissionService transmissionService,
            ILogger<TorrentListCommandHandler> logger)
            : base(botTelegramClient)
        {
            _transmissionService = transmissionService;
            _logger = logger;
        }

        public IEnumerable<string> SupportedCommands => _supportedCommands;

        public async void HandleCommand(Message message, string command, IReadOnlyCollection<string> arguments)
        {
            var torrents = await _transmissionService.GetTorrentsList();

            if (torrents == null || torrents.Length == 0)
            {
                BotClient.SendTextMessage(message.Chat, "Нет активных торрентов");
                return;
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

            BotClient.SendTextMessage(message.Chat, builder.ToString());
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

        private static string GetTorrentStatus(ITorrentInformation torrent)
        {
            return string.Empty;
        }
    }
}