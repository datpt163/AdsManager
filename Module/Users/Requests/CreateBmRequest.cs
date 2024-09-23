namespace FBAdsManager.Module.Users.Requests
{
    public class CreateBmRequest
    {
        public string email { get; set; } = string.Empty;
        public Guid GroupId { get; set; }
        public string TokenTelegram { get; set; } = string.Empty;
        public string ChatId { get; set; } = string.Empty; 
        public List<string> BmsId { get; set; } = new List<string>();
    }
}
