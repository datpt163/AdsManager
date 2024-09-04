using System;
using System.Collections.Generic;

namespace FBAdsManager.Common.Database.Data;

public partial class Pm
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public Guid UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
