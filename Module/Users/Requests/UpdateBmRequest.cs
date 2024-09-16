namespace FBAdsManager.Module.Users.Requests
{
    public class UpdateBmRequest
    {
        public Guid Id { get; set; }
        public string email { get; set; } = string.Empty;
        public Guid GroupId { get; set; }
        public List<string> BmsId { get; set; } = new List<string>();
    }
}
