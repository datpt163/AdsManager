using System;
using System.Collections.Generic;

namespace FBAdsManager.Models;

public partial class Ad
{
    public string Id { get; set; } = null!;

    public string? AdsetId { get; set; }

    public string? Name { get; set; }

    public string? ActionType { get; set; }

    public string? TrackingSpecs { get; set; }

    public string? Adcreatives { get; set; }

    public string? EffectiveStatus { get; set; }

    public string? Status { get; set; }

    public string? ConfiguredStatus { get; set; }

    public string? CreatedTime { get; set; }

    public string? StartTime { get; set; }

    public string? UpdatedTime { get; set; }

    public DateTime? UpdateDataTime { get; set; }

    public virtual Adset? Adset { get; set; }

    public virtual ICollection<Insight> Insights { get; set; } = new List<Insight>();
}
