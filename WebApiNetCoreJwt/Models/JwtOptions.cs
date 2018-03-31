using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiNetCore_Jwt.Models
{
    /// <summary>
    /// Jwt 保留項，存放 Token 與相關憑證設定的保存
    /// </summary>
    public class JwtOptions
    {
        //private readonly IAudienceProvider _audienceProvider;

        /// <summary>
        /// iss (Issuer) - 此 JWT 的發行者
        /// </summary>
        /// <remarks>如果值中包含 : 那麼它就是一個 URI</remarks>
        public string Issuer { get; set; }

        /// <summary>
        /// sub (Subject) - 陳述主題的值
        /// </summary>
        /// <remarks>可以用來鑑別使用者</remarks>
        public string Subject { get; set; }

        /// <summary>
        /// aud (Audience) - 代表這個 Jwt 的接收對象
        /// </summary>
        /// <remarks>如果值中包含 : 那麼它就是一個 URI。 通常使用 URI 資源的聲明是有效的。例如，在 OAuth 中，接受者是授權伺服器。</remarks>
        public string Audience { get; set; }

        /// <summary>
        /// nbf (Not Before) - 有效起始時間 (即在此之前是無效的)
        /// </summary>
        public DateTime NotBefore => DateTime.UtcNow;

        /// <summary>
        /// iat (Issue At) - 發行時間
        /// </summary>
        public DateTime IssueAt => DateTime.UtcNow;

        /// <summary>
        /// exp (Expiration) - 過期時間
        /// </summary>
        public DateTime Expiration => IssueAt.Add(ValidFor);
        
        /// <summary>
        /// 有效時間 (預設60分鐘)
        /// </summary>
        public TimeSpan ValidFor { get; set; }// = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Func 委託方法(對象), 生成 jti (JWT ID) 唯一ID
        /// </summary>
        public Func<Task<string>> JtiGenerator => () => Task.FromResult(Guid.NewGuid().ToString("N"));

        /// <summary>
        /// 加密密鑰 (產生 Server 端簽名憑證時須使用)
        /// </summary>
        public string SecurityKey { get; set; }

        /// <summary>
        /// Base64 加密後的密鑰 (產生 Server 端簽名憑證時須使用)
        /// </summary>
        public string SecurityBase64Key { get; set; }

        /// <summary>
        /// 產生 Token 時使用的簽名憑證
        /// </summary>
        public SigningCredentials SigningCredentials { get; set; }
    }
}
