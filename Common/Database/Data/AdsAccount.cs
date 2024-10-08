﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FBAdsManager.Common.Database.Data;

public partial class AdsAccount
{
    public string AccountId { get; set; } = null!;

    public Guid? EmployeeId { get; set; }

    public string? Name { get; set; }

    public int? AccountStatus { get; set; }

    public string? Currency { get; set; }

    public string? SpendCap { get; set; }

    public string? AmountSpent { get; set; }

    public string? Balance { get; set; }

    public string? CreatedTime { get; set; }

    public string? Owner { get; set; }

    public string? TimezoneName { get; set; }

    public int? DisableReason { get; set; }

    public string? InforCardBanking { get; set; }

    public int? TypeCardBanking { get; set; }

    public string? MinCampaignGroupSpendCap { get; set; }

    public double? MinDailyBudget { get; set; }
    public string? TypeAccount { get; set; }
    public string? SourceAccount { get; set; }
    public string? Cost { get; set; }
    public string? InformationLogin { get; set; }

    public int? IsPersonal { get; set; }
    public int? IsActive { get; set; }
    public bool IsDelete { get; set; }
    public DateTime? UpdateDataTime { get; set; }
    [JsonIgnore]
    public virtual ICollection<Campaign> Campaigns { get; set; } = new List<Campaign>();

    public virtual Employee? Employee { get; set; }
    public virtual ICollection<Pm> Pms { get; set; } = new List<Pm>();
}
