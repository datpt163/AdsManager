namespace FBAdsManager.Module.Branches.Requests
{
    public class AddBranchRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid OrganizationId { get; set; }
    }
}
