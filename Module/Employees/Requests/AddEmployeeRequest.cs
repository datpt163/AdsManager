namespace FBAdsManager.Module.Employees.Requests
{
    public class AddEmployeeRequest
    {
        public string Name { get; set; } = null!;

        public string Description { get; set; } = string.Empty;
        public string Email {  get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public Guid GroupId { get; set; }
    }
}
