using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FBAdsManager.Common.Database.Data;

public partial class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;
    [JsonIgnore]
    public string? Password { get; set; } = string.Empty;
    [JsonIgnore]
    public bool IsActive { get; set; }
    [JsonIgnore]
    public string? AccessTokenFb { get; set; } = string.Empty;
    [JsonIgnore]
    public Guid RoleId { get; set; }
    [JsonIgnore]
    public Guid? GroupId { get; set; }
    [JsonIgnore]
    public virtual ICollection<Pm> Pms { get; set; } = new List<Pm>();
    public virtual Role Role { get; set; } = null!;
    public virtual Group? Group { get; set; }
}
