using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using WebApiJWT.Models;

namespace WebApiJWT.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values        
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
        [HttpGet, Route("api/values/getUserInfo")]
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


        /// <summary>
        /// 手動驗證 JWT Token
        /// </summary>
        /// <param name="token">Token 字串</param>
        /// <returns></returns>
        //POST api/values/parseToken
        [HttpPost, Route("api/values/parseToken")]
        public IHttpActionResult Post_JwtTokenValidTest([FromBody]Form_Token model)
        {
            object result = null;
            if (!string.IsNullOrEmpty(model.Token))
            {
                result = TokenValidate(model.Token, model.ValidateLifetime);
            }
            return Ok(result);
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


        public class Form_Token
        {
            /// <summary>
            /// Jwt Token 字串
            /// </summary>
            public string Token { get; set; }

            /// <summary>
            /// 是否要驗證 Token 生命週期
            /// </summary>
            public bool ValidateLifetime { get; set; }
        }


        /// <summary>
        /// 驗證 Jwt Token
        /// </summary>
        /// <param name="jwtInput">Token 內容</param>
        /// <param name="validateLifetime">是否驗證生命週期</param>
        /// <returns>解析後的 Token 內容或錯誤訊息</returns>
        /// <remarks>.net core: https://dotblogs.com.tw/libtong/2018/01/11/121549</remarks>
        public string TokenValidate(string jwtInput, bool validateLifetime = true)
        {
            string output = "";

            try
            {
                var jwtHandler = new JwtSecurityTokenHandler();
                var readableToken = jwtHandler.CanReadToken(jwtInput);

                if (readableToken != true)
                {
                    output = "the token does not seem to be in a proper JWT format";
                }
                else
                {
                    var token = jwtHandler.ReadJwtToken(jwtInput);
                    var headers = token.Header;
                    var jwtHeader = "{";
                    foreach (var h in headers)
                    {
                        jwtHeader += '"' + h.Key + "\":\"" + h.Value + "\",";
                    }
                    jwtHeader += "}";
                    output = "Header:\r\n" + JToken.Parse(jwtHeader).ToString(Formatting.Indented);


                    //Extract the payload of the JWT
                    var claims = token.Claims;
                    var jwtPayload = "{";
                    foreach (Claim c in claims)
                    {
                        jwtPayload += '"' + c.Type + "\":\"" + c.Value + "\",";
                    }
                    jwtPayload += "}";
                    output += "\r\nPayload:\r\n" + JToken.Parse(jwtPayload).ToString(Formatting.Indented);


                    //Token 驗證
                    SysConfig sysConfig = new SysConfig();
                    Database _db = new Database();

                    //至 DB 取出有效的 AudienceID 清單
                    var audiences = _db.Audiences;
                    string[] audience = audiences == null || audiences.Count() == 0 ? new string[0] : audiences.Select(x => x.AudienceId).ToArray();

                    //加密secret
                    string symmetricKeyAsBase64 = sysConfig.JwtBase64Secret;
                    var keyByteArray = TextEncodings.Base64Url.Decode(symmetricKeyAsBase64);
                    var securityKey = new SymmetricSecurityKey(keyByteArray);

                    TokenValidationParameters paras = new TokenValidationParameters()
                    {
                        ValidateAudience = true,
                        ValidAudiences = audience,
                        ValidIssuer = sysConfig.JwtIssuer,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = securityKey,
                        ValidateLifetime = validateLifetime,                        
                        ClockSkew = TimeSpan.Zero,
                    };

                    ClaimsPrincipal _ValidateResult = jwtHandler.ValidateToken(jwtInput, paras, out SecurityToken validatedToken);

                    //萃取使用者資訊
                    var UserInfo = new
                    {
                        Account = _ValidateResult.FindFirst("Account").Value,
                        Name = _ValidateResult.FindFirst("username").Value,
                        RoleNames = _ValidateResult.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList(),
                        TokenIssueDate = UnixTimeStampToDateTime(double.Parse(_ValidateResult.FindFirst("nbf").Value)).ToUniversalTime().AddHours(8),
                        TokenExpiredDate = UnixTimeStampToDateTime(double.Parse(_ValidateResult.FindFirst("exp").Value)).ToUniversalTime().AddHours(8),
                    };
                }
            }
            catch (Exception ex)
            {
                output = ex.Message.ToString();
            }
            return output;
        }
    }
}