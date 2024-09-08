using FBAdsManager.Common.Response.ResponseService;
using FBAdsManager.Module.Auths.Requests;

namespace FBAdsManager.Module.Auths.Services
{
    public interface IAuthService
    {
        public Task<ResponseService> LoginAsyn(LoginRequest request);
        public Task<ResponseService> LoginByFacebook(string accessTokenFb);
    }
}
