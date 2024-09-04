namespace FBAdsManager.Module.Organizations.Requests
{
    public class UpdateOrganizationRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = string.Empty!;
    }
}
