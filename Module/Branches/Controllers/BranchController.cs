using FBAdsManager.Common.Response.ResponseApi;
using FBAdsManager.Module.Branches.Requests;
using FBAdsManager.Module.Branches.Services;
using FBAdsManager.Module.Organizations.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBAdsManager.Module.Branches.Controllers
{
    [Route("api/branches")]
    [ApiController]
    public class BranchController : BaseController
    {
        private readonly IBranchService _branchService;
        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Add([FromBody] AddBranchRequest request)
        {
            var result = await _branchService.AddAsync(request);
            if (string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseOk(result.Data);
            else
            {
                if (result.StatusCode == 404)
                    return ResponseNotFound(result.ErrorMessage);
                return ResponseBadRequest(result.ErrorMessage);
            }
        }

        [HttpGet("id")]
        [Authorize]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var result = await _branchService.GetDetailAsync(id);
            if (string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseOk(result.Data);
            return ResponseNotFound(result.ErrorMessage);
        }

        [HttpDelete("id")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _branchService.Delete(id);
            if (string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseNoContent();
            return ResponseNotFound(result.ErrorMessage);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetList([FromQuery] int? pageIndex, int? pageSize, Guid? organizationId)
        {
            var result = await _branchService.GetListAsync(pageIndex, pageSize, organizationId);
            if (string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseOkPaging(dataResponse: result.Data, pagingresponse: result.pagingResponse);
            return ResponseBadRequest(result.ErrorMessage);
        }
    }
}
