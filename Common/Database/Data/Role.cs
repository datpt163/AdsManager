using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FBAdsManager.Common.Database.Data;

public partial class Role
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    [JsonIgnore]
    public DateTime createTime { get; set; } = DateTime.Now;
    [JsonIgnore]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
