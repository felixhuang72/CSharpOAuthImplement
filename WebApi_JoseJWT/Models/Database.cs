using System.Collections.Generic;

namespace WebApiJoseJWT.Models
{
    /// <summary>
    /// DB 模擬
    /// </summary>
    public class Database
    {
        /// <summary>
        /// 使用者
        /// </summary>
        public List<User> Users { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public List<Role> Roles { get; set; }


        /// <summary>
        /// 授予介接的系統清單
        /// </summary>
        public List<Audience> Audiences { get; set; }

        public Database()
        {
            Users = new List<User>()
            {
                new User(){ Account = "felix", Password = "abc123456", Name = "Felix Huang", Roles = new List<string>{ "SysAdmin", "Admin" } },
                new User(){ Account = "keira", Password = "abc123456", Name = "Keira Hsiao", Roles = new List<string>{ "Admin" } },
                new User(){ Account = "wayne", Password = "abc123456", Name = "Wayne Chen", Roles = new List<string>{ "Guest" } },
            };
            
            Roles = new List<Role>()
            {
                new Role(){ RoleId = "SysAdmin", RoleName = "SystemAdmin" },
                new Role(){ RoleId = "Admin", RoleName = "Administrator" },
                new Role(){ RoleId = "Guest", RoleName = "Guest"},
            };

            Audiences = new List<Audience>()
            {
                new Audience(){ AudienceId = "AUD20180328001", AudienceName="Client A", IsActive = true },
                new Audience(){ AudienceId = "AUD20180328002", AudienceName="Client B", IsActive = true },                
            };
        }
    }
}
