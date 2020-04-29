using System;
using System.IO;
using Iznakurnoz.Bot.Interfaces;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace iznakurnoz.Bot.DocumentHandlers
{
    internal class TorrentDocumentHandler : IBotDocumentHandler
    {
        private readonly IBotTelegramClient _botClient;
        private readonly IConfigProvider _configProvider;
        private readonly ILogger<TorrentDocumentHandler> _logger;

        public TorrentDocumentHandler(
            ILogger<TorrentDocumentHandler> logger,
            IBotTelegramClient botClient,
            IConfigProvider configProvider)
        {
            _logger = logger;
            _botClient = botClient;
            _configProvider = configProvider;
        }
        public bool HandleDocument(Message message)
        {
            if (!IsDocumentSupported(message.Document))
            {
                return false;
            }

            DownloadAndCopyTorrentFile(message);
            return true;
        }

        async private void DownloadAndCopyTorrentFile(Message message)
        {
            // Загрузка файла из телеграма.
            var fileStream = await _botClient.GetFile(message.Document.FileId);

            if (fileStream == null)
            {
                return;
            }

            // Сохранение файла в каталог где демон transmission может их загрузить.
            var tempFilename = Path.Combine(
                _configProvider.CurrentConfig.TorrentServerSettings.WatchDirectoryPath, 
                message.Document.FileName);

            try
            {
                using (var stream = new FileStream(tempFilename, FileMode.Create))
                {
                    await fileStream.CopyToAsync(stream);
                    await stream.FlushAsync();
                    stream.Close();
                }
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Save file error");
                return;
            }

            _botClient.SendTextMessage(
                message.Chat,
                $"Торрент <code>{message.Document.FileName}</code> добавлен.");
        }

        private bool IsDocumentSupported(Document document)
        {
            if (!document.FileName.EndsWith(".torrent"))
            {
                return false;
            }

            if (!document.MimeType.Contains("application/x-bittorrent"))
            {
                return false;
            }

            return true;
        }
    }
}