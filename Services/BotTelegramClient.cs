using System;
using System.IO;
using System.Threading.Tasks;
using Iznakurnoz.Bot.Configuration;
using Iznakurnoz.Bot.Interfaces;
using Microsoft.Extensions.Logging;
using MihaZupan;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace iznakurnoz.Bot.Services
{    
    /// <summary>
    /// Клиент для работы с телеграм-ботом.
    /// </summary>
    internal class BotTelegramClient : IBotTelegramClient, IBotTelegramClientControl
    {
        private readonly ILogger _logger;
        private BotConfig _config;
        private TelegramBotClient _client = null;

        public BotTelegramClient(ILogger<BotTelegramClient> logger)
        {
            _logger = logger;
        }

        public event EventHandler<MessageEventArgs> OnMessageReceived;

        async public Task<Stream> GetFile(string fileId)
        {
            if (_client == null)
            {
                return null;
            }

            try
            {
                var file = await _client?.GetFileAsync(fileId);

                if (file == null)
                {
                    return null;
                }

                var memoryStream = new MemoryStream();

                await _client?.DownloadFileAsync(file.FilePath, memoryStream);

                memoryStream.Seek(0, SeekOrigin.Begin);
                return memoryStream;
            }
            catch (Exception error)
            {
                _logger.LogError(error, "GetFile error");                
            }

            return null;
        }

        public void SendTextMessage(Chat chat, string message)
        {
            if (chat == null)
            {
                return;
            }

            _client?.SendTextMessageAsync(chat, message, ParseMode.Html);
        }

        public bool Start(BotConfig config)
        {
            try
            {
                Stop();

                _config = config;
                var proxy = new HttpToSocks5Proxy(
                    _config.ProxySettings.Address,
                    _config.ProxySettings.Port,
                    _config.ProxySettings.Username,
                    _config.ProxySettings.Password);
                _client = new TelegramBotClient(_config.AuthToken, proxy);

            }
            catch (Exception error)
            {
                _logger.LogError($"TelegramBotClient create error: {error.Message}");
                return false;
            }

            _client.OnMessage += BotOnMessageReceived;
            _client.OnMessageEdited += BotOnMessageReceived;
            _client.StartReceiving();
            
            return true;
        }

        public void Stop()
        {
            _client?.StopReceiving();
            _client = null;
        }

        private void BotOnMessageReceived(object sender, MessageEventArgs messageEvent)
        {
            var handler = OnMessageReceived;
            handler?.Invoke(this, messageEvent);
        }
    }
}