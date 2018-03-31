using Jose;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using WebApiJoseJWT.Models;

namespace WebApiJoseJWT.Controllers
{    
    public class TokenController : ApiController
    {
        private readonly Database _db;
        private readonly SysConfig config;

        public TokenController()
        {
            _db = new Database();
            config = new SysConfig();
        }

        public object TextEncodings { get; private set; }

        /// <summary>
        /// 帳號驗證並回傳 JWT Token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public HttpResponseMessage Post(LoginVM model)
        {
            string sResult = "";
            var result = new HttpResponseMessage(HttpStatusCode.OK);


            if (_db.Users.Any(x => x.Account == model.Account && x.Password == model.Password))
            {
                User user = _db.Users.Single(x => x.Account == model.Account && x.Password == model.Password);
                var AuthData = new JwtAuthObject()
                {
                    Account = user.Account,
                    Name = user.Name,
                    Roles = user.Roles,
                    TimeStamp = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                };

                //若 Secret 為未經 Base64 編碼過的原始字串
                //var secret = Encoding.UTF8.GetBytes(config.JwtPlainSecret);
                //若 Secret 為已經 Base64 編碼過的字串
                var secret = Convert.FromBase64String(config.JwtBase64Secret);

                result = new HttpResponseMessage(HttpStatusCode.OK);
                sResult = JWT.Encode(AuthData, secret, JwsAlgorithm.HS256);
                result.Content = new StringContent(sResult);

                result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            }
            else
            {
                sResult = "Account or password is invalid.";
                result = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                result.Content = new StringContent(sResult);

                result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            }

            return result;
        }
    }
}
