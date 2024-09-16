using System.Xml.Linq;

namespace FBAdsManager.Module.AdsAccount.Responses
{
    public class ListAdsAccountFbResponse
    {
        public List<AdsAccountFbResponse>? data { get; set; }
    }

    public class AdsAccountFbResponse
    {
        public string id { get; set; } = string.Empty;
        public string account_id { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public int account_status { get; set; }
        public string currency { get; set; } = string.Empty;
        public string spend_cap { get; set; } = string.Empty;
        public string amount_spent { get; set; } = string.Empty;
        public string balance { get; set; } = string.Empty;
        public string created_time { get; set; } = string.Empty;
        public string owner { get; set; } = string.Empty;
        public string timezone_name { get; set; } = string.Empty;
        public int disable_reason { get; set; }
        public FundingSourceDetails? funding_source_details { get; set; }
        public string min_campaign_group_spend_cap { get; set; } = string.Empty;
        public double min_daily_budget { get; set; }
        public int is_personal { get; set; }
        public Business? business { get; set; } 
    }

    public class Business
    {
        public string id { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
    }

    public class FundingSourceDetails
    {
        public string display_string { get; set; } = string.Empty;
        public int type { get; set; }
    }
}
