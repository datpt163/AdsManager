﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FBAdsManager.Common.Database.Data;

public partial class Pm
{
    public string Id { get; set; } = null!;

    public Guid? UserId { get; set; }

    public virtual User? User { get; set; }
    [JsonIgnore]
    public virtual ICollection<AdsAccount> AdsAccounts { get; set; } = new List<AdsAccount>();
}
