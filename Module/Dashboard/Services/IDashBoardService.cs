using FBAdsManager.Common.Response.ResponseService;

namespace FBAdsManager.Module.Dashboard.Services
{
    public interface IDashBoardService
    {
        public ResponseService StatisticSpend(Guid? organizationId, Guid? branchId, Guid? groupId, DateTime start, DateTime end);
        public ResponseService StatisticCampaign(Guid? organizationId, Guid? branchId, Guid? groupId, DateTime start, DateTime end);
        public ResponseService StatisticResult(Guid? organizationId, Guid? branchId, Guid? groupId, DateTime start, DateTime end);
        public ResponseService StatisticCostPerResult(Guid? organizationId, Guid? branchId, Guid? groupId, DateTime start, DateTime end);
    }
}
