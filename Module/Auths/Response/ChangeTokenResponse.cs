namespace FBAdsManager.Module.Auths.Response
{
    public class ChangeTokenResponse
    {
        public string access_token { get; set; } = string.Empty;
    }
    public class VerifyTokenFbResponse
    {
        public string email { get; set; } = string.Empty;
    }
}
