using System;
using System.Collections.Generic;

namespace FBAdsManager.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string? Password { get; set; }

    public Guid RoleId { get; set; }

    public ulong IsActive { get; set; }

    public Guid? GroupId { get; set; }

    public string? AccessTokenFb { get; set; }

    public virtual Group? Group { get; set; }

    public virtual ICollection<Pm> Pms { get; set; } = new List<Pm>();

    public virtual Role Role { get; set; } = null!;
}
