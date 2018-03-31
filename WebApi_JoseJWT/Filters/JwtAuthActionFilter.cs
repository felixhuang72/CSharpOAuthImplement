using Jose;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebApiJoseJWT.Models;

namespace WebApiJoseJWT.Filters
{
    /// <summary>
    /// 進入 Action 前先行進行 Jwt 有效性驗證
    /// </summary>
    public class JwtAuthActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            SysConfig config = new SysConfig();

            if (actionContext.Request.Headers.Authorization == null ||
                actionContext.Request.Headers.Authorization.Scheme.ToLower() != "bearer")
            {
                setErrorResponse(actionContext, "Invalid authorization");
            }
            else
            {
                try
                {
                    var jwtObject = JWT.Decode<JwtAuthObject>(
                                        actionContext.Request.Headers.Authorization.Parameter,
                                        //使用明碼 secret 字串
                                        //Encoding.UTF8.GetBytes(config.JwtPlainSecret),
                                        //使用經過 Base64 編碼後的字串
                                        Convert.FromBase64String(config.JwtBase64Secret),
                                        JwsAlgorithm.HS256);

                    //時間逾時問題處理(TimeStamp)
                    DateTime tmp_TimeStamp = Convert.ToDateTime(jwtObject.TimeStamp);
                    TimeSpan ts = (DateTime.Now - tmp_TimeStamp);

                    if (Convert.ToInt32(ts.TotalSeconds) > config.JwtExpiredTimer * 60)
                    {
                        setErrorResponse(actionContext, "The token has already time out.");
                        return;
                    }

                    //重複登入問題處理(Token)
                    //與DB比對
                }
                catch (Exception ex)
                {
                    setErrorResponse(actionContext, ex.Message);
                }
            }
        }

        /// <summary>
        /// 實作 Action 篩選後發現錯誤時的回應內容
        /// </summary>
        /// <param name="actionContext"></param>
        /// <param name="message"></param>
        private static void setErrorResponse(HttpActionContext actionContext, string message)
        {
            var response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, message);
            actionContext.Response = response;
        }
    }
}