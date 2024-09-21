using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FBAdsManager.Common.Database.Data;

public partial class Adset
{
    public string Id { get; set; } = null!;

    public string? CampaignId { get; set; }

    public string? Name { get; set; }

    public double? LifetimeImps { get; set; }

    public string? Targeting { get; set; }

    public string? DailyBudget { get; set; }

    public string? BudgetRemaining { get; set; }

    public string? LifetimeBudget { get; set; }

    public string? EffectiveStatus { get; set; }

    public string? Status { get; set; }

    public string? ConfiguredStatus { get; set; }

    public string? PromoteObjectPageId { get; set; }

    public string? CreatedTime { get; set; }

    public string? StartTime { get; set; }

    public string? UpdatedTime { get; set; }

    public DateTime? UpdateDataTime { get; set; }
    [JsonIgnore]
    public virtual ICollection<Ads> Ads { get; set; } = new List<Ads>();

    public virtual Campaign? Campaign { get; set; }
}
