namespace FBAdsManager.Module.AdsAccount.Requests
{
    public class AddAccountRequest
    {
        public string AccountID { get; set; } = string.Empty;
        public Guid EmployeeID { get; set; }
        public string? TypeAccount { get; set; }
        public string? SourceAccount { get; set; }
        public string? Cost { get; set; }
        public string? InformationLogin { get; set; }
        public List<string> PmsId { get; set; } = new List<string>();
    }
}
