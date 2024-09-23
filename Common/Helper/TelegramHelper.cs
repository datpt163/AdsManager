using System.Net;

namespace FBAdsManager.Common.Helper
{
    public class TelegramHelper
    {
        private static readonly HttpClient client = new HttpClient();
        public async Task<bool> SendMessage(string tokenTelegram, string chatId, string message)
        {
            string url = $"https://api.telegram.org/bot{tokenTelegram}/sendMessage?chat_id={chatId}&text={Uri.EscapeDataString(message)}";
            var response = await client.GetAsync(url);
            if (response.StatusCode == HttpStatusCode.OK)
                return true;
            return false;
        }
    }
}
