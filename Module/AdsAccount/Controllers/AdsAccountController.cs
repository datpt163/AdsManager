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
        [Authorize(Roles = "PM")]
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
        public async Task<IActionResult> GetList([FromQuery] int? pageIndex, int? pageSize)
        {
            var result = await _adsAccountService.GetListAsync(pageIndex, pageSize);
            if (string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseOkPaging(dataResponse: result.Data, pagingresponse: result.pagingResponse);
            return ResponseBadRequest(result.ErrorMessage);
        }
    }
}
