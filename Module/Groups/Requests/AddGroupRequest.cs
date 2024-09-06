namespace FBAdsManager.Module.Groups.Requests
{
    public class AddGroupRequest
    {

        public string Name { get; set; } = null!;

        public string Description { get; set; } = string.Empty;

        public Guid BranchId { get; set; }
    }
}
