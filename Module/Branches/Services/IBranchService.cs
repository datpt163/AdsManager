using FBAdsManager.Common.Response.ResponseService;
using FBAdsManager.Module.Branches.Requests;
using FBAdsManager.Module.Organizations.Requests;

namespace FBAdsManager.Module.Branches.Services
{
    public interface IBranchService
    {
        public Task<ResponseService> AddAsync(AddBranchRequest request);
        public Task<ResponseService> GetListAsync(int? PageIndex, int? PageSize, Guid? organization);
        public Task<ResponseService> GetDetailAsync(Guid id);
        public Task<ResponseService> Delete(Guid id);
        //public Task<ResponseService> Update(UpdateOrganizationRequest request);
    }
}
