using System.Text.Json.Serialization;

namespace FBAdsManager.Module.Users.Requests
{
    public class AddUserRequest 
    {
        public string Email { get; set; } = null!;
        public string? Password { get; set; } = string.Empty!;
        public Guid RoleId { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? OrganizationId { get; set; }
    }
}
