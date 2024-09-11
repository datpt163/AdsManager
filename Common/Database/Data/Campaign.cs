using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FBAdsManager.Common.Database.Data;

public partial class Campaign
{
    public string Id { get; set; } = null!;

    public string? AccountId { get; set; }

    public string? Name { get; set; }

    public bool? BudgetRebalanceFlag { get; set; }

    public string? BuyingType { get; set; }

    public string? CreatedTime { get; set; }

    public string? StartTime { get; set; }

    public string? EffectiveStatus { get; set; }

    public string? ConfiguredStatus { get; set; }

    public string? Status { get; set; }

    public string? DailyBudget { get; set; }

    public string? LifetimeBudget { get; set; }

    public string? BudgetRemaining { get; set; }

    public string? SpecialAdCategory { get; set; }

    public string? SpecialAdCategoryCountry { get; set; }

    public string? UpdatedTime { get; set; }

    public string? Objective { get; set; }

    public DateTime? UpdateDataTime { get; set; }

    public virtual AdsAccount? Account { get; set; }
    [JsonIgnore]

    public virtual ICollection<Adset> Adsets { get; set; } = new List<Adset>();
}
