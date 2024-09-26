using FBAdsManager.Common.Response.ResponseApi;
using FBAdsManager.Module.Branches.Requests;
using FBAdsManager.Module.Branches.Services;
using FBAdsManager.Module.Employees.Requests;
using FBAdsManager.Module.Employees.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBAdsManager.Module.Employees.Controllers
{
    [Route("api/employee")]
    [ApiController]
    public class EmployeeController : BaseController
    {
        private readonly IEmployeeService _employeeSerivce;
        public EmployeeController(IEmployeeService employeeSerivce)
        {
            _employeeSerivce = employeeSerivce;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN,BRANCH,ORGANIZATION,GROUP")]
        public async Task<IActionResult> Add([FromBody] AddEmployeeRequest request)
        {
            var result = await _employeeSerivce.AddAsync(request);
            if (string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseOk(result.Data);
            else
            {
                if (result.StatusCode == 404)
                    return ResponseNotFound(result.ErrorMessage);
                return ResponseBadRequest(result.ErrorMessage);
            }
        }


        [HttpDelete]
        [Authorize(Roles = "ADMIN,BRANCH,ORGANIZATION,GROUP")]
        public async Task<IActionResult> Delete([FromQuery] Guid id)
        {
            var result = await _employeeSerivce.Delete(id);
            if (string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseNoContent();
            return ResponseNotFound(result.ErrorMessage);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetList([FromQuery] int? pageIndex, int? pageSize, Guid? organizationId, Guid? branchId, Guid? groupId)
        {
            var result = await _employeeSerivce.GetListAsync(pageIndex, pageSize, organizationId, branchId, groupId);
            if (string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseOkPaging(dataResponse: result.Data, pagingresponse: result.pagingResponse);
            return ResponseBadRequest(result.ErrorMessage);
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN,BRANCH,ORGANIZATION,GROUP")]
        public async Task<IActionResult> Update([FromBody] UpdateEmployeeRequest request)
        {
            var result = await _employeeSerivce.Update(request);
            if (string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseOk(result.Data);
            else
            {
                if (result.StatusCode == 404)
                    return ResponseNotFound(result.ErrorMessage);
                return ResponseBadRequest(result.ErrorMessage);
            }
        }
    }
}
