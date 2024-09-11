using FBAdsManager.Common.Response.ResponseService;

namespace FBAdsManager.Module.Campaigns.Services
{
    public interface ICampaignService
    {
        public Task<ResponseService> GetListAsync(int? PageIndex, int? PageSize, string? adsAccountId);
    }
}
