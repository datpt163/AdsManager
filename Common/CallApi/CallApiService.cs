using System.Text.Json;

namespace FBAdsManager.Common.CallApi
{
    public class CallApiService
    {
        public async Task<(int statusCode, T? data)> GetDataAsync<T>(string url) where T : class
        {
            using HttpClient client = new HttpClient();
            HttpResponseMessage responseMessage = await client.GetAsync(url);
            string responseData = await responseMessage.Content.ReadAsStringAsync();

            if (responseMessage.StatusCode.ToString().Equals("OK"))
            {
                try
                {
                    T? data = JsonSerializer.Deserialize<T>(responseData);
                    return (200, data);
                }
                catch (JsonException)
                {
                    return (500, null);
                }
            }
            else
            {
                return (400, null);
            }
        }
    }
}
