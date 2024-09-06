using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FBAdsManager.Common.Database.Data;

public partial class Group
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime UpdateDate { get; set; }

    public DateTime? DeleteDate { get; set; }

    public Guid? BranchId { get; set; }

    public virtual Branch? Branch { get; set; } = null!;
    [JsonIgnore]

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
