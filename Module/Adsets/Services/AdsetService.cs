using FBAdsManager.Common.Database.Repository;
using FBAdsManager.Common.Paging;
using FBAdsManager.Common.Response.ResponseService;
using Microsoft.EntityFrameworkCore;

namespace FBAdsManager.Module.Adsets.Services
{
    public class AdsetService : IAdsetService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AdsetService (IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseService> GetListAsync(int? pageIndex, int? pageSize, string? campaignId)
        {
            if (pageIndex != null && pageSize != null && campaignId != null)
            {
                if (pageIndex.Value < 1 || pageSize.Value < 0)
                    return new ResponseService("PageIndex, PageSize must >= 0", null);

                int skip = (pageIndex.Value - 1) * pageSize.Value;
                var pagedOrganizationQuery = await _unitOfWork.Adsets.Find(c => c.CampaignId != null && c.CampaignId.Equals(campaignId)).Skip(skip).Take(pageSize.Value).ToListAsync();
                var totalCount = _unitOfWork.Adsets.Find(c => c.CampaignId != null && c.CampaignId.Equals(campaignId)).Count();
                return new ResponseService("", pagedOrganizationQuery, new PagingResponse(totalCount, pageIndex.Value, pageSize.Value));
            }

            return new ResponseService("", await _unitOfWork.Adsets.Find(c => c.CampaignId != null && c.CampaignId.Equals(campaignId)).ToListAsync());
        }
    }
}
