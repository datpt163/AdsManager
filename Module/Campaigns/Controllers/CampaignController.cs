using FBAdsManager.Common.Response.ResponseApi;
using FBAdsManager.Module.Campaigns.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBAdsManager.Module.Campaigns.Controllers
{
    [Route("api/campaign")]
    [ApiController]
    public class CampaignController : BaseController
    {
        private readonly ICampaignService _campaignService;
        public CampaignController(ICampaignService campaignService)
        {
            _campaignService = campaignService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetList([FromQuery] int? pageIndex, int? pageSize, string? adsAccountId)
        {
            var result = await _campaignService.GetListAsync(pageIndex, pageSize, adsAccountId);
            if (string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseOkPaging(dataResponse: result.Data, pagingresponse: result.pagingResponse);
            return ResponseBadRequest(result.ErrorMessage);
        }
    }
}
