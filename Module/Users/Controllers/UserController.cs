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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetList([FromQuery] int? pageIndex, int? pageSize)
        {
            var result = await _userService.GetListAsync(pageIndex, pageSize);
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
    }
}
