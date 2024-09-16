using System;
using System.Collections.Generic;

namespace FBAdsManager.Models;

public partial class Campaign
{
    public string Id { get; set; } = null!;

    public string? AccountId { get; set; }

    public string? Name { get; set; }

    public ulong? BudgetRebalanceFlag { get; set; }

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

    public virtual ICollection<Adset> Adsets { get; set; } = new List<Adset>();
}
