﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FBAdsManager.Common.Database.Data;

public partial class Employee
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public DateTime UpdateDate { get; set; }

    public DateTime? DeleteDate { get; set; }

    public Guid? GroupId { get; set; }

    public virtual Group? Group { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<AdsAccount> AdsAccounts { get; set; } = new List<AdsAccount>();
}
