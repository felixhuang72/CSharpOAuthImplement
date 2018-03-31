using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace WebApiNetCore_Jwt.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private IHttpContextAccessor _accessor;

        //DI
        public ValuesController(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }


        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/auth        
        [HttpGet("Auth")]
        [Authorize(Policy = "SuperAuthorize")]
        public IEnumerable<string> GetWithRole()
        {
            return new string[] { "s_value1", "s_value2" };
        }

        [Authorize]
        [HttpGet("GetUserInfo")]
        public IActionResult GetUserInfo()
        {
            try
            {
                if (_accessor.HttpContext.User.Identity is ClaimsIdentity identity)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var result = new
                    {
                        Account = identity.FindFirst("account").Value,
                        Name = identity.FindFirst("username").Value,
                        RoleNames = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList(),
                        TokenIssueDate = UnixTimeStampToDateTime(double.Parse(identity.FindFirst("nbf").Value)).ToUniversalTime(),
                        TokenExpiredDate = UnixTimeStampToDateTime(double.Parse(identity.FindFirst("exp").Value)).ToUniversalTime(),
                    };
                    return new OkObjectResult(result);
                }
                else
                {
                    return new OkObjectResult(new { msg = "No data" });
                }
            }
            catch { return new OkObjectResult(new { msg = "No data" }); }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
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
