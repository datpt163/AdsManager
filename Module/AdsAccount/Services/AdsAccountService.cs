using FBAdsManager.Common.CallApi;
using FBAdsManager.Common.Database.Data;
using FBAdsManager.Common.Database.Repository;
using FBAdsManager.Common.Jwt;
using FBAdsManager.Common.Paging;
using FBAdsManager.Common.Response.ResponseService;
using FBAdsManager.Module.AdsAccount.Requests;
using FBAdsManager.Module.AdsAccount.Responses;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace FBAdsManager.Module.AdsAccount.Services
{
    public class AdsAccountService : IAdsAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly CallApiService _callApiService;

        public AdsAccountService(IUnitOfWork unitOfWork, IJwtService jwtService, CallApiService callApiService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _callApiService = callApiService;
        }

        public async Task<ResponseService> AddAsync(string token, AddAccountRequest request)
        {

            var employee = _unitOfWork.Employees.FindOne(x => x.Id == request.EmployeeID);
            if(employee == null)
                return new ResponseService("Không tìm thấy nhân viên này", null,404);

            var user = await _jwtService.VerifyTokenAsync(token);
            (int statusCode, AdsAccountFbResponse? Data) = await _callApiService.GetDataAsync<AdsAccountFbResponse>("https://graph.facebook.com/v20.0/"+ request.AccountID + "?fields=id,account_id,name,account_status,currency,spend_cap,amount_spent,balance,business,created_time,owner,timezone_id,timezone_name,disable_reason,funding_source,funding_source_details,min_campaign_group_spend_cap,min_daily_budget,partner,business_city,business_country_code,business_name,business_state,business_street,business_street2,business_zip,capabilities,is_personal,line_numbers&access_token=" + user.AccessTokenFb);
            if(statusCode == 405 )
                return new ResponseService("Tài khoản quảng cáo không thuộc tài khoản PM này", null, 404);
            if(statusCode == 401)
                return new ResponseService("Access Token fb expired", null, 401);
            if(employee.GroupId != employee.GroupId)
                return new ResponseService("This group of ads account not belong PM", null, 400);

            if (Data != null)
            {
                var adsAccount = new FBAdsManager.Common.Database.Data.AdsAccount()
                {
                    AccountId = Data.account_id,
                    EmployeeId = request.EmployeeID,
                    Name = Data.name,
                    AccountStatus = Data.account_status,
                    Currency = Data.currency,
                    SpendCap = Data.spend_cap,
                    AmountSpent = Data.amount_spent,
                    Balance = Data.balance,
                    CreatedTime = Data.created_time,
                    Owner = Data.owner,
                    TimezoneName = Data.timezone_name,
                    DisableReason = Data.disable_reason,
                    InforCardBanking = Data.funding_source_details != null ? Data.funding_source_details.display_string : "",
                    TypeCardBanking = Data.funding_source_details != null ? Data.funding_source_details.type : -1,
                    MinCampaignGroupSpendCap = Data.min_campaign_group_spend_cap,
                    MinDailyBudget = Data.min_daily_budget,
                    IsPersonal = Data.is_personal,
                    UpdateDataTime = DateTime.Now,
                };

                var AdsAccountCheck = _unitOfWork.AdsAccounts.FindOne(x => x.AccountId.Equals(Data.account_id));
                if(AdsAccountCheck != null )
                    return new ResponseService("Tài khoản quảng cáo này đã được thêm", null, 400);

                _unitOfWork.AdsAccounts.Add(adsAccount);
                await _unitOfWork.SaveChangesAsync();
                return new ResponseService("", adsAccount);
            }
            return new ResponseService("Some thing wrong", null,400) ;
        }

        public async Task<ResponseService> GetListAsync(int? pageIndex, int? pageSize)
        {
            if (pageIndex != null && pageSize != null)
            {
                if (pageIndex.Value < 1 || pageSize.Value < 0)
                    return new ResponseService("PageIndex, PageSize must >= 0", null);

                int skip = (pageIndex.Value - 1) * pageSize.Value;
                var pagedOrganizationQuery = _unitOfWork.AdsAccounts.GetQuery().Skip(skip).Take(pageSize.Value).Include(c => c.Employee);
                var totalCount = _unitOfWork.AdsAccounts.GetQuery().Count();
                return new ResponseService("", pagedOrganizationQuery, new PagingResponse(totalCount, pageIndex.Value, pageSize.Value));
            }

            return new ResponseService("", await _unitOfWork.AdsAccounts.GetQuery().Include(c => c.Employee).ToListAsync());
        }
    }
}
