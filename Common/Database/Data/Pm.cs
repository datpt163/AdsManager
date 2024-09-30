using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FBAdsManager.Common.Database.Data;

public partial class Pm
{
    public string Id { get; set; } = null!;

    public Guid? UserId { get; set; }
    public string? TypeAccount { get; set; }
    public string? SourceAccount { get; set; }
    public float? Cost { get; set; }
    public string? InformationLogin { get; set; }
    public virtual User? User { get; set; }
    [JsonIgnore]
    public virtual ICollection<AdsAccount> AdsAccounts { get; set; } = new List<AdsAccount>();
}
