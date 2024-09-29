using FBAdsManager.Common.Response.ResponseService;
using FBAdsManager.Module.Organizations.Requests;
using FBAdsManager.Module.Users.Requests;

namespace FBAdsManager.Module.Users.Services
{
    public interface IUserService
    {
    
        public Task<ResponseService> GetListAsyncSystem(int? pageIndex, int? pageSize, Guid? roleId, string token);
        public Task<ResponseService> GetListAsyncBm(int? pageIndex, int? pageSize, Guid? groupId, Guid? branchId, Guid? organizationId);
        public Task<ResponseService> CreateAsyncBm(CreateBmRequest request);
        public Task<ResponseService> Delete(Guid id);
        public Task<ResponseService> AddAsync(AddUserRequest request);
        public Task<ResponseService> UpdateBmAsync(UpdateBmRequest request);
        public Task<ResponseService> UpdateSystemAsync(UpdateUserSystemRequest request);
    }
}
