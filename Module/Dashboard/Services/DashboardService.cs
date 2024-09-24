using FBAdsManager.Common.Database.Data;
using FBAdsManager.Common.Database.Repository;
using FBAdsManager.Common.Response.ResponseService;
using FBAdsManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FBAdsManager.Module.Dashboard.Services
{
    public class DashboardService : IDashBoardService
    {
        private readonly IUnitOfWork _unitOfWork;
        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ResponseService StatisticSpend(Guid? organizationId, Guid? branchId, Guid? groupId, DateTime start, DateTime end)
        {
            var x = new List<string>();
            var y = new List<double>();

            if (groupId.HasValue)
            {
                var employees = _unitOfWork.Employees.Find(x => x.DeleteDate == null && x.GroupId == groupId)
                   .Include(c => c.AdsAccounts).ThenInclude(c => c.Campaigns).ThenInclude(c => c.Adsets).ThenInclude(c => c.Ads).ThenInclude(c => c.Insights).ToList();

                foreach (var o in employees)
                {
                    x.Add(o.Name);
                    double totalSpend = 0;
                    var insight = new List<FBAdsManager.Common.Database.Data.Insight>();

                    foreach (var adAccount in o.AdsAccounts.Where(x => x.IsDelete == false).ToList())
                    {
                        var currency = adAccount.Currency ?? "";
                        foreach (var campaign in adAccount.Campaigns)
                        {
                            foreach (var adset in campaign.Adsets)
                            {
                                foreach (var ad in adset.Ads)
                                {
                                    foreach (var i in ad.Insights)
                                    {
                                        if (i.DateAt >= start && i.DateAt <= end)
                                        {
                                            double spend = 0;
                                            if (currency.Equals("VND"))
                                            {
                                                if (!string.IsNullOrEmpty(currency))
                                                    spend += float.Parse(i.Spend ?? "0");
                                            }
                                            else if (currency.Trim().Equals("USD"))
                                            {
                                                if (!string.IsNullOrEmpty(currency))
                                                    spend += float.Parse(i.Spend ?? "0") * 24670;
                                            }
                                            else if (currency.Trim().Equals("EUR"))
                                            {
                                                if (!string.IsNullOrEmpty(currency))
                                                    spend += double.Parse(i.Spend ?? "0") * 27447.72;
                                            }
                                            else if (currency.Trim().Equals("PHP"))
                                            {
                                                if (!string.IsNullOrEmpty(currency))
                                                    spend += double.Parse(i.Spend ?? "0") * 442.52;
                                            }
                                            else if (currency.Trim().Equals("THB"))
                                            {
                                                if (!string.IsNullOrEmpty(currency))
                                                    spend += double.Parse(i.Spend ?? "0") * 739.54;
                                            }
                                            else
                                                spend = 0;

                                            totalSpend += spend;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    y.Add(totalSpend);
                }
            }
            else if (branchId.HasValue)
            {
                var groups = _unitOfWork.Groups.Find(x => x.DeleteDate == null && x.BranchId == branchId)
                   .Include(c => c.Employees).ThenInclude(c => c.AdsAccounts).ThenInclude(c => c.Campaigns).ThenInclude(c => c.Adsets).ThenInclude(c => c.Ads).ThenInclude(c => c.Insights).ToList();

                foreach (var o in groups)
                {
                    x.Add(o.Name);
                    double totalSpend = 0;
                    var insight = new List<FBAdsManager.Common.Database.Data.Insight>();

                    foreach (var employee in o.Employees)
                    {
                        foreach (var adAccount in employee.AdsAccounts.Where(x => x.IsDelete == false).ToList())
                        {
                            var currency = adAccount.Currency ?? "";
                            foreach (var campaign in adAccount.Campaigns)
                            {
                                foreach (var adset in campaign.Adsets)
                                {
                                    foreach (var ad in adset.Ads)
                                    {
                                        foreach (var i in ad.Insights)
                                        {
                                            if (i.DateAt >= start && i.DateAt <= end)
                                            {
                                                double spend = 0;
                                                if (currency.Equals("VND"))
                                                {
                                                    if (!string.IsNullOrEmpty(currency))
                                                        spend += float.Parse(i.Spend ?? "0");
                                                }
                                                else if (currency.Trim().Equals("USD"))
                                                {
                                                    if (!string.IsNullOrEmpty(currency))
                                                        spend += float.Parse(i.Spend ?? "0") * 24670;
                                                }
                                                else if (currency.Trim().Equals("EUR"))
                                                {
                                                    if (!string.IsNullOrEmpty(currency))
                                                        spend += double.Parse(i.Spend ?? "0") * 27447.72;
                                                }
                                                else if (currency.Trim().Equals("PHP"))
                                                {
                                                    if (!string.IsNullOrEmpty(currency))
                                                        spend += double.Parse(i.Spend ?? "0") * 442.52;
                                                }
                                                else if (currency.Trim().Equals("THB"))
                                                {
                                                    if (!string.IsNullOrEmpty(currency))
                                                        spend += double.Parse(i.Spend ?? "0") * 739.54;
                                                }
                                                else
                                                    spend = 0;

                                                totalSpend += spend;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    y.Add(totalSpend);
                }
            }

            else if (organizationId.HasValue)
            {
                var branchs = _unitOfWork.Branchs.Find(x => x.DeleteDate == null && x.OrganizationId == organizationId).Include(c => c.Groups)
                    .ThenInclude(c => c.Employees).ThenInclude(c => c.AdsAccounts).ThenInclude(c => c.Campaigns).ThenInclude(c => c.Adsets).ThenInclude(c => c.Ads).ThenInclude(c => c.Insights).ToList();

                foreach (var o in branchs)
                {
                    x.Add(o.Name);
                    double totalSpend = 0;
                    var insight = new List<FBAdsManager.Common.Database.Data.Insight>();

                    foreach (var group in o.Groups)
                    {
                        foreach (var employee in group.Employees)
                        {
                            foreach (var adAccount in employee.AdsAccounts.Where(x => x.IsDelete == false).ToList())
                            {
                                var currency = adAccount.Currency ?? "";
                                foreach (var campaign in adAccount.Campaigns)
                                {
                                    foreach (var adset in campaign.Adsets)
                                    {
                                        foreach (var ad in adset.Ads)
                                        {
                                            foreach (var i in ad.Insights)
                                            {
                                                if (i.DateAt >= start && i.DateAt <= end)
                                                {
                                                    double spend = 0;
                                                    if (currency.Equals("VND"))
                                                    {
                                                        if (!string.IsNullOrEmpty(currency))
                                                            spend += float.Parse(i.Spend ?? "0");
                                                    }
                                                    else if (currency.Trim().Equals("USD"))
                                                    {
                                                        if (!string.IsNullOrEmpty(currency))
                                                            spend += float.Parse(i.Spend ?? "0") * 24670;
                                                    }
                                                    else if (currency.Trim().Equals("EUR"))
                                                    {
                                                        if (!string.IsNullOrEmpty(currency))
                                                            spend += double.Parse(i.Spend ?? "0") * 27447.72;
                                                    }
                                                    else if (currency.Trim().Equals("PHP"))
                                                    {
                                                        if (!string.IsNullOrEmpty(currency))
                                                            spend += double.Parse(i.Spend ?? "0") * 442.52;
                                                    }
                                                    else if (currency.Trim().Equals("THB"))
                                                    {
                                                        if (!string.IsNullOrEmpty(currency))
                                                            spend += double.Parse(i.Spend ?? "0") * 739.54;
                                                    }
                                                    else
                                                        spend = 0;

                                                    totalSpend += spend;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    y.Add(totalSpend);
                }
            }

            else if (organizationId == null && branchId == null && groupId == null)
            {
                var organizations = _unitOfWork.Organizations.Find(x => x.DeleteDate == null).Include(c => c.Branches).ThenInclude(c => c.Groups)
                    .ThenInclude(c => c.Employees).ThenInclude(c => c.AdsAccounts).ThenInclude(c => c.Campaigns).ThenInclude(c => c.Adsets).ThenInclude(c => c.Ads).ThenInclude(c => c.Insights).ToList();

                foreach (var o in organizations)
                {
                    x.Add(o.Name);
                    double totalSpend = 0;
                    var insight = new List<FBAdsManager.Common.Database.Data.Insight>();
                    foreach (var branch in o.Branches)
                    {
                        foreach (var group in branch.Groups)
                        {
                            foreach (var employee in group.Employees)
                            {
                                foreach (var adAccount in employee.AdsAccounts.Where(x => x.IsDelete == false).ToList())
                                {
                                    var currency = adAccount.Currency ?? "";
                                    foreach (var campaign in adAccount.Campaigns)
                                    {
                                        foreach (var adset in campaign.Adsets)
                                        {
                                            foreach (var ad in adset.Ads)
                                            {
                                                foreach (var i in ad.Insights)
                                                {
                                                    if (i.DateAt >= start && i.DateAt <= end)
                                                    {
                                                        double spend = 0;
                                                        if (currency.Equals("VND"))
                                                        {
                                                            if (!string.IsNullOrEmpty(currency))
                                                                spend += float.Parse(i.Spend ?? "0");
                                                        }
                                                        else if (currency.Trim().Equals("USD"))
                                                        {
                                                            if (!string.IsNullOrEmpty(currency))
                                                                spend += float.Parse(i.Spend ?? "0") * 24670;
                                                        }
                                                        else if (currency.Trim().Equals("EUR"))
                                                        {
                                                            if (!string.IsNullOrEmpty(currency))
                                                                spend += double.Parse(i.Spend ?? "0") * 27447.72;
                                                        }
                                                        else if (currency.Trim().Equals("PHP"))
                                                        {
                                                            if (!string.IsNullOrEmpty(currency))
                                                                spend += double.Parse(i.Spend ?? "0") * 442.52;
                                                        }
                                                        else if (currency.Trim().Equals("THB"))
                                                        {
                                                            if (!string.IsNullOrEmpty(currency))
                                                                spend += double.Parse(i.Spend ?? "0") * 739.54;
                                                        }
                                                        else
                                                            spend = 0;

                                                        totalSpend += spend;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    y.Add(totalSpend);
                }
            }
            return new ResponseService("", new { x = x, y = y });
        }

        public ResponseService StatisticCampaign(Guid? organizationId, Guid? branchId, Guid? groupId, DateTime start, DateTime end)
        {
            var x = new List<string>();
            var y = new List<double>();
            if (groupId.HasValue)
            {
                var employees = _unitOfWork.Employees.Find(x => x.DeleteDate == null && x.GroupId == groupId)
                   .Include(c => c.AdsAccounts).ThenInclude(c => c.Campaigns).ThenInclude(c => c.Adsets).ThenInclude(c => c.Ads).ThenInclude(c => c.Insights).ToList();

                foreach (var o in employees)
                {
                    x.Add(o.Name);
                    double total = 0;
                    var insight = new List<FBAdsManager.Common.Database.Data.Insight>();

                    foreach (var adAccount in o.AdsAccounts.Where(x => x.IsDelete == false).ToList())
                    {
                        total += adAccount.Campaigns.Where(c => DateTime.Parse(c.StartTime ?? "1555-08-08") >= start && DateTime.Parse(c.StartTime ?? "1555-08-08") <= end).Count();
                    }
                    y.Add(total);
                }
            }

            else if (branchId.HasValue)
            {
                var groups = _unitOfWork.Groups.Find(x => x.DeleteDate == null && x.BranchId == branchId)
                   .Include(c => c.Employees).ThenInclude(c => c.AdsAccounts).ThenInclude(c => c.Campaigns).ThenInclude(c => c.Adsets).ThenInclude(c => c.Ads).ThenInclude(c => c.Insights).ToList();

                foreach (var o in groups)
                {
                    x.Add(o.Name);
                    double total = 0;
                    var insight = new List<FBAdsManager.Common.Database.Data.Insight>();

                    foreach (var employee in o.Employees)
                    {
                        foreach (var adAccount in employee.AdsAccounts.Where(x => x.IsDelete == false).ToList())
                        {
                            total += adAccount.Campaigns.Where(c => DateTime.Parse(c.StartTime ?? "1555-08-08") >= start && DateTime.Parse(c.StartTime ?? "1555-08-08") <= end).Count();
                        }
                    }
                    y.Add(total);
                }
            }
            else if (organizationId.HasValue)
            {
                var branchs = _unitOfWork.Branchs.Find(x => x.DeleteDate == null && x.OrganizationId == organizationId).Include(c => c.Groups)
                    .ThenInclude(c => c.Employees).ThenInclude(c => c.AdsAccounts).ThenInclude(c => c.Campaigns).ThenInclude(c => c.Adsets).ThenInclude(c => c.Ads).ThenInclude(c => c.Insights).ToList();

                foreach (var o in branchs)
                {
                    x.Add(o.Name);
                    double total = 0;
                    var insight = new List<FBAdsManager.Common.Database.Data.Insight>();

                    foreach (var group in o.Groups)
                    {
                        foreach (var employee in group.Employees)
                        {
                            foreach (var adAccount in employee.AdsAccounts.Where(x => x.IsDelete == false).ToList())
                            {
                                total += adAccount.Campaigns.Where(c => DateTime.Parse(c.StartTime ?? "1555-08-08") >= start && DateTime.Parse(c.StartTime ?? "1555-08-08") <= end).Count();
                            }
                        }
                    }
                    y.Add(total);
                }
            }
            else if (organizationId == null && branchId == null && groupId == null)
            {
                var organizations = _unitOfWork.Organizations.Find(x => x.DeleteDate == null).Include(c => c.Branches).ThenInclude(c => c.Groups)
                    .ThenInclude(c => c.Employees).ThenInclude(c => c.AdsAccounts).ThenInclude(c => c.Campaigns).ToList();

                foreach (var o in organizations)
                {
                    x.Add(o.Name);
                    int total = 0;
                    var insight = new List<FBAdsManager.Common.Database.Data.Insight>();
                    foreach (var branch in o.Branches)
                    {
                        foreach (var group in branch.Groups)
                        {
                            foreach (var employee in group.Employees)
                            {
                                foreach (var adAccount in employee.AdsAccounts.Where(x => x.IsDelete == false).ToList())
                                {
                                    total += adAccount.Campaigns.Where(c => DateTime.Parse(c.StartTime ?? "1555-08-08") >= start && DateTime.Parse(c.StartTime ?? "1555-08-08") <= end).Count();
                                }
                            }
                        }
                    }
                    y.Add(total);
                }
            }



            return new ResponseService("", new { x = x, y = y });
        }

        public ResponseService StatisticResult(Guid? organizationId, Guid? branchId, Guid? groupId, DateTime start, DateTime end)
        {
            var x = new List<string>();
            var y = new List<double>();

            if (groupId.HasValue)
            {
                var employees = _unitOfWork.Employees.Find(x => x.DeleteDate == null && x.GroupId == groupId)
                   .Include(c => c.AdsAccounts).ThenInclude(c => c.Campaigns).ThenInclude(c => c.Adsets).ThenInclude(c => c.Ads).ThenInclude(c => c.Insights).ToList();

                foreach (var o in employees)
                {
                    x.Add(o.Name);
                    double totalSpend = 0;
                    var insight = new List<FBAdsManager.Common.Database.Data.Insight>();

                    foreach (var adAccount in o.AdsAccounts.Where(x => x.IsDelete == false).ToList())
                    {
                        var currency = adAccount.Currency ?? "";
                        foreach (var campaign in adAccount.Campaigns)
                        {
                            foreach (var adset in campaign.Adsets)
                            {
                                foreach (var ad in adset.Ads)
                                {
                                    foreach (var i in ad.Insights)
                                    {
                                        if (i.DateAt >= start && i.DateAt <= end)
                                        {
                                            if ((!string.IsNullOrEmpty(i.Actions)) && !i.Actions.Equals("null"))
                                            {
                                                var action = JsonSerializer.Deserialize<List<FBAdsManager.Module.DataFacebook.Responses.Action>>(i.Actions);
                                                if (action != null)
                                                {
                                                    foreach (var a in action)
                                                    {
                                                        if (a.action_type.Trim().Equals("onsite_conversion.total_messaging_connection"))
                                                            totalSpend += double.Parse(a.value);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    y.Add(totalSpend);
                }
            }
            else if (branchId.HasValue)
            {
                var groups = _unitOfWork.Groups.Find(x => x.DeleteDate == null && x.BranchId == branchId)
                   .Include(c => c.Employees).ThenInclude(c => c.AdsAccounts).ThenInclude(c => c.Campaigns).ThenInclude(c => c.Adsets).ThenInclude(c => c.Ads).ThenInclude(c => c.Insights).ToList();

                foreach (var o in groups)
                {
                    x.Add(o.Name);
                    double totalSpend = 0;
                    var insight = new List<FBAdsManager.Common.Database.Data.Insight>();

                    foreach (var employee in o.Employees)
                    {
                        foreach (var adAccount in employee.AdsAccounts.Where(x => x.IsDelete == false).ToList())
                        {
                            var currency = adAccount.Currency ?? "";
                            foreach (var campaign in adAccount.Campaigns)
                            {
                                foreach (var adset in campaign.Adsets)
                                {
                                    foreach (var ad in adset.Ads)
                                    {
                                        foreach (var i in ad.Insights)
                                        {
                                            if (i.DateAt >= start && i.DateAt <= end)
                                            {
                                                if ((!string.IsNullOrEmpty(i.Actions)) && !i.Actions.Equals("null"))
                                                {
                                                    var action = JsonSerializer.Deserialize<List<FBAdsManager.Module.DataFacebook.Responses.Action>>(i.Actions);
                                                    if (action != null)
                                                    {
                                                        foreach (var a in action)
                                                        {
                                                            if (a.action_type.Trim().Equals("onsite_conversion.total_messaging_connection"))
                                                                totalSpend += double.Parse(a.value);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    y.Add(totalSpend);
                }
            }

            else if (organizationId.HasValue)
            {
                var branchs = _unitOfWork.Branchs.Find(x => x.DeleteDate == null && x.OrganizationId == organizationId).Include(c => c.Groups)
                    .ThenInclude(c => c.Employees).ThenInclude(c => c.AdsAccounts).ThenInclude(c => c.Campaigns).ThenInclude(c => c.Adsets).ThenInclude(c => c.Ads).ThenInclude(c => c.Insights).ToList();

                foreach (var o in branchs)
                {
                    x.Add(o.Name);
                    double totalSpend = 0;
                    var insight = new List<FBAdsManager.Common.Database.Data.Insight>();

                    foreach (var group in o.Groups)
                    {
                        foreach (var employee in group.Employees)
                        {
                            foreach (var adAccount in employee.AdsAccounts.Where(x => x.IsDelete == false).ToList())
                            {
                                var currency = adAccount.Currency ?? "";
                                foreach (var campaign in adAccount.Campaigns)
                                {
                                    foreach (var adset in campaign.Adsets)
                                    {
                                        foreach (var ad in adset.Ads)
                                        {
                                            foreach (var i in ad.Insights)
                                            {
                                                if (i.DateAt >= start && i.DateAt <= end)
                                                {
                                                    if ((!string.IsNullOrEmpty(i.Actions)) && !i.Actions.Equals("null"))
                                                    {
                                                        var action = JsonSerializer.Deserialize<List<FBAdsManager.Module.DataFacebook.Responses.Action>>(i.Actions);
                                                        if (action != null)
                                                        {
                                                            foreach (var a in action)
                                                            {
                                                                if (a.action_type.Trim().Equals("onsite_conversion.total_messaging_connection"))
                                                                    totalSpend += double.Parse(a.value);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    y.Add(totalSpend);
                }
            }

            else if (organizationId == null && branchId == null && groupId == null)
            {
                var organizations = _unitOfWork.Organizations.Find(x => x.DeleteDate == null).Include(c => c.Branches).ThenInclude(c => c.Groups)
                    .ThenInclude(c => c.Employees).ThenInclude(c => c.AdsAccounts).ThenInclude(c => c.Campaigns).ThenInclude(c => c.Adsets).ThenInclude(c => c.Ads).ThenInclude(c => c.Insights).ToList();

                foreach (var o in organizations)
                {
                    x.Add(o.Name);
                    double totalSpend = 0;
                    var insight = new List<FBAdsManager.Common.Database.Data.Insight>();
                    foreach (var branch in o.Branches)
                    {
                        foreach (var group in branch.Groups)
                        {
                            foreach (var employee in group.Employees)
                            {
                                foreach (var adAccount in employee.AdsAccounts.Where(x => x.IsDelete == false).ToList())
                                {
                                    var currency = adAccount.Currency ?? "";
                                    foreach (var campaign in adAccount.Campaigns)
                                    {
                                        foreach (var adset in campaign.Adsets)
                                        {
                                            foreach (var ad in adset.Ads)
                                            {
                                                foreach (var i in ad.Insights)
                                                {
                                                    if (i.DateAt >= start && i.DateAt <= end)
                                                    {
                                                        if ((!string.IsNullOrEmpty(i.Actions)) && !i.Actions.Equals("null"))
                                                        {
                                                            var action = JsonSerializer.Deserialize<List<FBAdsManager.Module.DataFacebook.Responses.Action>>(i.Actions);
                                                            if (action != null)
                                                            {
                                                                foreach (var a in action)
                                                                {
                                                                    if (a.action_type.Trim().Equals("onsite_conversion.total_messaging_connection"))
                                                                        totalSpend += double.Parse(a.value);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    y.Add(totalSpend);
                }
            }



            return new ResponseService("", new { x = x, y = y });
        }


        public ResponseService StatisticCostPerResult(Guid? organizationId, Guid? branchId, Guid? groupId, DateTime start, DateTime end)
        {
            var x = new List<string>();
            var y = new List<double>();

            if (groupId.HasValue)
            {
                var employees = _unitOfWork.Employees.Find(x => x.DeleteDate == null && x.GroupId == groupId)
                   .Include(c => c.AdsAccounts).ThenInclude(c => c.Campaigns).ThenInclude(c => c.Adsets).ThenInclude(c => c.Ads).ThenInclude(c => c.Insights).ToList();

                foreach (var o in employees)
                {
                    x.Add(o.Name);
                    double totalResult = 0;
                    double totalSpend = 0;
                    var insight = new List<FBAdsManager.Common.Database.Data.Insight>();

                    foreach (var adAccount in o.AdsAccounts.Where(x => x.IsDelete == false).ToList())
                    {
                        var currency = adAccount.Currency ?? "";
                        foreach (var campaign in adAccount.Campaigns)
                        {
                            foreach (var adset in campaign.Adsets)
                            {
                                foreach (var ad in adset.Ads)
                                {
                                    foreach (var i in ad.Insights)
                                    {
                                        if (i.DateAt >= start && i.DateAt <= end)
                                        {
                                            if ((!string.IsNullOrEmpty(i.Actions)) && !i.Actions.Equals("null"))
                                            {
                                                var action = JsonSerializer.Deserialize<List<FBAdsManager.Module.DataFacebook.Responses.Action>>(i.Actions);
                                                if (action != null)
                                                {
                                                    foreach (var a in action)
                                                    {
                                                        if (a.action_type.Trim().Equals("onsite_conversion.total_messaging_connection"))
                                                            totalResult += double.Parse(a.value);
                                                        break;

                                                    }
                                                }
                                            }

                                            double spend = 0;
                                            if (currency.Equals("VND"))
                                            {
                                                if (!string.IsNullOrEmpty(currency))
                                                    spend += float.Parse(i.Spend ?? "0");
                                            }
                                            else if (currency.Trim().Equals("USD"))
                                            {
                                                if (!string.IsNullOrEmpty(currency))
                                                    spend += float.Parse(i.Spend ?? "0") * 24670;
                                            }
                                            else if (currency.Trim().Equals("EUR"))
                                            {
                                                if (!string.IsNullOrEmpty(currency))
                                                    spend += double.Parse(i.Spend ?? "0") * 27447.72;
                                            }
                                            else if (currency.Trim().Equals("PHP"))
                                            {
                                                if (!string.IsNullOrEmpty(currency))
                                                    spend += double.Parse(i.Spend ?? "0") * 442.52;
                                            }
                                            else if (currency.Trim().Equals("THB"))
                                            {
                                                if (!string.IsNullOrEmpty(currency))
                                                    spend += double.Parse(i.Spend ?? "0") * 739.54;
                                            }
                                            else
                                                spend = 0;

                                            totalSpend += spend;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (totalResult == 0)
                        totalResult = 1;
                    y.Add(totalSpend / totalResult);
                }
            }
            else if (branchId.HasValue)
            {
                var groups = _unitOfWork.Groups.Find(x => x.DeleteDate == null && x.BranchId == branchId)
                   .Include(c => c.Employees).ThenInclude(c => c.AdsAccounts).ThenInclude(c => c.Campaigns).ThenInclude(c => c.Adsets).ThenInclude(c => c.Ads).ThenInclude(c => c.Insights).ToList();

                foreach (var o in groups)
                {
                    x.Add(o.Name);
                    double totalResult = 0;
                    double totalSpend = 0;
                    var insight = new List<FBAdsManager.Common.Database.Data.Insight>();

                    foreach (var employee in o.Employees)
                    {
                        foreach (var adAccount in employee.AdsAccounts.Where(x => x.IsDelete == false).ToList())
                        {
                            var currency = adAccount.Currency ?? "";
                            foreach (var campaign in adAccount.Campaigns)
                            {
                                foreach (var adset in campaign.Adsets)
                                {
                                    foreach (var ad in adset.Ads)
                                    {
                                        foreach (var i in ad.Insights)
                                        {
                                            if (i.DateAt >= start && i.DateAt <= end)
                                            {
                                                if ((!string.IsNullOrEmpty(i.Actions)) && !i.Actions.Equals("null"))
                                                {
                                                    var action = JsonSerializer.Deserialize<List<FBAdsManager.Module.DataFacebook.Responses.Action>>(i.Actions);
                                                    if (action != null)
                                                    {
                                                        foreach (var a in action)
                                                        {
                                                            if (a.action_type.Trim().Equals("onsite_conversion.total_messaging_connection"))
                                                                totalResult += double.Parse(a.value);
                                                            break;

                                                        }
                                                    }
                                                }

                                                double spend = 0;
                                                if (currency.Equals("VND"))
                                                {
                                                    if (!string.IsNullOrEmpty(currency))
                                                        spend += float.Parse(i.Spend ?? "0");
                                                }
                                                else if (currency.Trim().Equals("USD"))
                                                {
                                                    if (!string.IsNullOrEmpty(currency))
                                                        spend += float.Parse(i.Spend ?? "0") * 24670;
                                                }
                                                else if (currency.Trim().Equals("EUR"))
                                                {
                                                    if (!string.IsNullOrEmpty(currency))
                                                        spend += double.Parse(i.Spend ?? "0") * 27447.72;
                                                }
                                                else if (currency.Trim().Equals("PHP"))
                                                {
                                                    if (!string.IsNullOrEmpty(currency))
                                                        spend += double.Parse(i.Spend ?? "0") * 442.52;
                                                }
                                                else if (currency.Trim().Equals("THB"))
                                                {
                                                    if (!string.IsNullOrEmpty(currency))
                                                        spend += double.Parse(i.Spend ?? "0") * 739.54;
                                                }
                                                else
                                                    spend = 0;

                                                totalSpend += spend;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (totalResult == 0)
                        totalResult = 1;
                    y.Add(totalSpend / totalResult);
                }
            }

            else if (organizationId.HasValue)
            {
                var branchs = _unitOfWork.Branchs.Find(x => x.DeleteDate == null && x.OrganizationId == organizationId).Include(c => c.Groups)
                    .ThenInclude(c => c.Employees).ThenInclude(c => c.AdsAccounts).ThenInclude(c => c.Campaigns).ThenInclude(c => c.Adsets).ThenInclude(c => c.Ads).ThenInclude(c => c.Insights).ToList();

                foreach (var o in branchs)
                {
                    x.Add(o.Name);
                    double totalResult = 0;
                    double totalSpend = 0;
                    var insight = new List<FBAdsManager.Common.Database.Data.Insight>();

                    foreach (var group in o.Groups)
                    {
                        foreach (var employee in group.Employees)
                        {
                            foreach (var adAccount in employee.AdsAccounts.Where(x => x.IsDelete == false).ToList())
                            {
                                var currency = adAccount.Currency ?? "";
                                foreach (var campaign in adAccount.Campaigns)
                                {
                                    foreach (var adset in campaign.Adsets)
                                    {
                                        foreach (var ad in adset.Ads)
                                        {
                                            foreach (var i in ad.Insights)
                                            {
                                                if (i.DateAt >= start && i.DateAt <= end)
                                                {
                                                    if ((!string.IsNullOrEmpty(i.Actions)) && !i.Actions.Equals("null"))
                                                    {
                                                        var action = JsonSerializer.Deserialize<List<FBAdsManager.Module.DataFacebook.Responses.Action>>(i.Actions);
                                                        if (action != null)
                                                        {
                                                            foreach (var a in action)
                                                            {
                                                                if (a.action_type.Trim().Equals("onsite_conversion.total_messaging_connection"))
                                                                    totalResult += double.Parse(a.value);
                                                                break;

                                                            }
                                                        }
                                                    }

                                                    double spend = 0;
                                                    if (currency.Equals("VND"))
                                                    {
                                                        if (!string.IsNullOrEmpty(currency))
                                                            spend += float.Parse(i.Spend ?? "0");
                                                    }
                                                    else if (currency.Trim().Equals("USD"))
                                                    {
                                                        if (!string.IsNullOrEmpty(currency))
                                                            spend += float.Parse(i.Spend ?? "0") * 24670;
                                                    }
                                                    else if (currency.Trim().Equals("EUR"))
                                                    {
                                                        if (!string.IsNullOrEmpty(currency))
                                                            spend += double.Parse(i.Spend ?? "0") * 27447.72;
                                                    }
                                                    else if (currency.Trim().Equals("PHP"))
                                                    {
                                                        if (!string.IsNullOrEmpty(currency))
                                                            spend += double.Parse(i.Spend ?? "0") * 442.52;
                                                    }
                                                    else if (currency.Trim().Equals("THB"))
                                                    {
                                                        if (!string.IsNullOrEmpty(currency))
                                                            spend += double.Parse(i.Spend ?? "0") * 739.54;
                                                    }
                                                    else
                                                        spend = 0;

                                                    totalSpend += spend;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (totalResult == 0)
                        totalResult = 1;
                    y.Add(totalSpend / totalResult);
                }
            }

            else if (organizationId == null && branchId == null && groupId == null)
            {
                var organizations = _unitOfWork.Organizations.Find(x => x.DeleteDate == null).Include(c => c.Branches).ThenInclude(c => c.Groups)
                    .ThenInclude(c => c.Employees).ThenInclude(c => c.AdsAccounts).ThenInclude(c => c.Campaigns).ThenInclude(c => c.Adsets).ThenInclude(c => c.Ads).ThenInclude(c => c.Insights).ToList();

                foreach (var o in organizations)
                {
                    x.Add(o.Name);
                    double totalSpend = 0;
                    double totalResult = 0;
                    var insight = new List<FBAdsManager.Common.Database.Data.Insight>();
                    foreach (var branch in o.Branches)
                    {
                        foreach (var group in branch.Groups)
                        {
                            foreach (var employee in group.Employees)
                            {
                                foreach (var adAccount in employee.AdsAccounts.Where(x => x.IsDelete == false).ToList())
                                {
                                    var currency = adAccount.Currency ?? "";
                                    foreach (var campaign in adAccount.Campaigns)
                                    {
                                        foreach (var adset in campaign.Adsets)
                                        {
                                            foreach (var ad in adset.Ads)
                                            {
                                                foreach (var i in ad.Insights)
                                                {
                                                    if (i.DateAt >= start && i.DateAt <= end)
                                                    {
                                                        if ((!string.IsNullOrEmpty(i.Actions)) && !i.Actions.Equals("null"))
                                                        {
                                                            var action = JsonSerializer.Deserialize<List<FBAdsManager.Module.DataFacebook.Responses.Action>>(i.Actions);
                                                            if (action != null)
                                                            {
                                                                foreach (var a in action)
                                                                {
                                                                    if (a.action_type.Trim().Equals("onsite_conversion.total_messaging_connection"))
                                                                        totalResult += double.Parse(a.value);
                                                                    break;
                                                                }
                                                            }
                                                        }

                                                        double spend = 0;

                                                        if (currency.Equals("VND"))
                                                        {
                                                            if (!string.IsNullOrEmpty(currency))
                                                                spend += float.Parse(i.Spend ?? "0");
                                                        }
                                                        else if (currency.Trim().Equals("USD"))
                                                        {
                                                            if (!string.IsNullOrEmpty(currency))
                                                                spend += float.Parse(i.Spend ?? "0") * 24670;
                                                        }
                                                        else if (currency.Trim().Equals("EUR"))
                                                        {
                                                            if (!string.IsNullOrEmpty(currency))
                                                                spend += double.Parse(i.Spend ?? "0") * 27447.72;
                                                        }
                                                        else if (currency.Trim().Equals("PHP"))
                                                        {
                                                            if (!string.IsNullOrEmpty(currency))
                                                                spend += double.Parse(i.Spend ?? "0") * 442.52;
                                                        }
                                                        else if (currency.Trim().Equals("THB"))
                                                        {
                                                            if (!string.IsNullOrEmpty(currency))
                                                                spend += double.Parse(i.Spend ?? "0") * 739.54;
                                                        }
                                                        else
                                                            spend = 0;

                                                        totalSpend += spend;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (totalResult == 0)
                        totalResult = 1;
                    y.Add(totalSpend / totalResult);
                }
            }
            return new ResponseService("", new { x = x, y = y });
        }
    }
}
