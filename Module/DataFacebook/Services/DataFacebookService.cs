using FBAdsManager.Common.CallApi;
using FBAdsManager.Common.Database.Repository;
using FBAdsManager.Common.Jwt;
using FBAdsManager.Common.Response.ResponseService;
using FBAdsManager.Module.AdsAccount.Responses;
using FBAdsManager.Module.DataFacebook.Responses;
using System.Text.Json;

namespace FBAdsManager.Module.DataFacebook.Services
{
    public class DataFacebookService : IDataFacebookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly CallApiService _callApiService;

        public DataFacebookService(IUnitOfWork unitOfWork, IJwtService jwtService, CallApiService callApiService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _callApiService = callApiService;
        }
        public async Task<ResponseService> CrawlData(string token)
        {
            var user = await _jwtService.VerifyTokenAsync(token);
            (int statuscode, ListCampainFbResponse? listcampaign) = await _callApiService.GetDataAsync<ListCampainFbResponse>("https://graph.facebook.com/v20.0/act_3260791477508708/campaigns?fields=id,name, execution_options,frequency_cap_reset_period,multi_optimization_goal_weight,is_skadnetwork_attribution,frequency_control_specs,is_using_l3_revenue_model,daily_spend_cap,lifetime_spend_cap,attribution_spec,budget_rebalance_flag,buying_type,daily_budget,lifetime_budget,created_time,start_time,stop_time,effective_status,configured_status,status,bid_strategy,account_id,adlabels,bid_amount,boosted_object_id,budget_remaining,can_create_brand_lift_study,can_use_spend_cap,issues_info,last_budget_toggling_time,objective,pacing_type,promoted_object,recommendations,smart_promotion_type,source_campaign,source_campaign_id,special_ad_categories,special_ad_category,special_ad_category_country,spend_cap,topline_id,updated_time&access_token=" + user.AccessTokenFb);
            if (statuscode == 405)
                return new ResponseService("tài khoản quảng cáo không thuộc tài khoản pm này", null, 404);
            if (statuscode == 401)
                return new ResponseService("access token fb expired", null, 401);
            if (listcampaign == null || listcampaign.data == null)
                return new ResponseService("some thing wrong", null, 401);

            var campaignsadd = new List<Common.Database.Data.Campaign>();
            
            foreach (var data in listcampaign.data)
            {
                bool exists = _unitOfWork.AdsAccounts.GetQuery().Any(a => a.AccountId.Equals(data.account_id));

                if (exists)
                {
                    var temp = _unitOfWork.Campaigns.FindOne(x => x.Id == data.id);
                    if (temp != null)
                        _unitOfWork.Campaigns.Remove(temp);

                    campaignsadd.Add(new Common.Database.Data.Campaign()
                    {
                        Id = data.id,
                        AccountId = data.account_id,
                        Name = data.name,
                        BudgetRebalanceFlag = data.budget_rebalance_flag,
                        BuyingType = data.buying_type,
                        CreatedTime = data.created_time,
                        StartTime = data.start_time,
                        EffectiveStatus = data.effective_status,
                        ConfiguredStatus = data.configured_status,
                        Status = data.status,
                        DailyBudget = data.daily_budget,
                        LifetimeBudget = data.lifetime_budget,
                        BudgetRemaining = data.budget_remaining,
                        SpecialAdCategory = JsonSerializer.Serialize(data.special_ad_categories),
                        SpecialAdCategoryCountry = JsonSerializer.Serialize(data.special_ad_category_country),
                        UpdatedTime = data.updated_time,
                        Objective = data.objective,
                        UpdateDataTime = DateTime.Now
                    });
                }
            }

            _unitOfWork.Campaigns.AddRange(campaignsadd);

            (int statusCode, Root? ListAdsets) = await _callApiService.GetDataAsync<Root>("https://graph.facebook.com/v20.0/act_3260791477508708/adsets?fields=id,amount_spent,compaign_id,cpm,lifetime_imps,spend,name,targeting,use_new_app_click,time_based_ad_rotation,frequency_cap,delivery_estimate,effective_status,daily_min_spend_target,campaign_spec,bid_constraints,attribution_spec,adset_schedule,start_time,account_id,bid_amount,bid_strategy,budget_remaining,budget_rebalance_flag,buying_type,configured_status,created_time,daily_budget,lifetime_budget,is_skadnetwork_attribution,last_budget_toggling_time,objective,pacing_type,promoted_object,recommendations,smart_promotion_type,source_campaign,source_campaign_id,special_ad_categories,special_ad_category,special_ad_category_country,spend_cap,status,updated_time,adlabels,campaign,campaign_id,conversion_domain,creative_sequence,daily_spend_cap,is_dynamic_creative,is_recommendations_disabled&access_token=" + user.AccessTokenFb);
            if (ListAdsets == null || ListAdsets.data == null)
                return new ResponseService("Some thing wrong", null, 401);

            var ListAdsetsAdd = new List<Common.Database.Data.Adset>();

            foreach (var data in ListAdsets.data)
            {
                bool exists = _unitOfWork.Campaigns.GetQuery().Any(a => a.Id.Equals(data.campaign_id));

                if (exists)
                {
                    var temp = _unitOfWork.Adsets.FindOne(x => x.Id == data.id);
                    if (temp != null)
                        _unitOfWork.Adsets.Remove(temp);

                    ListAdsetsAdd.Add(new Common.Database.Data.Adset()
                    {
                        Id = data.id,
                        CampaignId = data.campaign_id,
                        Name = data.name,
                        LifetimeImps = data.lifetime_imps,
                        Targeting = JsonSerializer.Serialize(data.targeting),
                        DailyBudget = data.daily_budget,
                        BudgetRemaining = data.budget_remaining,
                        LifetimeBudget = data.lifetime_budget,
                        EffectiveStatus = data.effective_status,
                        Status = data.status,
                        ConfiguredStatus = data.configured_status,
                        PromoteObjectPageId = data.promoted_object == null ? "" : data.promoted_object.page_id,
                        CreatedTime = data.created_time,
                        StartTime = data.start_time,
                        UpdatedTime = data.updated_time,
                        UpdateDataTime = DateTime.Now
                    });
                }
            }

            _unitOfWork.Adsets.AddRange(ListAdsetsAdd);

            (int statusCode2, RootObject? ListAds) = await _callApiService.GetDataAsync<RootObject>("https://graph.facebook.com/v20.0/act_3260791477508708/ads?fields=id,name,source_ad,last_evaluated_time,recommendations,issues_info,inline_link_og_object,inline_link_url,adset_id,campaign_id,configured_status,frequency_cap,effective_status,status,frequency_control_specs,conversion_specs,engagement_audience,bid_amount,bid_info,spend_cap,adcreatives{body,call_to_action_type,configurations,creative_id,creative_type,primary_text,link,link_url,description,headline,image_url,video_url},adlabels,ad_review_feedback,ad_format_type,ad_rotation,ad_delivery_status,ad_optimization_goal,ad_type,adset,campaign,creative,creative_status,creative_type,creative_rotation,creative_summary,custom_event_type,tracking_specs,ad_group_id,created_time,updated_time&access_token=" + user.AccessTokenFb);
            if (ListAds == null || ListAds.data == null)
                return new ResponseService("Some thing wrong", null, 401);

            var ListAdsAdd = new List<Common.Database.Data.Ads>();
            foreach (var data in ListAds.data)
            {
                bool exists = _unitOfWork.Adsets.GetQuery().Any(a => a.Id.Equals(data.adset_id));
                if (exists)
                {
                    var temp = _unitOfWork.Adses.FindOne(x => x.Id == data.id);
                    if (temp != null)
                        _unitOfWork.Adses.Remove(temp);

                    ListAdsAdd.Add(new Common.Database.Data.Ads()
                    {
                        Id = data.id,
                        AdsetId = data.adset_id,
                        Name = data.name,
                        ActionType = JsonSerializer.Serialize(data.conversion_specs),
                        TrackingSpecs = JsonSerializer.Serialize(data.tracking_specs),
                        Adcreatives = JsonSerializer.Serialize(data.adcreatives),
                        EffectiveStatus = data.effective_status,
                        Status = data.status,
                        ConfiguredStatus = data.configured_status,
                        CreatedTime = data.created_time,
                        StartTime = "",
                        UpdatedTime = data.updated_time,
                        UpdateDataTime = DateTime.Now
                    });
                    break;
                }
            }
            _unitOfWork.Adses.AddRange(ListAdsAdd);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseService("", null);
        }
    }
}
