using FBAdsManager.Common.Response.ResponseApi;
using FBAdsManager.Module.Adsets.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBAdsManager.Module.Adsets.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdsetController : BaseController
    {
        private readonly IAdsetService _adsetService;
        public AdsetController(IAdsetService adsetService)
        {
            _adsetService = adsetService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetList([FromQuery] int? pageIndex, int? pageSize, string? adsAccountId, DateTime start, DateTime end)
        {
            var result = await _adsetService.GetListAsync(pageIndex, pageSize, adsAccountId, start, end);
            if (string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseOkPaging(dataResponse: result.Data, pagingresponse: result.pagingResponse);
            return ResponseBadRequest(result.ErrorMessage);
        }
    }
}
