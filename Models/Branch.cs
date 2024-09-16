using System;
using System.Collections.Generic;

namespace FBAdsManager.Models;

public partial class Branch
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime UpdateDate { get; set; }

    public DateTime? DeleteDate { get; set; }

    public Guid? OrganizationId { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();

    public virtual Organization? Organization { get; set; }
}
