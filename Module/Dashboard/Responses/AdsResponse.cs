namespace FBAdsManager.Module.Dashboard.Responses
{
    public class AdsResponse
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
        public InsightResponse? Insight { get; set; }
    }

    public class InsightResponse
    {
        public string? Impressions { get; set; }
        public string? Clicks { get; set; }
        public string? Spend { get; set; }
        public string? Reach { get; set; }
        public string? Ctr { get; set; }
        public string? Cpm { get; set; }
        public string? Cpc { get; set; }
        public string? Frequency { get; set; }
        public string? OnsiteConversionTotalMessagingConnection { get; set; }
        public string? OnsiteConversionMessagingFirstReply { get; set; }
        public string? PostEngagement { get; set; }
        public string? PageEngagement { get; set; }
        public string? PhotoView { get; set; }
        public string? VideoPlay { get; set; }
        public string? VideoView { get; set; }
        public string? Video10sView { get; set; }
        public string? Video30sView { get; set; }
        public string? VideoCompleteView { get; set; }
        public string? OnsiteConversionMessagingConversationStarted7d { get; set; }
        public string? CostPerAction { get; set; } 
    }
}
