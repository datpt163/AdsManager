using System;
using System.Collections.Generic;

namespace FBAdsManager.Models;

public partial class Organization
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime UpdateDate { get; set; }

    public DateTime? DeleteDate { get; set; }

    public virtual ICollection<Branch> Branches { get; set; } = new List<Branch>();
}
