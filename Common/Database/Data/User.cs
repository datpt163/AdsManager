using System;
using System.Collections.Generic;

namespace FBAdsManager.Common.Database.Data;

public partial class User
{
    public Guid Id { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = string.Empty!;

    public Guid RoleId { get; set; }

    public virtual ICollection<Pm> Pms { get; set; } = new List<Pm>();

    public virtual Role Role { get; set; } = null!;
}
