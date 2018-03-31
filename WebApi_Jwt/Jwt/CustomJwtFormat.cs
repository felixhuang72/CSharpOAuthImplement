using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using WebApiJWT.Models;

namespace WebApiJWT.Jwt
{
    //STEP#3: 定義 JWT 內容格式

    /// <summary>
    /// 實作了 “ISecureDataFormat<AuthenticationTicket>” 介面定義的方法, JWT 將於 “Protect” 方法中產出。
    /// </summary>
    public class CustomJwtFormat : ISecureDataFormat<AuthenticationTicket>
    {
        private const string AudiencePropertyKey = "audience";
        private readonly string _issuer = string.Empty;
        private readonly Database _db;

        /// <summary>
        /// 建構式
        /// </summary>
        /// <param name="issuer">Token 核發服務的名稱或 URI</param>
        public CustomJwtFormat(string issuer)
        {
            _issuer = issuer;
            _db = new Database();
        }

        /***** Protect() 方法主要內容：
         * 
         * 1. 自 Authentication 的 Ticket 中取得 audience (client id)，並依此取得此 Audience 的相關資訊
         * 2. 自相關的系統設定中建立 Symmetric key，將其進行 Base64 decode 後產生 byte[]，由此建立 JWT Token 所需
         *    的 HMAC265 signing key
         * 3. 準備要簽入至 JWT payload 內的資料 (如: issuer, audience, user claims, issue date, expiry date...etc)
         * 4. 最後，建立 JWT Token
         *
         ******/

        public string Protect(AuthenticationTicket data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            //#1 驗證請求端系統
            //判斷申請 Token 的請求端是否有傳遞合法的 AudienceId (ClientId)
            string audienceId = data.Properties.Dictionary.ContainsKey(AudiencePropertyKey) ? data.Properties.Dictionary[AudiencePropertyKey] : null;
            if (string.IsNullOrWhiteSpace(audienceId)) throw new InvalidOperationException("Do not provide 'Client Id', access deny."/*"AuthenticationTicket.Properties does not include audience"*/);
            Audience audience = _db.Audiences.Where(x => x.AudienceId == audienceId).FirstOrDefault();
            if (audience == null)
            {
                throw new InvalidOperationException("Invalid server, access deny.");
            }
            else if (!audience.IsActive)
            {
                throw new InvalidOperationException("The access right of this client is already out-of-date, access deny.");
            }

            //#2 請求端系統驗證合法，產出 Token
            SysConfig config = new SysConfig();

            //取得建立 Token 憑證時須使用的加密密鑰

            //=== 若 Config.JwtSecret 為未編碼的一般字串 ===
            //var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.JwtPlainSecret));

            //=== 若 Config.JwtSecret 是已經過 Base64 編碼後的字串 ===            
            string symmetricKeyAsBase64 = config.JwtBase64Secret;
            var keyByteArray = TextEncodings.Base64Url.Decode(symmetricKeyAsBase64); //Convert.FromBase64String(symmetricKeyAsBase64);
            var securityKey = new SymmetricSecurityKey(keyByteArray);

            //建立 Token 憑證
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Token 簽發時間
            var issued = data.Properties.IssuedUtc;
            //Token 預計註銷時間
            var expires = data.Properties.ExpiresUtc;
            //產出 Token
            var token = new JwtSecurityToken(_issuer, audienceId, data.Identity.Claims, issued.Value.UtcDateTime, expires.Value.UtcDateTime, credentials);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.WriteToken(token);

            //執行至此，代表登入並驗證成功，可在 JWT 回傳前做些操作 (ex: log...etc)            

            return jwt;
        }

        public AuthenticationTicket Unprotect(string protectedText)
        {
            throw new NotImplementedException();
        }
    }
}