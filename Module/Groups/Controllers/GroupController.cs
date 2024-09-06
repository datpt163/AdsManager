using FBAdsManager.Common.Database.Repository;
using FBAdsManager.Common.Response.ResponseApi;
using FBAdsManager.Module.Branches.Requests;
using FBAdsManager.Module.Groups.Requests;
using FBAdsManager.Module.Groups.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBAdsManager.Module.Groups.Controllers
{
    [Route("api/groups")]
    [ApiController]
    public class GroupController : BaseController
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetList([FromQuery] int? pageIndex, int? pageSize, Guid? organizationId, Guid? branchId)
        {
            var result = await _groupService.GetListAsync(pageIndex, pageSize, organizationId, branchId);
            if (string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseOkPaging(dataResponse: result.Data, pagingresponse: result.pagingResponse);
            return ResponseBadRequest(result.ErrorMessage);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Add([FromBody] AddGroupRequest request)
        {
            var result = await _groupService.AddAsync(request);
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
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Delete([FromQuery] Guid id)
        {
            var result = await _groupService.Delete(id);
            if (string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseNoContent();
            return ResponseNotFound(result.ErrorMessage);
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Update([FromBody] UpdateGroupRequest request)
        {
            var result = await _groupService.Update(request);
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
