namespace FBAdsManager.Module.DataFacebook.Responses
{
    public class insightResponse
    {
        public string impressions { get; set; }
        public string clicks { get; set; }
        public string spend { get; set; }
        public string ctr { get; set; }
        public string cpm { get; set; }
        public string cpc { get; set; }
        public string cpp { get; set; }
        public string reach { get; set; }
        public string frequency { get; set; }
        public List<Action> actions { get; set; }
        public List<CostPerActionType> cost_per_action_type { get; set; }
        public string date_start { get; set; }
        public string date_stop { get; set; }
    }

    public class Action
    {
        public string action_type { get; set; }
        public string value { get; set; } = string.Empty;
    }

    public class CostPerActionType
    {
        public string action_type { get; set; }
        public string value { get; set; }
    }

    public class insightFbResponse
    {
        public List<insightResponse> data { get; set; }
    }
}
