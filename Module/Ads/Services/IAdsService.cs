using FBAdsManager.Common.Response.ResponseService;

namespace FBAdsManager.Module.Ads.Services
{
    public interface IAdsService
    {
        public Task<ResponseService> GetListAsync(int? PageIndex, int? PageSize, string? adsetId);
    }
}
