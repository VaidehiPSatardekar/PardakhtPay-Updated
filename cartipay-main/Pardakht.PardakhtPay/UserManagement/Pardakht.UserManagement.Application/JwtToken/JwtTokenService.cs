using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Pardakht.PardakhtPay.Enterprise.Utilities.Infrastructure.Helpers;
using Pardakht.UserManagement.Domain.Platform;
using Pardakht.UserManagement.Shared.Models.Configuration;
using Pardakht.UserManagement.Shared.Models.Infrastructure;

namespace Pardakht.UserManagement.Application.JwtToken
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtIssuerOptions config;
        private readonly IPlatformManager platformManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtTokenService(IOptions<JwtIssuerOptions> config, IPlatformManager platformManager, IHttpContextAccessor httpContextAccessor)
        {
            this.platformManager = platformManager;
            _httpContextAccessor = httpContextAccessor;
            this.config = config.Value;
        }

        public async Task<string> CreateToken(Shared.Models.WebService.StaffUser staffUser, string platformGuid)
        {
            var context = _httpContextAccessor.HttpContext;
            var accountContext = HttpContextHelper.GetCustomHeader(context, "account-context");
            accountContext = accountContext.ToLower();

            var platform = await platformManager.GetByPlatformGuid(platformGuid);
            if (platform == null)
            {
                throw new Exception("Platform not found");
            }

            var key = Encoding.ASCII.GetBytes(platform.JwtKey);
            var userData = Newtonsoft.Json.JsonConvert.SerializeObject(new JwtUserData
            {
                PlatformGuid = platformGuid,
                TenantGuid = staffUser.TenantGuid,
                ParentAccountId = staffUser.ParentAccountId,
                TenantUid = accountContext,
                BrandId = staffUser.BrandId,
                UserType = (int)staffUser.UserType
            });

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, staffUser.Username ?? ""),
                    new Claim(ClaimTypes.NameIdentifier, staffUser.AccountId),
                    new Claim(ClaimTypes.UserData, userData)
                }),
                Issuer = config.Issuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            if (staffUser.Permissions != null)
            {
                foreach (var permission in staffUser.Permissions)
                {
                    tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, permission));
                }
            }

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
