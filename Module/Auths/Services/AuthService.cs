using FBAdsManager.Common.Database.Repository;
using FBAdsManager.Common.Jwt;
using FBAdsManager.Common.Response.ResponseService;
using FBAdsManager.Module.Auths.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FBAdsManager.Module.Auths.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtService _jwtService;

        public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService)
        {
            _unitOfWork = unitOfWork;
            _jwtService = jwtService;
        }

        public async Task<ResponseService> LoginAsyn(LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.UserName))
                return new ResponseService("User name rỗng", null, 400);
            if (string.IsNullOrEmpty(request.Password))
                return new ResponseService("User name rõng", null, 400);

            var user = await _unitOfWork.Users.Find(x => x.UserName.Equals(request.UserName)).Include(c => c.Role).FirstOrDefaultAsync();
            if(user == null)
                return new ResponseService("Tài khoản không tồn tại", null, 404);

            if(!user.Password.Equals(request.Password))
                return new ResponseService("Mật khẩu không chính xác", null, 400);

            var accessToken = _jwtService.GenerateJwtToken(user, DateTime.Now.AddMonths(1));

            return new ResponseService("", new { AccessToken = accessToken });
        }
    }
}
