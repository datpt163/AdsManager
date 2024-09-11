namespace FBAdsManager.Module.AdsAccount.Requests
{
    public class AddAccountRequest
    {
        public string AccountID { get; set; } = string.Empty;
        public Guid EmployeeID { get; set; }
    }
}
