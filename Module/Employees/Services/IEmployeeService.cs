using FBAdsManager.Common.Response.ResponseService;
using FBAdsManager.Module.Employees.Requests;
using FBAdsManager.Module.Groups.Requests;

namespace FBAdsManager.Module.Employees.Services
{
    public interface IEmployeeService
    {
        public Task<ResponseService> AddAsync(AddEmployeeRequest request);
        public Task<ResponseService> GetListAsync(int? PageIndex, int? PageSize, Guid? organizationId, Guid? branchId, Guid? groupId);
        //public Task<ResponseService> GetDetailAsync(Guid id);
        public Task<ResponseService> Delete(Guid id);
        public Task<ResponseService> Update(UpdateEmployeeRequest request);
    }
}
