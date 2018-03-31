using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiNetCore_Jwt.Models
{
    /// <summary>
    /// 授予介接的系統資訊
    /// </summary>
    public class Audience
    {
        /// <summary>
        /// 授予介接的系統代碼
        /// </summary>
        public string AudienceId { get; set; }

        /// <summary>
        /// 授予介接的系統名稱
        /// </summary>
        public string AudienceName { get; set; }

        /// <summary>
        /// 是否仍允許介接
        /// </summary>
        public bool IsActive { get; set; }

        public Audience()
        {
            IsActive = true;
        }
    }
}