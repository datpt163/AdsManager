using FBAdsManager.Common.Database.Repository;
using FBAdsManager.Common.Paging;
using FBAdsManager.Common.Response.ResponseService;
using FBAdsManager.Module.Adsets.Responses;
using FBAdsManager.Module.Campaigns.Responses;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FBAdsManager.Module.Campaigns.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CampaignService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseService> GetListAsync(int? pageIndex, int? pageSize, string? adsAccountId, DateTime start, DateTime end)
        {
            if (pageIndex != null && pageSize != null && adsAccountId != null)
            {
                if (pageIndex.Value < 1 || pageSize.Value < 0)
                    return new ResponseService("PageIndex, PageSize must >= 0", null);

                int skip = (pageIndex.Value - 1) * pageSize.Value;
                var campaigns = new List<CampainResponse>();
                var pagedOrganizationQuery = _unitOfWork.Campaigns.Find(c => c.Account != null && c.AccountId != null && c.AccountId.Equals(adsAccountId)).Skip(skip).Take(pageSize.Value).Include(c => c.Adsets).ThenInclude(c => c.Ads).ThenInclude(c => c.Insights).Include(c => c.Account).ToList();


                foreach (var campaign in pagedOrganizationQuery)
                {
                    var campaignAdded = new CampainResponse();
                    double impression = 0, clicks = 0, spend = 0, reach = 0, cpm = 0, ctr = 0, cpc = 0, frequency = 0,
                    onsiteConversionTotalMessagingConnection = 0, onsiteConversionMessagingFirstReply = 0,
                    postEngagement = 0, pageEngagement = 0, photoView = 0, videoPlay = 0, videoView = 0,
                    video10sView = 0, video30sView = 0, videoCompleteView = 0, onsiteConversionMessagingConversationStarted7d = 0;

                    campaignAdded.Id = campaign.Id;

                    campaignAdded.AccountId = campaign.AccountId;

                    campaignAdded.Name = campaign.Name;

                    campaignAdded.BudgetRebalanceFlag = campaign.BudgetRebalanceFlag;

                    campaignAdded.BuyingType = campaign.BuyingType;

                    campaignAdded.CreatedTime = campaign.CreatedTime;

                    campaignAdded.StartTime = campaign.StartTime;

                    campaignAdded.EffectiveStatus = campaign.EffectiveStatus;

                    campaignAdded.ConfiguredStatus = campaign.ConfiguredStatus;

                    campaignAdded.Status = campaign.Status;

                    campaignAdded.DailyBudget = campaign.DailyBudget;

                    campaignAdded.LifetimeBudget = campaign.LifetimeBudget;

                    campaignAdded.BudgetRemaining = campaign.BudgetRemaining;

                    campaignAdded.SpecialAdCategory = campaign.SpecialAdCategory;

                    campaignAdded.SpecialAdCategoryCountry= campaign.SpecialAdCategoryCountry;

                    campaignAdded.UpdatedTime = campaign.UpdatedTime;

                    campaignAdded.Objective = campaign.Objective;

                    campaignAdded.UpdateDataTime =  campaign.UpdateDataTime;

                    foreach(var adset in campaign.Adsets)
                    {
                        foreach (var l in adset.Ads)
                        {
                            foreach (var i in l.Insights)
                            {
                                if (i.DateAt != null && i.DateAt.Value.Date >= start.Date && i.DateAt.Value.Date <= end.Date)
                                {
                                    impression += double.Parse(i.Impressions ?? "0");
                                    clicks += double.Parse(i.Clicks ?? "0");
                                    spend += double.Parse(i.Spend ?? "0");
                                    reach += double.Parse(i.Reach ?? "0");
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

                            if (reach != 0)
                                frequency = impression / reach;

                            campaignAdded.Insight = new Dashboard.Responses.InsightResponse()
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
                                campaignAdded.Insight.CostPerAction = spend / onsiteConversionTotalMessagingConnection + "";
                        }
                    }
                    campaigns.Add(campaignAdded);
                }

                var totalCount = _unitOfWork.Campaigns.Find(c => c.AccountId != null && c.AccountId.Equals(adsAccountId)).Count();
                return new ResponseService("", campaigns, new PagingResponse(totalCount, pageIndex.Value, pageSize.Value));
            }

            return new ResponseService("", null);
        }
    }
}
