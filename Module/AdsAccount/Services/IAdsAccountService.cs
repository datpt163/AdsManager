using FBAdsManager.Common.Response.ResponseService;
using FBAdsManager.Module.AdsAccount.Requests;

namespace FBAdsManager.Module.AdsAccount.Services
{
    public interface IAdsAccountService
    {
        public Task<ResponseService> AddAsync(string token, AddAccountRequest request);
        public Task<ResponseService>UpdateAsync(UpdateAdsAccountRequest request);
        public Task<ResponseService> GetListAsync(int? PageIndex, int? PageSize);
        public Task<ResponseService> DeleteAsync(string id);
        public Task<ResponseService> GetListAsyncActived(int? PageIndex, int? PageSize, Guid? organizationId, Guid? branchId, Guid? groupId, Guid? employeeId );
    }
}
