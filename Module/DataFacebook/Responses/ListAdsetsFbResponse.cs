namespace FBAdsManager.Module.DataFacebook.Responses
{

    public class Root
    {
        public List<Data> data { get; set; }
    }

    public class Data
    {
        public string id { get; set; }
        public int lifetime_imps { get; set; }
        public string name { get; set; }
        public Targeting targeting { get; set; }
        public bool use_new_app_click { get; set; }
        public DeliveryEstimate delivery_estimate { get; set; }
        public string effective_status { get; set; }
        public List<AttributionSpec> attribution_spec { get; set; }
        public string start_time { get; set; }
        public string account_id { get; set; }
        public string bid_strategy { get; set; }
        public string budget_remaining { get; set; }
        public string configured_status { get; set; }
        public string created_time { get; set; }
        public string daily_budget { get; set; }
        public string lifetime_budget { get; set; }
        public List<string> pacing_type { get; set; }
        public PromotedObject? promoted_object { get; set; }
        public string status { get; set; }
        public string updated_time { get; set; }
        public Campaign campaign { get; set; }
        public string campaign_id { get; set; }
        public bool is_dynamic_creative { get; set; }
    }

    public class Targeting
    {
        public int age_max { get; set; }
        public int age_min { get; set; }
        public GeoLocations geo_locations { get; set; }
        public List<string> brand_safety_content_filter_levels { get; set; }
        public TargetingAutomation targeting_automation { get; set; }
        public List<string> publisher_platforms { get; set; }
        public List<string> facebook_positions { get; set; }
        public List<string> device_platforms { get; set; }
        public List<string> instagram_positions { get; set; }
    }

    public class GeoLocations
    {
        public List<string> countries { get; set; }
        public List<string> location_types { get; set; }
    }

    public class TargetingAutomation
    {
        public int advantage_audience { get; set; }
    }

    public class DeliveryEstimate
    {
        public List<DeliveryEstimateData> data { get; set; }
    }

    public class DeliveryEstimateData
    {
        public List<DailyOutcomesCurve> daily_outcomes_curve { get; set; }
        public int estimate_dau { get; set; }
        public int estimate_mau_lower_bound { get; set; }
        public int estimate_mau_upper_bound { get; set; }
        public bool estimate_ready { get; set; }
    }

    public class DailyOutcomesCurve
    {
        public double spend { get; set; }
        public double reach { get; set; }
        public double impressions { get; set; }
        public double actions { get; set; }
    }

    public class AttributionSpec
    {
        public string event_type { get; set; }
        public int window_days { get; set; }
    }

    public class PromotedObject
    {
        public string page_id { get; set; }
    }

    public class Campaign
    {
        public string id { get; set; }
    }


}
