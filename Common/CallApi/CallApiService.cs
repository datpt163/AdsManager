using System.Text.Json;

namespace FBAdsManager.Common.CallApi
{
    public class CallApiService
    {
        public async Task<(int StatusCode, T? data)> GetDataAsync<T>(string url) where T : class
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


                    return (400, null);
                }
            }
            else
            {
                try
                {
                    var data = JsonSerializer.Deserialize<ErrorResponse>(responseData);
                    if(data != null &&  data.error.message.Contains("expired"))
                        return (401, null);
                    else if(data != null &&  data.error.message.Contains("Missing permissions"))
                        return (405, null);
                    return (400, null);
                }
                catch 
                {
                    return (400, null);
                }
            }
        }
    }

    public class ErrorResponse
    {
        public ErrorDetails error { get; set; } = null!;

        public class ErrorDetails
        {
            public string message { get; set; } = string.Empty;
            public string type { get; set; } = string.Empty;
            public int code { get; set; }
            public int errorSubcode { get; set; }
            public string fbtraceId { get; set; } = string.Empty;
        }
    }

}
