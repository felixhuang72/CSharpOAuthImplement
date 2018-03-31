using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApiNetCore_Jwt.Models;

namespace WebApiNetCore_Jwt.Controllers
{
    [Route("api/[controller]")]
    public class OAuthController : Controller
    {
        private Database db;
        private readonly JwtOptions _jwtOptions;

        public OAuthController(IOptions<JwtOptions> JwtOptions)
        {
            _jwtOptions = JwtOptions.Value;
            db = new Database();
        }

        /// <summary>
        /// 取得 JWT 資訊
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        //POST api/OAuth
        [HttpPost]
        public async Task<IActionResult> Post(SigninVM model)
        {
            User _user = new User();

            #region #1 帳密資訊驗證

            if (model.ClientId == null)
            {
                return new BadRequestObjectResult("invalid_clientid: Please provide 'Client ID'.");
            }

            var audience = db.Audiences.Where(x => x.AudienceId == model.ClientId);
            if (audience == null)
            {
                return new BadRequestObjectResult($"invalid_clientid: Client ID '{model.ClientId}' is isvalid.");
            }

            //取得 Jwt 過程中的帳號與密碼驗證
            if (string.IsNullOrEmpty(model.Account) || string.IsNullOrEmpty(model.Password))
            {
                return new BadRequestObjectResult($"invalid_clientid: Account or password is incorrect.");
            }
            else
            {
                _user = db.Users.Single(x => x.Account == model.Account && x.Password == model.Password);

                //使用者資訊驗證                
                if (_user == null)
                {
                    return new BadRequestObjectResult($"invalid_clientid: Account or password is incorrect.");
                }
            }
            #endregion

            #region #2 資訊驗證成功，建立 Identity

            //取出帳號所屬角色資訊
            List<Claim> role_claim = new List<Claim>();
            if (_user.Roles != null && _user.Roles.Count() > 0)
            {
                foreach (var r in _user.Roles)
                {
                    role_claim.Add(new Claim(ClaimTypes.Role, r));
                }
            }

            //建立 Identity.Claim
            ClaimsIdentity identity = new ClaimsIdentity(new GenericIdentity(model.Account, "Token"),
                    new[]
                    {
                        //new Claim("IsValidAuthorized", "true"),
                        new Claim("Uid", _user.Account),
                        new Claim("Account", _user.Account),
                        new Claim("Username", _user.Name),
                    }.Concat(role_claim));

            #endregion

            #region #3 發放 Token

            //設定 Token 記載內容
            double unixEpocDate = Math.Round((_jwtOptions.IssueAt.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Aud, model.ClientId),
                new Claim(JwtRegisteredClaimNames.Sub, model.Account),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, unixEpocDate.ToString(), ClaimValueTypes.Integer64),
            };
            claims.AddRange(identity.Claims);

            //生成 Jwt token, 並進行編碼
            var jwt = new JwtSecurityToken(
                    issuer: _jwtOptions.Issuer,
                    claims: claims,
                    notBefore: _jwtOptions.NotBefore,
                    expires: _jwtOptions.Expiration,
                    signingCredentials: _jwtOptions.SigningCredentials
                );
            string encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            //回傳 Token
            var response = new { access_token = encodedJwt, token_type = "bearer", expires_in = (int)_jwtOptions.ValidFor.TotalSeconds };
            return new OkObjectResult(response);

            #endregion

        }
    }
}