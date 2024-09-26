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
using System.Security.AccessControl;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Intrinsics.Arm;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using FBAdsManager.Common.Helper;
namespace FBAdsManager.Module.DataFacebook.Services
{
    public class DataFacebookService : IDataFacebookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly CallApiService _callApiService;
        private readonly TelegramHelper _telegramHelper;

        public DataFacebookService(IUnitOfWork unitOfWork, IJwtService jwtService, CallApiService callApiService, TelegramHelper telegramHelper)
        {
            _telegramHelper = telegramHelper;
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _callApiService = callApiService;
        }
        public async Task<ResponseService> CheckFacebookTokenExpire(string token)
        {
            var user = await _jwtService.VerifyTokenAsync(token);
            var pm = user.Pms.FirstOrDefault();

            if (pm == null)
                return new ResponseService("Some thing wrong", null);
            (int statusCode22, BmFbResponse? DataBm) = await _callApiService.GetDataAsync<BmFbResponse>("https://graph.facebook.com/v20.0/" + pm.Id + "/owned_ad_accounts?fields=id,account_id,name&access_token=" + user.AccessTokenFb);
            if (statusCode22 == 401)
            {
                await _telegramHelper.SendMessage(user.TokenTelegram ?? "", user.ChatId ?? "", "Access Token expired");
                return new ResponseService("access token fb expired", null, 401);
            }
            return new ResponseService("", null);
        }

        public async Task<ResponseService> CrawlData(string token, DateTime? since, DateTime? until)
        {
            try
            {
                var user = await _jwtService.VerifyTokenAsync(token);
                var adsAccountIds = user.Pms
                                   .SelectMany(p => p.AdsAccounts)
                                   .Select(a => a.AccountId)
                                   .ToList(); // Lấy danh sách AccountId trước

                var adsAccounts = _unitOfWork.AdsAccounts
                    .Find(x => adsAccountIds.Contains(x.AccountId))
                    .Include(c => c.Campaigns)
                    .ThenInclude(c => c.Adsets)
                    .ThenInclude(c => c.Ads)
                    .ThenInclude(c => c.Insights)
                    .ToList();
                var listCheck = new List<string>();
                foreach (var pm in user.Pms)
                {
                    (int statusCode22, BmFbResponse? DataBm) = await _callApiService.GetDataAsync<BmFbResponse>("https://graph.facebook.com/v20.0/" + pm.Id + "/owned_ad_accounts?fields=id,account_id,name&access_token=" + user.AccessTokenFb);
                    if (statusCode22 == 401)
                    {
                        await _telegramHelper.SendMessage(user.TokenTelegram ?? "", user.ChatId ?? "", "Access Token expired");
                        return new ResponseService("access token fb expired", null, 401);
                    }

                    if (statusCode22 == 200 && DataBm != null && DataBm.data != null)
                        foreach (var l in DataBm.data)
                            listCheck.Add(l.account_id);

                    (int statusCode46, BmFbResponse? DataBm2) = await _callApiService.GetDataAsync<BmFbResponse>("https://graph.facebook.com/v20.0/" + pm.Id + "/client_ad_accounts?fields=id,account_id,name&access_token=" + user.AccessTokenFb);

                    if (statusCode46 == 200 && DataBm2 != null && DataBm2.data != null)
                        foreach (var l in DataBm2.data)
                            listCheck.Add(l.account_id);
                }

                adsAccounts = adsAccounts.Where(x => listCheck.Any(y => y == x.AccountId)).ToList();

                foreach (var adsacc in adsAccounts)
                {
                    (int statusCode4, AdsAccountFbResponse? Data) = await _callApiService.GetDataAsync<AdsAccountFbResponse>("https://graph.facebook.com/v20.0/" + "act_" + adsacc.AccountId + "?fields=id,account_id,name,account_status,currency,spend_cap,amount_spent,balance,business,created_time,owner,timezone_id,timezone_name,disable_reason,funding_source,funding_source_details,min_campaign_group_spend_cap,min_daily_budget,partner,business_city,business_country_code,business_name,business_state,business_street,business_street2,business_zip,capabilities,is_personal,line_numbers&access_token=" + user.AccessTokenFb);
                    if (statusCode4 != 405 && Data != null)
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
                foreach (var acc in adsAccounts)
                {
                    (int statuscode, ListCampainFbResponse? listcampaign) = await _callApiService.GetDataAsync<ListCampainFbResponse>("https://graph.facebook.com/v20.0/" + "act_" + acc.AccountId + "/campaigns?fields=id,name, execution_options,frequency_cap_reset_period,multi_optimization_goal_weight,is_skadnetwork_attribution,frequency_control_specs,is_using_l3_revenue_model,daily_spend_cap,lifetime_spend_cap,attribution_spec,budget_rebalance_flag,buying_type,daily_budget,lifetime_budget,created_time,start_time,stop_time,effective_status,configured_status,status,bid_strategy,account_id,adlabels,bid_amount,boosted_object_id,budget_remaining,can_create_brand_lift_study,can_use_spend_cap,issues_info,last_budget_toggling_time,objective,pacing_type,promoted_object,recommendations,smart_promotion_type,source_campaign,source_campaign_id,special_ad_categories,special_ad_category,special_ad_category_country,spend_cap,topline_id,updated_time&access_token=" + user.AccessTokenFb + "&limit=100");
                    if (statuscode == 405)
                        continue;
                    if (statuscode == 401)
                    {
                        await _telegramHelper.SendMessage(user.TokenTelegram ?? "", user.ChatId ?? "", "Access Token expired");
                        return new ResponseService("access token fb expired", null, 401);
                    }
                    if (listcampaign == null || listcampaign.data == null)
                        continue;

                    foreach (var data in listcampaign.data)
                    {
                        var temp = acc.Campaigns.Where(x => x.Id == data.id).FirstOrDefault();
                        if (temp != null)
                        {
                            temp.AccountId = data.account_id;
                            temp.Name = data.name;
                            temp.BudgetRebalanceFlag = data.budget_rebalance_flag;
                            temp.BuyingType = data.buying_type;
                            temp.CreatedTime = data.created_time;
                            temp.StartTime = data.start_time;
                            temp.EffectiveStatus = data.effective_status;
                            temp.ConfiguredStatus = data.configured_status;
                            temp.Status = data.status;
                            temp.DailyBudget = data.daily_budget;
                            temp.LifetimeBudget = data.lifetime_budget;
                            temp.BudgetRemaining = data.budget_remaining;
                            temp.SpecialAdCategory = JsonSerializer.Serialize(data.special_ad_categories);
                            temp.SpecialAdCategoryCountry = JsonSerializer.Serialize(data.special_ad_category_country);
                            temp.UpdatedTime = data.updated_time;
                            temp.Objective = data.objective;
                            temp.UpdateDataTime = DateTime.Now;
                        }
                        else
                        {

                            var campaign = new Common.Database.Data.Campaign()
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
                            };
                            acc.Campaigns.Add(campaign);
                        }
                    }
                }
                foreach (var acc in adsAccounts)
                {
                    string url = "https://graph.facebook.com/v20.0/" + "act_" + acc.AccountId + "/adsets?fields=id,amount_spent,compaign_id,cpm,lifetime_imps,spend,name,targeting,use_new_app_click,time_based_ad_rotation,frequency_cap,delivery_estimate,effective_status,daily_min_spend_target,campaign_spec,bid_constraints,attribution_spec,adset_schedule,start_time,account_id,bid_amount,bid_strategy,budget_remaining,budget_rebalance_flag,buying_type,configured_status,created_time,daily_budget,lifetime_budget,is_skadnetwork_attribution,last_budget_toggling_time,objective,pacing_type,promoted_object,recommendations,smart_promotion_type,source_campaign,source_campaign_id,special_ad_categories,special_ad_category,special_ad_category_country,spend_cap,status,updated_time,adlabels,campaign,campaign_id,conversion_domain,creative_sequence,daily_spend_cap,is_dynamic_creative,is_recommendations_disabled&access_token=" + user.AccessTokenFb + "&limit=100";
                    (int statusCode5, Root? ListAdsets) = await _callApiService.GetDataAsync<Root>(url);
                    if (statusCode5 == 401)
                    {
                        await _telegramHelper.SendMessage(user.TokenTelegram ?? "", user.ChatId ?? "", "Access Token expired");
                        return new ResponseService("access token fb expired", null, 401);
                    }
                    if (statusCode5 == 405)
                        continue;
                    if (ListAdsets == null || ListAdsets.data == null)
                        continue;

                    var ListAdsetsAdd = new List<Common.Database.Data.Adset>();

                    foreach (var data in ListAdsets.data)
                    {
                        var temp = acc.Campaigns
                                .SelectMany(campaign => campaign.Adsets)
                                .FirstOrDefault(adset => adset.Id == data.id);

                        if (temp != null)
                        {
                            temp.Id = data.id;
                            temp.CampaignId = data.campaign_id;
                            temp.Name = data.name;
                            temp.LifetimeImps = data.lifetime_imps;
                            temp.Targeting = JsonSerializer.Serialize(data.targeting);
                            temp.DailyBudget = data.daily_budget;
                            temp.BudgetRemaining = data.budget_remaining;
                            temp.LifetimeBudget = data.lifetime_budget;
                            temp.EffectiveStatus = data.effective_status;
                            temp.Status = data.status;
                            temp.ConfiguredStatus = data.configured_status;
                            temp.PromoteObjectPageId = data.promoted_object == null ? "" : data.promoted_object.page_id;
                            temp.CreatedTime = data.created_time;
                            temp.StartTime = data.start_time;
                            temp.UpdatedTime = data.updated_time;
                            temp.UpdateDataTime = DateTime.Now;
                        }
                        else
                        {
                            var adset = new Common.Database.Data.Adset()
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
                            };
                            var check = acc.Campaigns.FirstOrDefault(c => c.Id == data.campaign_id);
                            if (check != null)
                            {
                                check.Adsets.Add(adset);
                            }
                            else
                            {
                                var test = 0;
                            }
                        }
                    }
                }

                foreach (var acc in adsAccounts)
                {
                    (int statusCode10, RootObject? ListAds) = await _callApiService.GetDataAsync<RootObject>("https://graph.facebook.com/v20.0/" + "act_" + acc.AccountId + "/ads?fields=id,name,source_ad,last_evaluated_time,recommendations,issues_info,inline_link_og_object,inline_link_url,adset_id,campaign_id,configured_status,frequency_cap,effective_status,status,frequency_control_specs,conversion_specs,engagement_audience,bid_amount,bid_info,spend_cap,adcreatives{body,call_to_action_type,configurations,creative_id,creative_type,primary_text,link,link_url,description,headline,image_url,video_url},adlabels,ad_review_feedback,ad_format_type,ad_rotation,ad_delivery_status,ad_optimization_goal,ad_type,adset,campaign,creative,creative_status,creative_type,creative_rotation,creative_summary,custom_event_type,tracking_specs,ad_group_id,created_time,updated_time&access_token=" + user.AccessTokenFb + "&limit=100");
                    if (statusCode10 == 401)
                    {
                        await _telegramHelper.SendMessage(user.TokenTelegram ?? "", user.ChatId ?? "", "Access Token expired");
                        return new ResponseService("access token fb expired", null, 401);
                    }
                    if (statusCode10 == 405)
                        continue;
                    if (ListAds == null || ListAds.data == null)
                        continue;

                    var ListAdsAdd = new List<Common.Database.Data.Ads>();
                    var ListInsight = new List<Common.Database.Data.Insight>();
                    foreach (var data in ListAds.data)
                    {
                        var temp = acc.Campaigns.SelectMany(x => x.Adsets).SelectMany(x => x.Ads).Where(x => x.Id == data.id).FirstOrDefault();
                        if (temp != null)
                        {
                            temp.Id = data.id;
                            temp.AdsetId = data.adset_id;
                            temp.Name = data.name;
                            temp.ActionType = JsonSerializer.Serialize(data.conversion_specs);
                            temp.TrackingSpecs = JsonSerializer.Serialize(data.tracking_specs);
                            temp.Adcreatives = JsonSerializer.Serialize(data.adcreatives);
                            temp.EffectiveStatus = data.effective_status;
                            temp.Status = data.status;
                            temp.ConfiguredStatus = data.configured_status;
                            temp.CreatedTime = data.created_time;
                            temp.StartTime = "";
                            temp.UpdatedTime = data.updated_time;
                            temp.UpdateDataTime = DateTime.Now;
                        }
                        else
                        {
                            var y = data;
                            var x = data.adset_id.ToString();

                            var ads = new Common.Database.Data.Ads()
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
                            };
                            var check = acc.Campaigns.SelectMany(x => x.Adsets).FirstOrDefault(c => c.Id.Trim().Equals(data.adset_id.Trim()));
                            if (check != null)
                            {
                                check.Ads.Add(ads);
                            }
                            else
                            {
                                var test = 0;
                            }
                        }

                        if (since.HasValue && until.HasValue)
                        {
                            for (DateTime currentDate = since.Value; currentDate <= until.Value; currentDate = currentDate.AddDays(1))
                            {
                                (int statusCode60, insightFbResponse? ListInsightData) = await _callApiService.GetDataAsync<insightFbResponse>("https://graph.facebook.com/v20.0/" + data.id + "/insights?fields=impressions,clicks,spend,ctr,cpm,cpc,cpp,reach,frequency,actions,cost_per_action_type,cost_per_conversion&access_token=" + user.AccessTokenFb + "&time_range[since]=" + currentDate.ToString("yyyy-MM-dd") + "&time_range[until]=" + currentDate.ToString("yyyy-MM-dd"));
                                if (statusCode60 == 401)
                                {
                                    await _telegramHelper.SendMessage(user.TokenTelegram ?? "", user.ChatId ?? "", "Access Token expired");
                                    return new ResponseService("access token fb expired", null, 401);
                                }

                                if (statusCode60 == 200 && ListInsightData != null && ListInsightData.data != null)
                                {
                                    var insightFb = ListInsightData.data.FirstOrDefault();
                                    if (insightFb != null)
                                    {
                                        var insightCheck = acc.Campaigns.SelectMany(x => x.Adsets).SelectMany(x => x.Ads).SelectMany(x => x.Insights).Where(x => x.AdsId == data.id && x.DateAt.HasValue && x.DateAt.Value.Date == currentDate.Date).FirstOrDefault();
                                        if (insightCheck != null)
                                        {
                                            insightCheck.Impressions = insightFb.impressions;
                                            insightCheck.Clicks = insightFb.clicks;
                                            insightCheck.Spend = insightFb.spend;
                                            insightCheck.Reach = insightFb.reach;
                                            insightCheck.Ctr = insightFb.ctr;
                                            insightCheck.Cpm = insightFb.cpm;
                                            insightCheck.Cpc = insightFb.cpc;
                                            insightCheck.Cpp = insightFb.cpp;
                                            insightCheck.Frequency = insightFb.frequency;
                                            insightCheck.Actions = JsonSerializer.Serialize(insightFb.actions);
                                            insightCheck.UpdateDataTime = DateTime.Now;
                                            insightCheck.AdsId = data.id;
                                            insightCheck.CostPerAction = JsonSerializer.Serialize(insightFb.cost_per_action_type);
                                        }
                                        else
                                        {
                                            var insight = new Common.Database.Data.Insight()
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
                                                DateAt = currentDate.AddHours(12),
                                                UpdateDataTime = DateTime.Now,
                                                CostPerAction = JsonSerializer.Serialize(insightFb.cost_per_action_type),
                                                AdsId = data.id,
                                            };
                                            acc.Campaigns.SelectMany(x => x.Adsets).SelectMany(x => x.Ads).FirstOrDefault(c => c.Id == data.id)?.Insights.Add(insight);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            (int statusCode60, insightFbResponse? ListInsightData) = await _callApiService.GetDataAsync<insightFbResponse>("https://graph.facebook.com/v20.0/" + data.id + "/insights?fields=impressions,clicks,spend,ctr,cpm,cpc,cpp,reach,frequency,actions,cost_per_action_type,cost_per_conversion&access_token=" + user.AccessTokenFb + "&time_range[since]=" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + "&time_range[until]=" + DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
                            if (statusCode60 == 401)
                            {
                                await _telegramHelper.SendMessage(user.TokenTelegram ?? "", user.ChatId ?? "", "Access Token expired");
                                return new ResponseService("access token fb expired", null, 401);
                            }
                            if (statusCode60 == 200 && ListInsightData != null && ListInsightData.data != null)
                            {
                                var insightFb = ListInsightData.data.FirstOrDefault();
                                if (insightFb != null)
                                {
                                    var insightCheck = acc.Campaigns.SelectMany(x => x.Adsets).SelectMany(x => x.Ads).SelectMany(x => x.Insights).Where(x => x.AdsId == data.id && x.DateAt.HasValue && x.DateAt.Value.Date == DateTime.Now.AddDays(-1).Date).FirstOrDefault();
                                    if (insightCheck != null)
                                    {
                                        insightCheck.Impressions = insightFb.impressions;
                                        insightCheck.Clicks = insightFb.clicks;
                                        insightCheck.Spend = insightFb.spend;
                                        insightCheck.Reach = insightFb.reach;
                                        insightCheck.Ctr = insightFb.ctr;
                                        insightCheck.Cpm = insightFb.cpm;
                                        insightCheck.Cpc = insightFb.cpc;
                                        insightCheck.Cpp = insightFb.cpp;
                                        insightCheck.Frequency = insightFb.frequency;
                                        insightCheck.Actions = JsonSerializer.Serialize(insightFb.actions);
                                        insightCheck.UpdateDataTime = DateTime.Now;
                                        insightCheck.AdsId = data.id;
                                        insightCheck.CostPerAction = JsonSerializer.Serialize(insightFb.cost_per_action_type);
                                    }
                                    else
                                    {
                                        float? costPerResult = null;
                                        if (insightFb.actions != null && insightFb.actions.FirstOrDefault() != null && insightFb.spend != null && !string.IsNullOrEmpty(insightFb.actions.FirstOrDefault().value))
                                        {
                                            costPerResult = float.Parse(insightFb.spend) / float.Parse(insightFb.actions.FirstOrDefault().value);
                                        }

                                        var insight = new Common.Database.Data.Insight()
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
                                            DateAt = DateTime.Now.AddDays(-1).AddHours(12),
                                            UpdateDataTime = DateTime.Now,
                                            CostPerAction = JsonSerializer.Serialize(insightFb.cost_per_action_type),
                                            AdsId = data.id,
                                        };
                                        acc.Campaigns.SelectMany(x => x.Adsets).SelectMany(x => x.Ads).FirstOrDefault(c => c.Id == data.id)?.Insights.Add(insight);
                                    }
                                }
                            }
                        }
                    }
                }

                _unitOfWork.AdsAccounts.UpdateRange(adsAccounts);

                await _unitOfWork.SaveChangesAsync();

                return new ResponseService("", null);
            }
            catch (Exception e)
            {
                return new ResponseService(e.Message, null);
            }
        }
    }
}
