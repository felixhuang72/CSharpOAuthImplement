namespace WebApiJoseJWT.Models
{
    public class LoginVM
    {
        /// <summary>
        /// 帳號
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 登入密碼
        /// </summary>
        public string Password { get; set; }
    }
}