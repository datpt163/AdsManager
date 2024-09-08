using FBAdsManager.Common.Database.Data;
using System.Text.Json.Serialization;

namespace FBAdsManager.Module.Users.Response
{
    public class UserDTO
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = null!;
        public virtual string? RoleName { get; set; } 
        public virtual string? GroupName { get; set; }
    }
}
