namespace FBAdsManager.Module.DataFacebook.Responses
{
    public class RootObject
{
    public List<AdData> data { get; set; }
    public Paging paging { get; set; }
}

public class AdData
{
    public string id { get; set; }
    public string name { get; set; }
    public string adset_id { get; set; }
    public string campaign_id { get; set; }
    public string configured_status { get; set; }
    public string effective_status { get; set; }
    public string status { get; set; }
    public List<ConversionSpec> conversion_specs { get; set; }
    public bool engagement_audience { get; set; }
    public AdCreatives adcreatives { get; set; }
    public Adset adset { get; set; }
    public Campaign2 campaign { get; set; }
    public Creative creative { get; set; }
    public List<TrackingSpec> tracking_specs { get; set; }
    public List<IssuesInfo> issues_info { get; set; }
    public string created_time { get; set; }
    public string updated_time { get; set; }
}

public class ConversionSpec
{
    public List<string>? action_type { get; set; }
    public List<string>? conversion_id { get; set; }
}

public class AdCreatives
{
    public List<AdCreative> data { get; set; }
}

public class AdCreative
{
    public string? body { get; set; }
    public string? call_to_action_type { get; set; }
    public string? id { get; set; }
}

public class Adset
{
    public string id { get; set; }
}

public class Campaign2
{
    public string id { get; set; }
}

public class Creative
{
    public string id { get; set; }
}

public class TrackingSpec
{
    public List<string>? action_type { get; set; }
    public List<string>? page { get; set; }
    public List<string>? post { get; set; }
    public List<string>? conversion_id { get; set; }
    public List<string>? post_wall { get; set; }
}

public class IssuesInfo
{
    public string level { get; set; }
    public int error_code { get; set; }
    public string error_summary { get; set; }
    public string error_message { get; set; }
    public string error_type { get; set; }
}

public class Paging
{
    public Cursors cursors { get; set; }
}

public class Cursors
{
    public string before { get; set; }
    public string after { get; set; }
}

}
