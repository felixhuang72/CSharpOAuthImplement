using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace WebApiJWT.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        [Authorize]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/auth
        [HttpPost, ActionName("Auth")]
        [Authorize(Roles = "SysAdmin")] //指定具有 SysAdmin 角色的帳號能存取
        public IEnumerable<string> GetWithRole()
        {
            return new string[] { "s_value1", "s_value2" };
        }

        // GET api/values/getUserInfo
        [HttpGet, ActionName("GetUserInfo")]
        [Authorize]
        public object Get_UserInfo()
        {
            try
            {
                var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var result = new
                    {
                        Account = identity.FindFirst("account").Value,
                        Name = identity.FindFirst("username").Value,
                        RoleNames = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList(),
                        TokenIssueDate = UnixTimeStampToDateTime(double.Parse(identity.FindFirst("nbf").Value)).ToUniversalTime().AddHours(8),
                        TokenExpiredDate = UnixTimeStampToDateTime(double.Parse(identity.FindFirst("exp").Value)).ToUniversalTime().AddHours(8),
                    };
                    return result;
                }
                else
                {
                    return "No data";
                }
            }
            catch { return "No data"; }
        }



        // GET api/values/5        
        public string Get(int id)
        {
            return id.ToString();
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }


        /// <summary>
        /// 將 Unix 的時間戳記轉換為 C# 的時間
        /// </summary>
        /// <param name="unixTimeStamp">Unix 的時間戳記</param>
        /// <returns></returns>

        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}