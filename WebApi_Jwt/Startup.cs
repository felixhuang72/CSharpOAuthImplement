using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Linq;
using System.Text;
using System.Web.Http;
using WebApiJWT.Jwt;
using WebApiJWT.Models;

//STEP#1: 定義 App 啟動後「發放 JWT Token 機制相關設定」及「使用 JWT 進行 token 權限 ([Authorize]) 驗證」設定

[assembly: OwinStartup(typeof(WebApiJWT.Startup))]
namespace WebApiJWT
{
    public class Startup
    {
        public SysConfig SysConfig;
        public Database _db;

        public Startup()
        {
            SysConfig = new SysConfig();
            _db = new Database();
        }

        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();

            // Web API routes
            config.MapHttpAttributeRoutes();
            ConfigureOAuth(app);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
        }


        public void ConfigureOAuth(IAppBuilder app)
        {
            #region 發放 JWT Token 機制相關設定

            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                //允許不安全的連線方式 (一般，如開發階段: true；SSL: false)
                AllowInsecureHttp = true,
                //發放 Token 的位置 (可不用另建 Controller，直接 [POST] http://localhost:<port>/oauth/token 即可存取)
                TokenEndpointPath = new PathString("/oauth/token"),
                //設定 Token 有效期限 (預設 30 分鐘)
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(SysConfig.JwtExpiredTimer),
                //指定資訊驗證的方式 (驗證內容: 存取系統的 ClientID、申請 Token 的使用者帳號/密碼…等)
                Provider = new CustomOAuthProvider(),
                //指定 Token 內容格式，並產生 JWT 格式的 Token。(藉此取代 .NET 採用 DPAPI 產生的 Access Token)
                AccessTokenFormat = new CustomJwtFormat(SysConfig.JwtIssuer),
            };
            // OAuth 2.0 Bearer Access Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);

            /* JWT 產出執行順序：
             * 
             * 1. Client 端輸入帳號、密碼、ClientId 資訊後進行 POST
             * ==== 驗證階段 ====
             * 2. 進入 CustomOAuthProvider.ValidateClientAuthentication() 進行 Audience 驗證
             * 3. 進入 CustomOAuthProvider.GrantResourceOwnerCredentials() 進行 Account/Password 身分驗證
             *          通過驗證後，建立 Identity 資訊，透過 AuthenticationTicket 傳遞給下一階段
             
             * ==== 建立 Token ==== 
             * 4. 進入 CustomJwtFormat.Protect()，透過 AuthenticationTicket 取得 JWT Token payload 所需資訊，產出 Token 後回傳
             * */

            #endregion

            #region 設定使用 JWT 進行 token 權限驗證

            //Token 發放者(服務)名稱 (可為 URI 或 字串名稱，須完全一致才會通過驗證)
            var issuer = SysConfig.JwtIssuer;
            //產生 Token 的密鑰
            //var secret = Encoding.UTF8.GetBytes(SysConfig.JwtPlainSecret);            //若 Secret 為未經任何編碼的明碼字串
            var secret = TextEncodings.Base64Url.Decode(SysConfig.JwtBase64Secret);     //若 Secret 是已進行 Base64 編碼後的字串


            //允許使用 API 的其他系統 ClientID
            //至 DB 取出有效的 AudienceID 清單
            var audiences = _db.Audiences;
            string[] audience = audiences == null || audiences.Count() == 0 ? new string[0] : audiences.Select(x => x.AudienceId).ToArray();


            //套用 [Authorize] 標籤的 Api 控制器將透過下面方法，使用 JWT 進行存取驗證
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active
                   ,
                    AllowedAudiences = audience
                   ,
                    IssuerSecurityKeyProviders = new IIssuerSecurityKeyProvider[]
                    {
                        new  SymmetricKeyIssuerSecurityKeyProvider(issuer, secret)
                    }
                });

            #endregion
        }
    }
}
