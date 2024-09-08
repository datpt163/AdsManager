using FBAdsManager.Common.CallApi;
using FBAdsManager.Common.Database.Data;
using FBAdsManager.Common.Database.Repository;
using FBAdsManager.Common.Jwt;
using FBAdsManager.Common.Response.ResponseService;
using FBAdsManager.Module.Auths.Requests;
using FBAdsManager.Module.Auths.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FBAdsManager.Module.Auths.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;
        private readonly CallApiService _callApiService;

       public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService, CallApiService callApiService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
            _callApiService = callApiService;
        }

        public async Task<ResponseService> LoginAsyn(LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.UserName))
                return new ResponseService("User name rỗng", null, 400);
            if (string.IsNullOrEmpty(request.Password))
                return new ResponseService("User name rõng", null, 400);

            var user = await _unitOfWork.Users.Find(x => x.Email.Equals(request.UserName)).Include(c => c.Role).FirstOrDefaultAsync();
            if(user == null)
                return new ResponseService("Tài khoản không tồn tại", null, 404);

            if (user.IsActive == false)
                return new ResponseService("Tài khoản đã bị khóa", null, 400);

            if (user.Password == null || !user.Password.Equals(request.Password))
                return new ResponseService("Mật khẩu không chính xác", null, 400);

            var accessToken = _jwtService.GenerateJwtToken(user, DateTime.Now.AddMonths(1));

            return new ResponseService("", new { AccessToken = accessToken });
        }

        public async Task<ResponseService> LoginByFacebook(string accessTokenFb)
        {
            (int status, ChangeTokenResponse? data) = await _callApiService.GetDataAsync<ChangeTokenResponse>("https://graph.facebook.com/v17.0/oauth/access_token?grant_type=fb_exchange_token&client_id=1615325169044430&client_secret=cd1a6ae60591ccad399c5516ea4c4c3c&fb_exchange_token=" + accessTokenFb );

            if(status == 200 && data != null)
            {
                (int status2, VerifyTokenFbResponse? data2) = await _callApiService.GetDataAsync<VerifyTokenFbResponse>("https://graph.facebook.com/v17.0/me?fields=email&access_token=" + data.access_token);

                if (status2 == 200 && data2 != null)
                {
                    var user = _unitOfWork.Users.Find(x => x.Email.Equals(data2.email) && x.IsActive == true && x.Role.Name == "PM" ).Include(c => c.Role).FirstOrDefault();

                    if (user != null)
                    {
                        var accessToken = _jwtService.GenerateJwtToken(user, DateTime.Now.AddMonths(1));
                        user.AccessTokenFb = data.access_token;
                        _unitOfWork.Users.Update(user);
                        await _unitOfWork.SaveChangesAsync();
                        return new ResponseService("", new { AccessToken = accessToken });
                    }

                    return new ResponseService("This account not found or not have role PM", null, 404);
                }
                return new ResponseService("Some thing wrong", null, 400);
            }
            return new ResponseService("Some thing wrong", null, 400);
        }
    }
}
