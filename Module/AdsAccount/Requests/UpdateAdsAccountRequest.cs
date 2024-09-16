namespace FBAdsManager.Module.AdsAccount.Requests
{
    public class UpdateAdsAccountRequest
    {
        public string AccountID { get; set; } = string.Empty;
        public Guid EmployeeID { get; set; }
    }
}
