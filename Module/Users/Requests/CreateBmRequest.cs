namespace FBAdsManager.Module.Users.Requests
{
    public class CreateBmRequest
    {
        public string email { get; set; } = string.Empty;
        public Guid GroupId { get; set; }
        public string ChatId { get; set; } = string.Empty; 
        public List<BmInformation> Bms { get; set; } = new List<BmInformation>();
    }

    public class BmInformation
    {
        public string BmId { get; set; }
        public string? TypeAccount { get; set; }
        public string? SourceAccount { get; set; }
        public float? Cost { get; set; }
        public string? InformationLogin { get; set; }
    }
}
