namespace FBAdsManager.Module.DataFacebook.Responses
{
    public class Campaignfb
    {
        public string id { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public bool is_skadnetwork_attribution { get; set; }
        public bool budget_rebalance_flag { get; set; }
        public string buying_type { get; set; } = string.Empty;
        public string created_time { get; set; } = string.Empty;
        public string start_time { get; set; } = string.Empty;
        public string stop_time { get; set; } = string.Empty;
        public string effective_status { get; set; } = string.Empty;
        public string configured_status { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public string account_id { get; set; } = string.Empty;
        public string boosted_object_id { get; set; } = string.Empty;
        public string budget_remaining { get; set; } = string.Empty;
        public bool can_create_brand_lift_study { get; set; }
        public bool can_use_spend_cap { get; set; }
        public string objective { get; set; } = string.Empty;
        public string smart_promotion_type { get; set; } = string.Empty;
        public string source_campaign_id { get; set; } = string.Empty;
        public string[]? special_ad_categories { get; set; } 
        public string special_ad_category { get; set; } = string.Empty;
        public string[]? special_ad_category_country { get; set; }
        public string topline_id { get; set; } = string.Empty;
        public string updated_time { get; set; } = string.Empty;
        public string bid_strategy { get; set; } = string.Empty;
        public string[]? pacing_type { get; set; } 
        public string daily_budget { get; set; } = string.Empty;
        public string lifetime_budget { get; set; } = string.Empty;
    }
    public class ListCampainFbResponse
    {
        public List<Campaignfb>? data { get; set; }
    }
}
