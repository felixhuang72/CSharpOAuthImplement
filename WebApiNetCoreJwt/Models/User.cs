using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApiNetCore_Jwt.Models
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


    /// <summary>
    /// 使用者登入 ViewModel
    /// </summary>
    public class SigninVM
    {
        /// <summary>
        /// 帳號
        /// </summary>
        [Required(ErrorMessage = "請輸入帳號。")]
        public string Account { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        [Required(ErrorMessage = "請輸入密碼。")]
        public string Password { get; set; }

        /// <summary>
        /// 介接服務的 Client ID
        /// </summary>
        [Required]
        public string ClientId { get; set; }
    }
}
