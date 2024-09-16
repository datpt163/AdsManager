namespace FBAdsManager.Module.DataFacebook.Responses
{
    public class BmFbResponse
    {
            public List<BmResponse> data { get; set; } = new List<BmResponse>();
    }

    public class BmResponse
    {
        public string account_id { get; set; } = string.Empty;
    }
}
