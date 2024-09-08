using FBAdsManager.Common.Response.ResponseApi;
using FBAdsManager.Module.Auths.Requests;
using FBAdsManager.Module.Auths.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBAdsManager.Module.Auths.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Auth([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsyn(request);

            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                if (result.StatusCode == 400)
                    return ResponseBadRequest(messageResponse: result.ErrorMessage);
                return ResponseNotFound(messageResponse: result.ErrorMessage);
            }
            return ResponseOk(dataResponse: result.Data);
        }

        [HttpGet("facebook")]
        public async Task<IActionResult> LoginByFacebook(string accessToken)
        {
            var result = await _authService.LoginByFacebook(accessToken);

            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                if (result.StatusCode == 400)
                    return ResponseBadRequest(messageResponse: result.ErrorMessage);
                return ResponseNotFound(messageResponse: result.ErrorMessage);
            }
            return ResponseOk(dataResponse: result.Data);
        }
    }
}
