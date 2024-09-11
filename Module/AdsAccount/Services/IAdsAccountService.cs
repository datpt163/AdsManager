using FBAdsManager.Common.Response.ResponseService;
using FBAdsManager.Module.AdsAccount.Requests;

namespace FBAdsManager.Module.AdsAccount.Services
{
    public interface IAdsAccountService
    {
        public Task<ResponseService> AddAsync(string token, AddAccountRequest request);
        public Task<ResponseService> GetListAsync(int? PageIndex, int? PageSize);
    }
}
