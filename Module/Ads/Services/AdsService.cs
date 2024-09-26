using FBAdsManager.Common.CallApi;
using FBAdsManager.Common.Database.Data;
using FBAdsManager.Common.Database.Repository;
using FBAdsManager.Common.Helper;
using FBAdsManager.Common.Paging;
using FBAdsManager.Common.Response.ResponseService;
using FBAdsManager.Module.Dashboard.Responses;
using FBAdsManager.Module.DataFacebook.Responses;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FBAdsManager.Module.Ads.Services
{
    public class AdsService : IAdsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly CallApiService _callApiService;
        public AdsService(IUnitOfWork unitOfWork, CallApiService callApiService)
        {
            _callApiService = callApiService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseService> GetListAsync(int? pageIndex, int? pageSize, string? adsetId, DateTime start, DateTime end)
        {
            if (pageIndex != null && pageSize != null && adsetId != null)
            {
                if (pageIndex.Value < 1 || pageSize.Value < 0)
                    return new ResponseService("PageIndex, PageSize must >= 0", null);

                    
                int skip = (pageIndex.Value - 1) * pageSize.Value;
                var adses = new List<AdsResponse>();
                var pagedOrganizationQuery = await _unitOfWork.Adses.Find(c => c.AdsetId != null && c.AdsetId.Equals(adsetId)).OrderBy(c => c.CreatedTime).Include(c => c.Insights).Include(c => c.Adset).ThenInclude(c => c.Campaign).ThenInclude(c => c.Account).ThenInclude(c => c.Pms).ThenInclude(c => c.User).Skip(skip).Take(pageSize.Value).ToListAsync();
                
                foreach(var l in pagedOrganizationQuery)
                {
                    var ads = new AdsResponse();
                    double impression = 0, clicks = 0, spend = 0, cpm = 0, ctr = 0, cpc = 0, 
                    onsiteConversionTotalMessagingConnection = 0, onsiteConversionMessagingFirstReply = 0,
                    postEngagement = 0, pageEngagement = 0, photoView = 0, videoPlay = 0, videoView = 0,
                    video10sView = 0, video30sView = 0, videoCompleteView = 0, onsiteConversionMessagingConversationStarted7d = 0;
                    string reach = "", frequency = "";

                    ads.Id = l.Id;
                    ads.AdsetId = l.AdsetId;
                    ads.Name = l.Name;
                    ads.ActionType = l.ActionType;
                    ads.TrackingSpecs = l.TrackingSpecs;
                    ads.Adcreatives = l.Adcreatives;
                    ads.EffectiveStatus = l.EffectiveStatus;
                    ads.Status = l.Status;
                    ads.ConfiguredStatus = l.ConfiguredStatus;
                    ads.CreatedTime = l.CreatedTime;
                    ads.StartTime = l.StartTime;
                    ads.UpdateDataTime = l.UpdateDataTime;

                    foreach (var i in l.Insights)
                    {
                        if (i.DateAt != null && i.DateAt.Value.Date >= start.Date && i.DateAt.Value.Date <= end.Date)
                        {
                            impression += double.Parse(i.Impressions ?? "0");
                            clicks += double.Parse(i.Clicks ?? "0");
                            spend += double.Parse(i.Spend ?? "0");
                            if ((!string.IsNullOrEmpty(i.Actions)) && !i.Actions.Equals("null"))
                            {
                                var action = JsonSerializer.Deserialize<List<FBAdsManager.Module.DataFacebook.Responses.Action>>(i.Actions);
                                if (action != null)
                                {
                                    foreach (var a in action)
                                    {
                                        if (a.action_type.Trim().Equals("onsite_conversion.total_messaging_connection"))
                                            onsiteConversionTotalMessagingConnection += double.Parse(a.value);
                                        if (a.action_type.Trim().Equals("onsite_conversion.messaging_first_reply"))
                                            onsiteConversionMessagingFirstReply += double.Parse(a.value);
                                        if (a.action_type.Trim().Equals("post_engagement"))
                                            postEngagement += double.Parse(a.value);
                                        if (a.action_type.Trim().Equals("page_engagement"))
                                            pageEngagement += double.Parse(a.value);
                                        if (a.action_type.Trim().Equals("photo_view"))
                                            photoView += double.Parse(a.value);
                                        if (a.action_type.Trim().Equals("video_play"))
                                            videoPlay += double.Parse(a.value);
                                        if (a.action_type.Trim().Equals("video_view"))
                                            videoView += double.Parse(a.value);
                                        if (a.action_type.Trim().Equals("video_10s_view"))
                                            video10sView += double.Parse(a.value);
                                        if (a.action_type.Trim().Equals("video_30s_view"))
                                            video30sView += double.Parse(a.value);
                                        if (a.action_type.Trim().Equals("video_complete_view"))
                                            videoCompleteView += double.Parse(a.value);
                                        if (a.action_type.Trim().Equals("onsite_conversion.messaging_conversation_started_7d"))
                                            onsiteConversionMessagingConversationStarted7d += double.Parse(a.value);
                                    }
                                }
                            }
                        }
                    }

                    if (impression != 0)
                    {
                        cpm = (spend / impression) * 1000;
                        ctr = (clicks / impression) * 100;
                    }
                    if (clicks != 0)
                        cpc = (spend / clicks);


                    var pms = l.Adset == null ? null : (l.Adset.Campaign == null ? null : (l.Adset.Campaign.Account == null ? null : l.Adset.Campaign.Account.Pms));
                    bool check = false;

                    if (pms != null && pms.Count() > 0)
                    {   
                        foreach(var pm in pms)
                        {
                            (int statusCode, insightFbResponse? ListInsightData) = await _callApiService.GetDataAsync<insightFbResponse>("https://graph.facebook.com/v20.0/" + l.Id + "/insights?fields=impressions,clicks,spend,ctr,cpm,cpc,cpp,reach,frequency,actions,cost_per_action_type,cost_per_conversion&access_token=" + (pm.User == null ? null : pm.User.AccessTokenFb) + "&time_range[since]=" + start.ToString("yyyy-MM-dd") + "&time_range[until]=" + end.ToString("yyyy-MM-dd"));
                            if (statusCode == 200)
                            {
                                reach = ListInsightData == null ? "0" : (ListInsightData.data.FirstOrDefault() == null ? "0" : ListInsightData.data.FirstOrDefault().reach);
                                frequency = ListInsightData == null ? "0" : (ListInsightData.data.FirstOrDefault() == null ? "0" : ListInsightData.data.FirstOrDefault().frequency);
                                check = true;
                                break;
                            }
                        }
                    }

                    if (!check)
                    {
                        reach = "dữ liệu bị lỗi";
                        frequency = "dữ liệu bị lỗi";
                    }

                    ads.Insight = new InsightResponse()
                    {
                        Impressions = impression + "",
                        Clicks = clicks + "",
                        Spend = spend + "",
                        Reach = reach + "",
                        Ctr = ctr + "",
                        Cpm = cpm + "",
                        Cpc = cpc + "",
                        Frequency = frequency + "",
                        OnsiteConversionTotalMessagingConnection = onsiteConversionTotalMessagingConnection + "",
                        OnsiteConversionMessagingFirstReply = onsiteConversionMessagingFirstReply + "",
                        PostEngagement = postEngagement + "",
                        PageEngagement = pageEngagement + "",
                        PhotoView = photoView + "",
                        VideoPlay = videoPlay + "",
                        VideoView = videoView + "",
                        Video10sView = video10sView + "",
                        Video30sView = video30sView + "",
                        VideoCompleteView = videoCompleteView + "",
                        OnsiteConversionMessagingConversationStarted7d = onsiteConversionMessagingConversationStarted7d + ""
                    };
                    if (onsiteConversionTotalMessagingConnection != 0)
                        ads.Insight.CostPerAction = spend / onsiteConversionTotalMessagingConnection + ""; 
                    adses.Add(ads);
                }

                var totalCount = _unitOfWork.Adses.Find(c => c.AdsetId != null && c.AdsetId.Equals(adsetId)).Count();
                return new ResponseService("", adses, new PagingResponse(totalCount, pageIndex.Value, pageSize.Value));
            }
            return new ResponseService("", null);
        }
    }
}
