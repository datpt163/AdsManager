using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FBAdsManager.Common.Database.Data;

public partial class Organization
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime UpdateDate { get; set; }

    public DateTime? DeleteDate { get; set; }
    [JsonIgnore]
    public virtual ICollection<Branch> Branches { get; set; } = new List<Branch>();
    [JsonIgnore]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
