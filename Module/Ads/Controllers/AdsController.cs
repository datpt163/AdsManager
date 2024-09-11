using FBAdsManager.Common.Response.ResponseApi;
using FBAdsManager.Module.Ads.Services;
using FBAdsManager.Module.Adsets.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBAdsManager.Module.Ads.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdsController : BaseController
    {
        private readonly IAdsService _adsService;
        public AdsController(IAdsService adsService)
        {
            _adsService = adsService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetList([FromQuery] int? pageIndex, int? pageSize, string? adsAccountId)
        {
            var result = await _adsService.GetListAsync(pageIndex, pageSize, adsAccountId);
            if (string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseOkPaging(dataResponse: result.Data, pagingresponse: result.pagingResponse);
            return ResponseBadRequest(result.ErrorMessage);
        }
    }
}
