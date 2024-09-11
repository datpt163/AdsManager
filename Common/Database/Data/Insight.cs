using System;
using System.Collections.Generic;

namespace FBAdsManager.Common.Database.Data;

public partial class Insight
{
    public Guid Id { get; set; }

    public string? Impressions { get; set; }

    public string? Clicks { get; set; }

    public string? Spend { get; set; }

    public string? Reach { get; set; }

    public string? Ctr { get; set; }

    public string? Cpm { get; set; }

    public string? Cpc { get; set; }

    public string? Cpp { get; set; }

    public string? Frequency { get; set; }

    public string? Actions { get; set; }

    public DateTime? DateAt { get; set; }

    public DateTime? UpdateDataTime { get; set; }

    public string? AdsId { get; set; }

    public virtual Ads? Ads { get; set; }
}
