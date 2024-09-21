using FBAdsManager.Common.Response.ResponseService;

namespace FBAdsManager.Module.BM.Services
{
    public interface IBmService
    {
        public Task<ResponseService> GetListAsync(Guid? groupId);
    }
}
