using FBAdsManager.Common.Database.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using FBAdsManager.Common.Database.Data;

namespace FBAdsManager.Common.Jwt
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IUnitOfWork _unitOfWork;
        public JwtService(IOptions<JwtSettings> jwtSettings, IUnitOfWork unitOfWork)
        {
            _jwtSettings = jwtSettings.Value;
            _unitOfWork = unitOfWork;
        }

        public string GenerateJwtToken(User account, DateTime expireTime)
        {
            var accountId = account.Id + "";

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, accountId),
                new Claim(ClaimTypes.Role, account.Role.Name),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtsecuritytoken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expireTime,
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(jwtsecuritytoken);
        }

        public async Task<User> VerifyTokenAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyJwt = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(keyJwt),
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                {
                    throw new SecurityTokenException("Invalid token");
                }
                var userId = Guid.Parse(userIdClaim.Value);
                var account = await _unitOfWork.Users.Find(s => s.Id == userId).Include(c => c.Pms).Include(c => c.Group).ThenInclude(c => c.Employees).ThenInclude(c => c.AdsAccounts).FirstOrDefaultAsync();
                if (account != null) 
                    return account;
                throw new SecurityTokenException();
            }
            catch (SecurityTokenExpiredException ex)
            {
                throw new SecurityTokenExpiredException("Token has expired.", ex);
            }
            catch (Exception ex)
            {
                throw new UnauthorizedAccessException("Token verification failed.", ex);
            }
        }
    }
}
