using System;

namespace iznakurnoz.Bot.Services.TransmissionService
{
    /// <summary>
    /// Ошибка службы для работы с торрент-сервером Transmission.
    /// </summary>
    internal class TransmissionServiceException : Exception
    {
        public TransmissionServiceException(string message)
            : base(message)
        {
        }
    }
}