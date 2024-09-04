using FBAdsManager.Common.Database.Data;

namespace FBAdsManager.Common.Jwt
{
    public interface IJwtService
    {
        string GenerateJwtToken(User account, DateTime expireTime);
        Task<User> VerifyTokenAsync(string token);
    }
}
