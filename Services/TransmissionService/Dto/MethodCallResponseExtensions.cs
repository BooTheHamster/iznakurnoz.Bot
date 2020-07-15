namespace iznakurnoz.Bot.Services.TransmissionService.Dto
{
    /// <summary>
    /// Методы расширения для <see cref="MethodCallResponse">.
    /// </summary>
    internal static class MethodCallResponseExtensions
    {
        private const string OkMessage = "Ok";
        private const string OkResult = "success";

        /// <summary>
        /// Возвращает строку с читабельным результатом выполнения запроса.
        /// </summary>
        /// <param name="response">Результат выполнения запроса.</param>
        /// <typeparam name="TArguments">Тип данных ответа.</typeparam>
        /// <returns>Cтроку читабельным результатом выполнения запроса.</returns>
        public static string GetOkOrResult<TArguments>(this MethodCallResponse<TArguments> response)
        {
            return response.Result.ToLower() == OkResult 
                ? OkMessage
                : $"Ответ сервера: {response.Result}";
        }
    }
}