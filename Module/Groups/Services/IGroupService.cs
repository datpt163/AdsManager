using FBAdsManager.Common.Response.ResponseService;
using FBAdsManager.Module.Groups.Requests;
using FBAdsManager.Module.Organizations.Requests;

namespace FBAdsManager.Module.Groups.Services
{
    public interface IGroupService
    {
        public Task<ResponseService> AddAsync(AddGroupRequest request);
        public Task<ResponseService> GetListAsync(int? PageIndex, int? PageSize, Guid? organizationId, Guid? branchId);
        //public Task<ResponseService> GetDetailAsync(Guid id);
        public Task<ResponseService> Delete(Guid id);
        public Task<ResponseService> Update(UpdateGroupRequest request);
    }
}
