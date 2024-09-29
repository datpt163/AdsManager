using FBAdsManager.Common.Response.ResponseApi;
using FBAdsManager.Module.AdsAccount.Requests;
using FBAdsManager.Module.AdsAccount.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace FBAdsManager.Module.AdsAccount.Controllers
{
    [Route("api/adsAccount")]
    [ApiController]
    public class AdsAccountController : BaseController
    {
        private readonly IAdsAccountService _adsAccountService;

     public AdsAccountController(IAdsAccountService adsAccountService)
        {
            _adsAccountService = adsAccountService;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Add([FromBody] AddAccountRequest request)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _adsAccountService.AddAsync(token, request);
            if (string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseOk(result.Data);
            else
            {
                if (result.StatusCode == 404)
                    return ResponseNotFound(result.ErrorMessage);
                if (result.StatusCode == 401)
                    return ResponseUnauthorized(result.ErrorMessage);
                return ResponseBadRequest(result.ErrorMessage);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetList([FromQuery] int? pageIndex, int? pageSize, bool? isDelete, Guid? organizationId, Guid? branchId, Guid? groupId, Guid? employeeId)
        {
            var result = await _adsAccountService.GetListAsync(pageIndex, pageSize, isDelete, organizationId, branchId,groupId,employeeId);
            if (string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseOkPaging(dataResponse: result.Data, pagingresponse: result.pagingResponse);
            return ResponseBadRequest(result.ErrorMessage);
        }

        [HttpGet("isActive")]
        [Authorize]
        public async Task<IActionResult> GetList2([FromQuery] int? pageIndex, int? pageSize, Guid? organizationId, Guid? branchId, Guid? groupId, Guid? employeeId)
        {
            var result = await _adsAccountService.GetListAsyncActived(pageIndex, pageSize, organizationId, branchId, groupId, employeeId);
            if (string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseOkPaging(dataResponse: result.Data, pagingresponse: result.pagingResponse);
            return ResponseBadRequest(result.ErrorMessage);
        }
        [HttpPut("{id}/toggle")]
        [Authorize]
        public async Task<IActionResult> Toggle(string id)
        {
            var result = await _adsAccountService.Toggle(id);
            if (string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseOkPaging(dataResponse: result.Data, pagingresponse: result.pagingResponse);
            return ResponseBadRequest(result.ErrorMessage);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> GetList2(UpdateAdsAccountRequest request)
        {
            var result = await _adsAccountService.UpdateAsync(request);
            if (string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseOkPaging(dataResponse: result.Data, pagingresponse: result.pagingResponse);
            return ResponseBadRequest(result.ErrorMessage);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _adsAccountService.DeleteAsync(id);
            if (string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseOk();
            return ResponseBadRequest(result.ErrorMessage);
        }

        [HttpPost("excel")]
        [Authorize]
        public async Task<IActionResult> AddByExcel([FromForm] AddByExcel request)
        {
            if (request.file == null || request.file.Length == 0)
            {
                return ResponseBadRequest("File not found");
            }
            var result = await _adsAccountService.AddByExcel(request.file);
            if(!string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseBadRequest(result.ErrorMessage);
            return ResponseOk(result.Data);
        }
    }
}
