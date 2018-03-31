using System;
using System.Collections.Generic;

namespace WebApiJoseJWT.Models
{
    /// <summary>
    /// JWT Auth Class
    /// </summary>
    //[Serializable]
    public class JwtAuthObject
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string Account { get; set; }
        
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 角色清單
        /// </summary>
        public List<string> Roles { get; set; }
        
        /// <summary>
        /// Token 簽發時間戳記
        /// </summary>
        public string TimeStamp { get; set; }
    }
}