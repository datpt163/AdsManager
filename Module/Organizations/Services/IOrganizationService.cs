using FBAdsManager.Common.Response.ResponseService;
using FBAdsManager.Module.Organizations.Requests;

namespace FBAdsManager.Module.Organizations.Services
{
    public interface IOrganizationService
    {
        public Task<ResponseService> AddOrganizationAsync(AddOrganizationRequest request);
        public Task<ResponseService> GetListAsync(int? PageIndex, int? PageSize);
        public Task<ResponseService> GetDetailAsync(Guid id);
        public Task<ResponseService> Delete(Guid id);
        public Task<ResponseService> Update(UpdateOrganizationRequest request);
    }
}
