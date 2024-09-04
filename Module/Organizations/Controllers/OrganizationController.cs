using FBAdsManager.Common.Response.ResponseApi;
using FBAdsManager.Module.Organizations.Requests;
using FBAdsManager.Module.Organizations.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBAdsManager.Module.Organizations.Controllers
{
    [Route("api/organizations")]
    [ApiController]
    public class OrganizationController : BaseController
    {
        private readonly IOrganizationService _organizationService;
        public OrganizationController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Add([FromBody] AddOrganizationRequest request)
        {
            var result = await _organizationService.AddOrganizationAsync(request);
            if (!string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseBadRequest(messageResponse: result.ErrorMessage);
            return ResponseOk(dataResponse: result.Data);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetList([FromQuery] int? pageIndex, int? pageSize)
        {
            var result = await _organizationService.GetListAsync(pageIndex, pageSize);
            if(string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseOkPaging(dataResponse: result.Data, pagingresponse: result.pagingResponse);
            return ResponseBadRequest(result.ErrorMessage);
        }

        [HttpGet("id")]
        [Authorize]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            var result = await _organizationService.GetDetailAsync(id);
            if (string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseOk(result.Data);
            return ResponseNotFound(result.ErrorMessage);
        }

        [HttpDelete("id")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _organizationService.Delete(id);
            if (string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseNoContent();
            return ResponseNotFound(result.ErrorMessage);
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Update([FromBody] UpdateOrganizationRequest request)
        {
            var result = await _organizationService.Update(request);
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
