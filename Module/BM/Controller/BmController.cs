using FBAdsManager.Common.Response.ResponseApi;
using FBAdsManager.Module.BM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBAdsManager.Module.BM.Controller
{
    [Route("api/bm")]
    [ApiController]
    public class BmController : BaseController
    {
        private readonly IBmService _bmService;
        public BmController(IBmService bmService)
        {
            _bmService = bmService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetList([FromQuery] Guid? groupId)
        {
            var result = await _bmService.GetListAsync(groupId);
                return ResponseOk(dataResponse: result.Data);
        }
    }
}
