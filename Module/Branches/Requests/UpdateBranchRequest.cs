namespace FBAdsManager.Module.Branches.Requests
{
    public class UpdateBranchRequest
    {
        public Guid Id { get; set; } 
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid OrganizationId { get; set; }
    }
}
