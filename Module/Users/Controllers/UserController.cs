using FBAdsManager.Common.Response.ResponseApi;
using FBAdsManager.Module.Organizations.Requests;
using FBAdsManager.Module.Users.Requests;
using FBAdsManager.Module.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBAdsManager.Module.Users.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("system")]
        [Authorize]
        public async Task<IActionResult> GetList([FromQuery] int? pageIndex, int? pageSize)
        {
            var result = await _userService.GetListAsyncSystem(pageIndex, pageSize);
            if (string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseOkPaging(dataResponse: result.Data, pagingresponse: result.pagingResponse);
            return ResponseBadRequest(result.ErrorMessage);
        }

        [HttpGet("bm")]
        [Authorize]
        public async Task<IActionResult> GetList2([FromQuery] int? pageIndex, int? pageSize)
        {
            var result = await _userService.GetListAsyncBm(pageIndex, pageSize);
            if (string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseOkPaging(dataResponse: result.Data, pagingresponse: result.pagingResponse);
            return ResponseBadRequest(result.ErrorMessage);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Add([FromBody] AddUserRequest request)
        {
            var result = await _userService.AddAsync(request);
            if (!string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseBadRequest(messageResponse: result.ErrorMessage);
            return ResponseOk(dataResponse: result.Data);
        }

        [HttpPost("bm")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Add([FromBody] CreateBmRequest request)
        {
            var result = await _userService.CreateAsyncBm(request);
            if (!string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseBadRequest(messageResponse: result.ErrorMessage);
            return ResponseOk(dataResponse: result.Data);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> remove(Guid id)
        {
            var result = await _userService.Delete(id);
            if (!string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseBadRequest(messageResponse: result.ErrorMessage);
            return ResponseOk(dataResponse: result.Data);
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> update(UpdateBmRequest request)
        {
            var result = await _userService.UpdateBmAsync(request);
            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                if(result.StatusCode == 404)
                    return ResponseNotFound(messageResponse: result.ErrorMessage);
                return ResponseBadRequest(messageResponse: result.ErrorMessage);
            }
            return ResponseOk(dataResponse: result.Data);
        }

        [HttpPut("system")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> update(UpdateUserSystemRequest request)
        {
            var result = await _userService.UpdateSystemAsync(request);
            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                if (result.StatusCode == 404)
                    return ResponseNotFound(messageResponse: result.ErrorMessage);
                return ResponseBadRequest(messageResponse: result.ErrorMessage);
            }
            return ResponseOk(dataResponse: result.Data);
        }
    }
}
