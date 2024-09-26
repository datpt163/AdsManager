namespace FBAdsManager.Module.Users.Requests
{
    public class UpdateUserSystemRequest
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? OrganizationId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password {  get; set; } = string.Empty;
    }
}
