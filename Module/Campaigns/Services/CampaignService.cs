using FBAdsManager.Common.Database.Repository;
using FBAdsManager.Common.Paging;
using FBAdsManager.Common.Response.ResponseService;
using Microsoft.EntityFrameworkCore;

namespace FBAdsManager.Module.Campaigns.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CampaignService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseService> GetListAsync(int? pageIndex, int? pageSize, string? adsAccountId)
        {
            if (pageIndex != null && pageSize != null && adsAccountId != null)
            {
                if (pageIndex.Value < 1 || pageSize.Value < 0)
                    return new ResponseService("PageIndex, PageSize must >= 0", null);

                int skip = (pageIndex.Value - 1) * pageSize.Value;
                var pagedOrganizationQuery = _unitOfWork.Campaigns.Find(c => c.Account != null && c.AccountId != null && c.AccountId.Equals(adsAccountId)).Skip(skip).Take(pageSize.Value).Include(c => c.Account);
                var totalCount = _unitOfWork.Campaigns.Find(c => c.AccountId != null && c.AccountId.Equals(adsAccountId)).Count();
                return new ResponseService("", pagedOrganizationQuery, new PagingResponse(totalCount, pageIndex.Value, pageSize.Value));
            }

            return new ResponseService("", await _unitOfWork.Campaigns.Find(c => c.Account != null && c.AccountId != null && c.AccountId.Equals(adsAccountId)).Include(c => c.Account).ToListAsync());
        }
    }
}
