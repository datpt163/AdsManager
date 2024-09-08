using FBAdsManager.Common.Response.ResponseService;
using FBAdsManager.Module.Organizations.Requests;
using FBAdsManager.Module.Users.Requests;

namespace FBAdsManager.Module.Users.Services
{
    public interface IUserService
    {
        public Task<ResponseService> GetListAsync(int? PageIndex, int? PageSize);
        public Task<ResponseService> AddAsync(AddUserRequest request);
    }
}
