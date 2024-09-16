using FBAdsManager.Common.Database.Data;
using FBAdsManager.Common.Database.Repository;
using FBAdsManager.Common.Paging;
using FBAdsManager.Common.Response.ResponseService;
using FBAdsManager.Module.DataFacebook.Responses;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace FBAdsManager.Module.Ads.Services
{
    public class AdsService : IAdsService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AdsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseService> GetListAsync(int? pageIndex, int? pageSize, string? adsetId)
        {
            var insign = _unitOfWork.Insights.GetQuery().FirstOrDefault();
            if (pageIndex != null && pageSize != null && adsetId != null)
            {
                if (pageIndex.Value < 1 || pageSize.Value < 0)
                    return new ResponseService("PageIndex, PageSize must >= 0", null);

                int skip = (pageIndex.Value - 1) * pageSize.Value;
                var pagedOrganizationQuery = await _unitOfWork.Adses.Find(c => c.AdsetId != null && c.AdsetId.Equals(adsetId)).Skip(skip).Take(pageSize.Value).Select(x => new
                {
                    Id = x.Id,
                    AdsetId = x.AdsetId,
                    Name = x.Name,
                    ActionType = x.ActionType,
                    TrackingSpecs = x.TrackingSpecs,
                    Adcreatives = x.Adcreatives,
                    EffectiveStatus = x.EffectiveStatus,
                    Status = x.Status,
                    ConfiguredStatus = x.ConfiguredStatus,
                    CreatedTime = x.CreatedTime,
                    StartTime = x.StartTime,
                    UpdatedTime = x.UpdatedTime,
                    UpdateDataTime = x.UpdateDataTime,
                    Insighn = x.Insights.FirstOrDefault(),

                }).ToListAsync();
                var totalCount = _unitOfWork.Adses.Find(c => c.AdsetId != null && c.AdsetId.Equals(adsetId)).Count();
                return new ResponseService("", pagedOrganizationQuery, new PagingResponse(totalCount, pageIndex.Value, pageSize.Value));
            }
            var response = await _unitOfWork.Adses.Find(c => c.AdsetId != null && c.AdsetId.Equals(adsetId)).Select(x => new
            {
                Id = x.Id,
                AdsetId = x.AdsetId,
                Name = x.Name,
                ActionType = x.ActionType,
                TrackingSpecs = x.TrackingSpecs,
                Adcreatives = x.Adcreatives,
                EffectiveStatus = x.EffectiveStatus,
                Status = x.Status,
                ConfiguredStatus = x.ConfiguredStatus,
                CreatedTime = x.CreatedTime,
                StartTime = x.StartTime,
                UpdatedTime = x.UpdatedTime,
                UpdateDataTime = x.UpdateDataTime,
                Insighn = x.Insights.FirstOrDefault(),

            }).ToListAsync();
            return new ResponseService("", response);
        }
    }
}
