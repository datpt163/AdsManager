using FBAdsManager.Common.Response.ResponseService;

namespace FBAdsManager.Module.Adsets.Services
{
    public interface IAdsetService
    {
        public Task<ResponseService> GetListAsync(int? PageIndex, int? PageSize, string? campaignId);
    }
}
