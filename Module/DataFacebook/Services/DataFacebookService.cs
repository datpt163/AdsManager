using FBAdsManager.Common.CallApi;
using FBAdsManager.Common.Database.Repository;
using FBAdsManager.Common.Jwt;
using FBAdsManager.Common.Response.ResponseService;
using FBAdsManager.Module.AdsAccount.Responses;
using FBAdsManager.Module.DataFacebook.Responses;
using System.Text.Json;
using FBAdsManager.Common.Database.Data;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
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
            var adsAccounts = new List<FBAdsManager.Common.Database.Data.AdsAccount>();

            if (user.Group != null)
            {
                foreach (var g in user.Group.Employees)
                    foreach (var adsacc in g.AdsAccounts)
                        adsAccounts.Add(adsacc);
            }

            var listCheck = new List<string>();

            foreach (var pm in user.Pms)
            {
                (int statusCode22, BmFbResponse? DataBm) = await _callApiService.GetDataAsync<BmFbResponse>("https://graph.facebook.com/v20.0/" + pm.Id + "/owned_ad_accounts?fields=id,account_id,name&access_token=" + user.AccessTokenFb);
                if (statusCode22 == 401)
                    return new ResponseService("access token fb expired", null, 401);

                if (statusCode22 == 200 && DataBm != null && DataBm.data != null)
                    foreach (var l in DataBm.data)
                        listCheck.Add(l.account_id);

                (int statusCode46, BmFbResponse? DataBm2) = await _callApiService.GetDataAsync<BmFbResponse>("https://graph.facebook.com/v20.0/" + pm.Id + "/client_ad_accounts?fields=id,account_id,name&access_token=" + user.AccessTokenFb);

                if (statusCode46 == 200 && DataBm2 != null && DataBm2.data != null)
                    foreach (var l in DataBm2.data)
                        listCheck.Add(l.account_id);
            }


            foreach (var adsacc in adsAccounts)
            {
                (int statusCode4, AdsAccountFbResponse? Data) = await _callApiService.GetDataAsync<AdsAccountFbResponse>("https://graph.facebook.com/v20.0/" + "act_" + adsacc.AccountId + "?fields=id,account_id,name,account_status,currency,spend_cap,amount_spent,balance,business,created_time,owner,timezone_id,timezone_name,disable_reason,funding_source,funding_source_details,min_campaign_group_spend_cap,min_daily_budget,partner,business_city,business_country_code,business_name,business_state,business_street,business_street2,business_zip,capabilities,is_personal,line_numbers&access_token=" + user.AccessTokenFb);
                if (statusCode4 != 405 && Data != null && listCheck.Any(x => x == adsacc.AccountId))
                {
                    adsacc.Name = Data.name;
                    adsacc.AccountStatus = Data.account_status;
                    adsacc.Currency = Data.currency;
                    adsacc.SpendCap = Data.spend_cap;
                    adsacc.AmountSpent = Data.amount_spent;
                    adsacc.Balance = Data.balance;
                    adsacc.CreatedTime = Data.created_time;
                    adsacc.Owner = Data.owner;
                    adsacc.TimezoneName = Data.timezone_name;
                    adsacc.DisableReason = Data.disable_reason;
                    adsacc.InforCardBanking = Data.funding_source_details != null ? Data.funding_source_details.display_string : "";
                    adsacc.TypeCardBanking = Data.funding_source_details != null ? Data.funding_source_details.type : -1;
                    adsacc.MinCampaignGroupSpendCap = Data.min_campaign_group_spend_cap;
                    adsacc.MinDailyBudget = Data.min_daily_budget;
                    adsacc.IsPersonal = Data.is_personal;
                    adsacc.UpdateDataTime = DateTime.Now;
                    adsacc.IsActive = 1;
                }
            }

            _unitOfWork.AdsAccounts.UpdateRange(adsAccounts);
            await _unitOfWork.SaveChangesAsync();
            foreach (var acc in adsAccounts)
            {
                (int statuscode, ListCampainFbResponse? listcampaign) = await _callApiService.GetDataAsync<ListCampainFbResponse>("https://graph.facebook.com/v20.0/" + "act_" + acc.AccountId + "/campaigns?fields=id,name, execution_options,frequency_cap_reset_period,multi_optimization_goal_weight,is_skadnetwork_attribution,frequency_control_specs,is_using_l3_revenue_model,daily_spend_cap,lifetime_spend_cap,attribution_spec,budget_rebalance_flag,buying_type,daily_budget,lifetime_budget,created_time,start_time,stop_time,effective_status,configured_status,status,bid_strategy,account_id,adlabels,bid_amount,boosted_object_id,budget_remaining,can_create_brand_lift_study,can_use_spend_cap,issues_info,last_budget_toggling_time,objective,pacing_type,promoted_object,recommendations,smart_promotion_type,source_campaign,source_campaign_id,special_ad_categories,special_ad_category,special_ad_category_country,spend_cap,topline_id,updated_time&access_token=" + user.AccessTokenFb);
                if (statuscode == 405)
                    continue;
                if (statuscode == 401)
                    return new ResponseService("access token fb expired", null, 401);
                if (listcampaign == null || listcampaign.data == null)
                    continue;

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
            }

            await _unitOfWork.SaveChangesAsync();

            foreach (var acc in adsAccounts)
            {
                (int statusCode5, Root? ListAdsets) = await _callApiService.GetDataAsync<Root>("https://graph.facebook.com/v20.0/" + "act_" + acc.AccountId + "/adsets?fields=id,amount_spent,compaign_id,cpm,lifetime_imps,spend,name,targeting,use_new_app_click,time_based_ad_rotation,frequency_cap,delivery_estimate,effective_status,daily_min_spend_target,campaign_spec,bid_constraints,attribution_spec,adset_schedule,start_time,account_id,bid_amount,bid_strategy,budget_remaining,budget_rebalance_flag,buying_type,configured_status,created_time,daily_budget,lifetime_budget,is_skadnetwork_attribution,last_budget_toggling_time,objective,pacing_type,promoted_object,recommendations,smart_promotion_type,source_campaign,source_campaign_id,special_ad_categories,special_ad_category,special_ad_category_country,spend_cap,status,updated_time,adlabels,campaign,campaign_id,conversion_domain,creative_sequence,daily_spend_cap,is_dynamic_creative,is_recommendations_disabled&access_token=" + user.AccessTokenFb);
                if (statusCode5 == 405)
                    continue;
                if (ListAdsets == null || ListAdsets.data == null)
                    continue;

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
            }
            await _unitOfWork.SaveChangesAsync();

            foreach (var acc in adsAccounts)
            {
                (int statusCode10, RootObject? ListAds) = await _callApiService.GetDataAsync<RootObject>("https://graph.facebook.com/v20.0/" + "act_" + acc.AccountId + "/ads?fields=id,name,source_ad,last_evaluated_time,recommendations,issues_info,inline_link_og_object,inline_link_url,adset_id,campaign_id,configured_status,frequency_cap,effective_status,status,frequency_control_specs,conversion_specs,engagement_audience,bid_amount,bid_info,spend_cap,adcreatives{body,call_to_action_type,configurations,creative_id,creative_type,primary_text,link,link_url,description,headline,image_url,video_url},adlabels,ad_review_feedback,ad_format_type,ad_rotation,ad_delivery_status,ad_optimization_goal,ad_type,adset,campaign,creative,creative_status,creative_type,creative_rotation,creative_summary,custom_event_type,tracking_specs,ad_group_id,created_time,updated_time&access_token=" + user.AccessTokenFb);
                if (statusCode10 == 405)
                    continue;
                if (ListAds == null || ListAds.data == null)
                    continue;

                var ListAdsAdd = new List<Common.Database.Data.Ads>();
                var ListInsight = new List<Common.Database.Data.Insight>();
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

                        (int statusCode60, insightFbResponse? ListInsightData) = await _callApiService.GetDataAsync<insightFbResponse>("https://graph.facebook.com/v20.0/" + data.id + "/insights?fields=impressions,clicks,spend,ctr,cpm,cpc,cpp,reach,frequency,actions,cost_per_action_type,cost_per_conversion&access_token=" + user.AccessTokenFb);
                        if (statusCode60 == 200 && ListInsightData != null && ListInsightData.data != null)
                        {
                            var insightFb = ListInsightData.data.FirstOrDefault();
                            if (insightFb != null)
                            {
                                var insight = new Insight()
                                {
                                    Impressions = insightFb.impressions,
                                    Clicks = insightFb.clicks,
                                    Spend = insightFb.spend,
                                    Reach = insightFb.reach,
                                    Ctr = insightFb.ctr,
                                    Cpm = insightFb.cpm,
                                    Cpc = insightFb.cpc,
                                    Cpp = insightFb.cpp,
                                    Frequency = insightFb.frequency,
                                    Actions = JsonSerializer.Serialize(insightFb.actions),
                                    DateAt = DateTime.Now,
                                    UpdateDataTime = DateTime.Now,
                                    AdsId = data.id,
                                };
                                _unitOfWork.Insights.Add(insight);
                            }
                        }
                    }
                    _unitOfWork.Adses.AddRange(ListAdsAdd);
                }
            }
            await _unitOfWork.SaveChangesAsync();

            return new ResponseService("", null);
        }
    }
}
