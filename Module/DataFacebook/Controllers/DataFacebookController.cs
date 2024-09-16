using FBAdsManager.Common.Response.ResponseApi;
using FBAdsManager.Module.DataFacebook.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBAdsManager.Module.DataFacebook.Controllers
{
    [Route("api/datafacebook")]
    [ApiController]
    public class DataFacebookController : BaseController
    {
        private readonly IDataFacebookService _dataFacebookService;
        public DataFacebookController(IDataFacebookService dataFacebookService)
        {
            _dataFacebookService = dataFacebookService;
        }
        [HttpGet]
        [Authorize(Roles = "BM")]
        public async Task<IActionResult> GetData()
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _dataFacebookService.CrawlData(token);
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
    }
}
