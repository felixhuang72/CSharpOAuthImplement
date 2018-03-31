using System.Configuration;

namespace WebApiJoseJWT.Models
{

    /// <summary>
    /// 系統參數
    /// </summary>
    public class SysConfig
    {
        /// <summary>
        /// Jwt Token 簽發者
        /// </summary>
        public string JwtIssuer { get; set; }

        /// <summary>
        /// Jwt Token 憑證加密密鑰 (明碼字串)
        /// </summary>
        public string JwtPlainSecret { get; set; }

        /// <summary>
        /// Jwt Token 憑證加密密鑰 (Based64 字串)
        /// </summary>
        public string JwtBase64Secret { get; set; }

        /// <summary>
        /// Jwt Token 有效時間 (單位: 分)
        /// </summary>
        public int JwtExpiredTimer { get; set; }


        public SysConfig()
        {
            JwtIssuer = ConfigurationManager.AppSettings["JwtIssuer"].ToString();
            JwtPlainSecret = ConfigurationManager.AppSettings["JwtPlainSecret"].ToString();
            JwtBase64Secret = ConfigurationManager.AppSettings["JwtBase64Secret"].ToString();
            JwtExpiredTimer = int.Parse(ConfigurationManager.AppSettings["JwtExpiredTimer"]);
        }
    }
}