//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Options;
//using Pardakht.PardakhtPay.Application.Interfaces;

//namespace Pardakht.PardakhtPay.RestApi.Controllers
//{
//    [Produces("application/json")]
//    [Route("Token")]
//    [ApiExplorerSettings(IgnoreApi = true)]
//    public class TokenController : ControllerBase
//    {
//        //private readonly IUniversalApiService _universalApiService;
//        //private readonly MicroservicesSettings _microservicesSettings;

//        IAccountService _AccountService = null;

//        public TokenController(IAccountService accountService)
//        {
//            _AccountService = accountService;
//        }

//        private async Task<string> GetTokenFromUserManagementService(string username, string password)
//        {
//            var response = await _AccountService.Login(new Shared.Models.WebService.LoginModel()
//            {
//                Username = username,
//                Password = password
//            });

//            if (response.Success)
//            {
//                return response.Payload;
//            }

//            return null;
//        }

//        [HttpPost, AllowAnonymous]
//        public async Task<IActionResult> RequestToken([FromForm] LoginRequest request)
//        {
//            try
//            {
//                string generatedToken;
//                switch (request.grant_type)
//                {
//                    case "client_credentials":
//                        string authHeader = Request.Headers["Authorization"];
//                        if (authHeader != null && authHeader.StartsWith("Basic"))
//                        {
//                            //Extract credentials
//                            var encoding = Encoding.GetEncoding("iso-8859-1");
//                            var usernamePassword = encoding.GetString(Convert.FromBase64String(authHeader.Replace("Basic ", "")));
//                            generatedToken = await GetTokenFromUserManagementService(usernamePassword.Split(":")[0], usernamePassword.Split(":")[1]);
//                        }
//                        else
//                        {
//                            //Handle what happens if that isn't the case
//                            throw new Exception("The authorization header is either empty or isn't Basic.");
//                        }
//                        break;
//                    default:
//                        throw new Exception("Not Implemented yet");
//                }

//                return Ok(new AccessTokensResponse { access_token = generatedToken });
//            }
//            catch
//            {
//                return BadRequest("Request Token Error");
//            }

//        }
//    }

//    /// <summary>
//    /// Encapsulates fields for login request.
//    /// </summary>
//    /// <remarks>
//    /// See: https://www.oauth.com/oauth2-servers/access-tokens/
//    /// </remarks>
//    public class LoginRequest
//    {
//        [Required]
//        public string grant_type { get; set; }
//        public string username { get; set; }
//        public string password { get; set; }
//        public string refresh_token { get; set; }
//        public string scope { get; set; }

//        public string client_id { get; set; }
//        public string client_secret { get; set; }
//    }

//    /// <summary>
//    /// JWT successful response.
//    /// </summary>
//    /// <remarks>
//    /// See: https://www.oauth.com/oauth2-servers/access-tokens/access-token-response/
//    /// </remarks>
//    public class AccessTokensResponse
//    {
//        /// <summary>
//        /// Initializes a new instance of <seealso cref="AccessTokensResponse"/>.
//        /// </summary>
//        /// <param name="securityToken"></param>
//        public AccessTokensResponse(JwtSecurityToken securityToken)
//        {
//            access_token = new JwtSecurityTokenHandler().WriteToken(securityToken);
//            token_type = "Bearer";
//            expires_in = Math.Truncate((securityToken.ValidTo - DateTime.UtcNow).TotalSeconds);
//        }

//        public AccessTokensResponse()
//        {

//        }
//        public string access_token { get; set; }
//        public string refresh_token { get; set; }
//        public string token_type { get; set; }
//        public double expires_in { get; set; }
//    }


//    public class UserManagementResponse
//    {
//        public string Token { get; set; }
//        public string UserId { get; set; }
//    }
//}