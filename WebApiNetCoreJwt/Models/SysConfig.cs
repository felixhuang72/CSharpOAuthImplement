using System.Configuration;

namespace WebApiNetCore_Jwt.Models
{
    /// <summary>
    /// 系統設定
    /// </summary>
    public class SysConfig
    {
        public JwtConfig JwtIssueOptions { get; set; }
    }


    /// <summary>
    /// JWT 設定項目
    /// </summary>
    public class JwtConfig
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
    }
}