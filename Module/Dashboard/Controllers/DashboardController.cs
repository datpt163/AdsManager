using FBAdsManager.Common.Response.ResponseApi;
using FBAdsManager.Module.Dashboard.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FBAdsManager.Module.Dashboard.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : BaseController
    {
        private readonly IDashBoardService _dashboardService;
        public DashboardController(IDashBoardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("spend")]
        public IActionResult StatisticCost(Guid? organizationId, Guid? branchId, Guid? groupId, DateTime start, DateTime end)
        {
            return ResponseOk(_dashboardService.StatisticSpend(organizationId, branchId, groupId, start, end));
        }

        [HttpGet("campaign")]
        public IActionResult StatisticCampaign(Guid? organizationId, Guid? branchId, Guid? groupId, DateTime start, DateTime end)
        {
            return ResponseOk(_dashboardService.StatisticCampaign(organizationId, branchId, groupId, start, end));
        }

        [HttpGet("result")]
        public IActionResult StatisticResult(Guid? organizationId, Guid? branchId, Guid? groupId, DateTime start, DateTime end)
        {
            return ResponseOk(_dashboardService.StatisticResult(organizationId, branchId, groupId, start, end));
        }

        [HttpGet("costPerResult")]
        public IActionResult costPerResult(Guid? organizationId, Guid? branchId, Guid? groupId, DateTime start, DateTime end)
        {
            var result = _dashboardService.StatisticCostPerResult(organizationId, branchId, groupId, start, end);
            if (string.IsNullOrEmpty(result.ErrorMessage))
                return ResponseOk(result.Data);
            return ResponseBadRequest(result.ErrorMessage);
        }
    }
}
