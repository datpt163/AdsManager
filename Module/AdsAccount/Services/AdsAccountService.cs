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
            if (employee == null)
                return new ResponseService("Không tìm thấy nhân viên này", null, 404);

            if (_unitOfWork.AdsAccounts.FindOne(x => x.AccountId.Trim().Equals(request.AccountID.Trim())) != null)
                return new ResponseService("tài khoản quảng cáo này đã tồn tại", null, 404);

            var listPm = new List<Pm>();
            foreach(var l in request.PmsId)
            {
                var pm = _unitOfWork.Pms.Find(x => x.Id == l).Include(c => c.User).FirstOrDefault();
                if(pm == null)
                    return new ResponseService("Pm Id not found", null, 404);
                if(pm.User != null && pm.User.GroupId != null && pm.User.GroupId == employee.GroupId )
                    listPm.Add(pm);
                else
                {
                    return new ResponseService("Tài khoản BM và employee không thuộc cùng một đội nhóm", null, 404);
                }
            }

            var adsAccount = new FBAdsManager.Common.Database.Data.AdsAccount()
            {
                AccountId = request.AccountID.Trim(),
                EmployeeId = request.EmployeeID,
                IsActive = 0,
            };

            adsAccount.Pms = listPm;  
            _unitOfWork.AdsAccounts.Add(adsAccount);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseService("", adsAccount);
        }

        public async Task<ResponseService> DeleteAsync(string id)
        {
            var adsAccount = _unitOfWork.AdsAccounts.Find(x => x.AccountId == id).Include(c => c.Pms).Include(c => c.Campaigns).ThenInclude(c => c.Adsets).ThenInclude(c => c.Ads).ThenInclude(c => c.Insights).FirstOrDefault();
            if (adsAccount == null)
                return new ResponseService("Not found", null);

            foreach (var l in adsAccount.Campaigns)
            {
                foreach (var c in l.Adsets)
                {
                    foreach(var d in c.Ads)
                    {
                        _unitOfWork.Insights.RemoveRange(d.Insights);
                        _unitOfWork.Adses.Remove(d);
                    }
                    _unitOfWork.Adsets.Remove(c);
                }
                _unitOfWork.Campaigns.Remove(l);
            }
            adsAccount.Pms = new List<Pm>();
            _unitOfWork.AdsAccounts.Update(adsAccount);
            _unitOfWork.AdsAccounts.Remove(adsAccount);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseService("", null);
        }

        public async Task<ResponseService> GetListAsync(int? pageIndex, int? pageSize, bool? isDeleted)
        {
            if (pageIndex != null && pageSize != null)
            {
                if (pageIndex.Value < 1 || pageSize.Value < 0)
                    return new ResponseService("PageIndex, PageSize must >= 0", null);

                int skip = (pageIndex.Value - 1) * pageSize.Value;
                var pagedOrganizationQuery = _unitOfWork.AdsAccounts.GetQuery().Include(c => c.Pms).Skip(skip).Take(pageSize.Value).Include(c => c.Employee).ThenInclude(c => c.Group).ThenInclude(c => c.Branch).ThenInclude(c => c.Organization).ToList();
                if (isDeleted.HasValue)
                    pagedOrganizationQuery = pagedOrganizationQuery.Where(x => x.IsDelete == isDeleted.Value).ToList();
                var totalCount = _unitOfWork.AdsAccounts.Find(x => x.IsDelete == isDeleted).Count();
                return new ResponseService("", pagedOrganizationQuery, new PagingResponse(totalCount, pageIndex.Value, pageSize.Value));
            }

            return new ResponseService("", await _unitOfWork.AdsAccounts.GetQuery().Include(c => c.Pms).Include(c => c.Employee).ThenInclude(c => c.Group).ThenInclude(c => c.Branch).ThenInclude(c => c.Organization).ToListAsync());
        }

        public async Task<ResponseService> GetListAsyncActived(int? pageIndex, int? pageSize, Guid? organizationId, Guid? branchId, Guid? groupId, Guid? employeeId)
        {
            if (pageIndex != null && pageSize != null)
            {
                if (pageIndex.Value < 1 || pageSize.Value < 0)
                    return new ResponseService("PageIndex, PageSize must >= 0", null);

                if (employeeId != null)
                {
                    int skip = (pageIndex.Value - 1) * pageSize.Value;
                    var pagedOrganizationQuery = _unitOfWork.AdsAccounts.Find(x => x.IsActive == 1 && x.IsDelete == false && x.EmployeeId == employeeId).Include(c => c.Pms).Skip(skip).Take(pageSize.Value).Include(c => c.Employee).ThenInclude(c => c.Group);
                    var totalCount = _unitOfWork.AdsAccounts.Find(x => x.IsActive == 1 && x.IsDelete == false && x.EmployeeId == employeeId).Count();
                    return new ResponseService("", pagedOrganizationQuery, new PagingResponse(totalCount, pageIndex.Value, pageSize.Value));
                }

                if (groupId != null)
                {
                    int skip = (pageIndex.Value - 1) * pageSize.Value;
                    var pagedOrganizationQuery = _unitOfWork.AdsAccounts.Find(x => x.IsActive == 1 && x.IsDelete == false && x.Employee != null && x.Employee.GroupId == groupId).Skip(skip).Take(pageSize.Value).Include(c => c.Employee).ThenInclude(c => c.Group).Include(c => c.Pms);
                    var totalCount = _unitOfWork.AdsAccounts.Find(x => x.IsActive == 1 && x.IsDelete == false && x.Employee != null && x.Employee.GroupId == groupId).Count();
                    return new ResponseService("", pagedOrganizationQuery, new PagingResponse(totalCount, pageIndex.Value, pageSize.Value));
                }

                if (branchId != null)
                {
                    int skip = (pageIndex.Value - 1) * pageSize.Value;
                     var pagedOrganizationQuery = _unitOfWork.AdsAccounts.Find(x => x.IsActive == 1 && x.IsDelete == false && x.Employee != null && x.Employee.Group != null && x.Employee.Group.BranchId == branchId).Skip(skip).Take(pageSize.Value).Include(c => c.Pms).Include(c => c.Employee).ThenInclude(c => c.Group);
                    var totalCount = _unitOfWork.AdsAccounts.Find(x => x.IsActive == 1 && x.IsDelete == false &&  x.Employee != null && x.Employee.Group != null && x.Employee.Group.BranchId == branchId).Count();
                    return new ResponseService("", pagedOrganizationQuery, new PagingResponse(totalCount, pageIndex.Value, pageSize.Value));
                }

                if (organizationId != null)
                {
                    int skip = (pageIndex.Value - 1) * pageSize.Value;
                    var pagedOrganizationQuery = _unitOfWork.AdsAccounts.Find(x => x.IsActive == 1 && x.IsDelete == false && x.Employee != null && x.Employee.Group != null && x.Employee.Group.Branch != null && x.Employee.Group.Branch.OrganizationId == organizationId).Include(c => c.Pms).Skip(skip).Take(pageSize.Value).Include(c => c.Employee).ThenInclude(c => c.Group);
                    var totalCount = _unitOfWork.AdsAccounts.Find(x => x.IsActive == 1 && x.IsDelete == false &&  x.Employee != null && x.Employee.Group != null && x.Employee.Group.Branch != null && x.Employee.Group.Branch.OrganizationId == organizationId).Count();
                    return new ResponseService("", pagedOrganizationQuery, new PagingResponse(totalCount, pageIndex.Value, pageSize.Value));
                }

                int skip2 = (pageIndex.Value - 1) * pageSize.Value;
                var pagedOrganizationQuery2 = _unitOfWork.AdsAccounts.Find(x => x.IsActive == 1 && x.IsDelete == false ).Include(c => c.Pms).Skip(skip2).Take(pageSize.Value).Include(c => c.Employee).ThenInclude(c => c.Group);
                var totalCount2 = _unitOfWork.AdsAccounts.Find(x => x.IsActive == 1 && x.IsDelete == false ).Count();
                return new ResponseService("", pagedOrganizationQuery2, new PagingResponse(totalCount2, pageIndex.Value, pageSize.Value));
            }

            return new ResponseService("", await _unitOfWork.AdsAccounts.Find(x => x.IsActive == 1).Include(c => c.Employee).ThenInclude(c => c.Group).Include(c => c.Pms).ToListAsync());
        }

        public async Task<ResponseService> Toggle(string id)
        {
            var adsAccount = _unitOfWork.AdsAccounts.FindOne(x => x.AccountId.Equals(id));
            if(adsAccount == null)
                return new ResponseService("Không tìm thấy tài khoản quảng cáo này", null, 400);
            adsAccount.IsDelete = !adsAccount.IsDelete;
            _unitOfWork.AdsAccounts.Update(adsAccount);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseService("", null);
        }

        public async Task<ResponseService> UpdateAsync(UpdateAdsAccountRequest request)
        {
            var adsAccount = _unitOfWork.AdsAccounts.Find(x => x.AccountId == request.AccountID).Include(c => c.Pms).FirstOrDefault();

            if (adsAccount == null)
                return new ResponseService("Không tìm thấy tài khoản quảng cáo này", null, 400);

            var employee = _unitOfWork.Employees.FindOne(x => x.Id == request.EmployeeID);
            if (employee == null)
                return new ResponseService("Không tìm thấy nhân viên này", null, 404);

            var listPm = new List<Pm>();
            foreach (var l in request.PmsId)
            {
                var pm = _unitOfWork.Pms.Find(x => x.Id == l).Include(c => c.User).FirstOrDefault();
                if (pm == null)
                    return new ResponseService("Pm Id not found", null, 404);
                if (pm.User != null && pm.User.GroupId != null && pm.User.GroupId == employee.GroupId)
                    listPm.Add(pm);
                else
                {
                    return new ResponseService("Tài khoản BM và employee không thuộc cùng một đội nhóm", null, 404);
                }
            }
            adsAccount.Pms = listPm;
            adsAccount.EmployeeId = request.EmployeeID;
            _unitOfWork.AdsAccounts.Update(adsAccount);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseService("", null);
        }
    }
}
