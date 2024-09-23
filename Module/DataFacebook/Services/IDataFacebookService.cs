using FBAdsManager.Common.Response.ResponseService;

namespace FBAdsManager.Module.DataFacebook.Services
{
    public interface IDataFacebookService
    {
        
        public Task<ResponseService> CrawlData(string token, DateTime? since, DateTime? until);
        public Task<ResponseService> CheckFacebookTokenExpire(string token);
    }
}
