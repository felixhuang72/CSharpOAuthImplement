using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApiJWT.Models;

namespace WebApiJWT.Jwt
{
    //STEP#2: 建立 JWT Token 核發前的 ClientId 及存取使用者的相關資訊驗證

    /// <summary>
    /// Token (JWT) 核發前的資訊驗證
    /// </summary>
    public class CustomOAuthProvider : OAuthAuthorizationServerProvider
    {
        private Database _db;
        public CustomOAuthProvider()
        {
            _db = new Database();
        }

        /// <summary>
        /// 驗證請求端服務所提供的 ClientID 是否在授權存取系統 (Audience) 清單中
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId = string.Empty;
            string clientSecret = string.Empty;
            string symmetricKeyAsBase64 = string.Empty;

            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            if (context.ClientId == null)
            {
                context.SetError("invalid_clientid", "Please provide 'Client ID'.");
                return Task.FromResult<object>(null);
            }

            Audience audience = _db.Audiences.Where(x => x.AudienceId == context.ClientId).FirstOrDefault();
            if (audience == null)
            {
                context.SetError("invalid_clientid", string.Format("Client ID '{0}' is invalid", context.ClientId));
                return Task.FromResult<object>(null);
            }

            context.Validated();
            return Task.FromResult<object>(null);  //接下來將繼續執行 GrantResourceOwnerCredentials()
        }


        /// <summary>
        /// 進行存取者身分驗證
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //跨域存取限制設定
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
            User user = new User();


            //取得 Jwt 過程中的帳號與密碼，可在此進行驗證
            if (string.IsNullOrEmpty(context.UserName) || string.IsNullOrEmpty(context.Password))
            {
                context.SetError("invalid_grant", "Account or password is incorrect.");
                return Task.FromResult<object>(null);
            }
            else
            {
                //使用者資訊驗證
                user = _db.Users.Where(x => x.Account == context.UserName).FirstOrDefault();
                if (user == null)
                {
                    context.SetError("invalid_grant", "Account or password is incorrect.");
                    return Task.FromResult<object>(null);
                }
                else
                {
                    //驗證密碼
                    if (user.Password == context.Password)
                    {
                        //通過驗證                        
                    }
                    else
                    {
                        context.SetError("invalid_grant", "Account or password is incorrect.");
                        return Task.FromResult<object>(null);
                    }
                }
            }

            //建立 Identity 資訊
            var identity = new ClaimsIdentity("JWT");
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim("sub", context.UserName));
            identity.AddClaim(new Claim("account", user.Account));
            identity.AddClaim(new Claim("username", user.Name));
            identity.AddClaim(new Claim("powered_by", "ASP.NET WebApi2"));

            //若有角色資訊，可在此將角色資訊寫入
            if (user.Roles != null)
            {
                user.Roles.ForEach(role =>
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                });
            }

            var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    {
                         "audience", context.ClientId ?? string.Empty
                    }
                });

            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);

            return Task.FromResult<object>(null);   //接續執行 CustomJwtFormat.Protect()，建立並發放 Token
        }
    }
}