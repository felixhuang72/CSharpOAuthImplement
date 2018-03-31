using System.Collections.Generic;

namespace WebApiJoseJWT.Models
{
    /// <summary>
    /// 使用者資訊
    /// </summary>
    public class User
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 角色清單
        /// </summary>
        public List<string> Roles { get; set; }
    }


    /// <summary>
    /// 角色
    /// </summary>
    public class Role
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// 角色名稱
        /// </summary>
        public string RoleName { get; set; }
    }
}
