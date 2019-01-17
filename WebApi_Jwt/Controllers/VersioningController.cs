using Microsoft.Web.Http;
using System;
using System.Web.Http;

//API版本化範例
//參考: https://dotblogs.com.tw/rainmaker/2017/03/12/130759

// API                              v1.0    v2.0    v2019-01-16.1.0-Beta    Comment
// ==============================   =====   =====   =====================   =============================
// version-test-a/test              OK      OK      OK                      各版本有各自方法，回傳值皆不同
// version-test-a/test_ex1          OK      OK      OK                      各版本皆執行 v1 的 test_ex1
// version-test-a/test_v2_ex1       -       OK      OK                      各版本皆執行 v2 的 test_v2_ex1
// version-test-a/getDatetime       -       -       OK                      getDatetime API 由 v2019-01-16.1.0-Beta 開始支援


//v1
namespace WebApiJwt.Controllers
{
    //v1,v2,v2019-01-16.1.0-Beta 皆可執行 VersioningExample1Controller 包含的所有 API
    [ApiVersion("1.0")]                 //初始版本
    [ApiVersion("2.0")]                 //使 v2.0 可執行 v1 的所有 APIs
    [ApiVersion("2019-01-16.1.0-Beta")] //使 v2019-01-16.1.0-Beta 可執行 v1 的所有 APIs
    [RoutePrefix("api/v{version:apiVersion}/version-test-a")]
    public class VersioningExample1Controller : ApiController
    {
        //v1 原始版本 (後續各版號執行各自版本的 API)
        //GET api/v1/version-test-a/test
        [Route("test")]
        public string Get() => "Hello world v1!";


        //適用於 v1, v2, v2019-01-16.1.0-Beta，皆執行同一方法
        //GET api/v1/version-test-a/test_ex1
        [Route("test_ex1")]
        public IHttpActionResult Get_Test()
        {
            return Ok("Hello test_ex1. This API supports from v1.");
        }
    }
}


//v2: 提供新 API: test_v2_ex1
namespace WebApiJwt.Controllers_v2
{
    [ApiVersion("2.0")]
    [ApiVersion("2019-01-16.1.0-Beta")] //使 v2019-01-16.1.0-Beta 版可執行 v2 的所有 APIs (包含 v2 獨有新功能)
    [RoutePrefix("api/v{version:apiVersion}/version-test-a")]
    public class VersioningExample1Controller : ApiController
    {
        //v2 改版後功能，各版號執行各自版本的 API
        //GET: api/v2/version-test-a/test
        [Route("test")]
        [MapToApiVersion("2.0")]
        public string Get() => "Hello world v2!";

        //v2 獨有的新功能
        //GET: api/v2/version-test-a/test_v2_ex1
        [Route("test_v2_ex1")]
        public string Get_V2Above() => "Hello world, test_v2_ex1. This API supports from v2.";
    }
}


//v3 (2019-01-16.1.0-Beta): 提供新 API: getDatetime
namespace WebApiJwt.Controllers_v2019_01_16_1_0_Beta
{
    [ApiVersion("2019-01-16.1.0-Beta")]
    [RoutePrefix("api/v{version:apiVersion}/version-test-a")]
    public class VersioningExample1Controller : ApiController
    {
        //v3 改版後功能，各版號執行各自版本的 API
        //GET: api/v2019-01-16.1.0-Beta/version-test-a/test
        [Route("test")]
        [MapToApiVersion("2019-01-16.1.0-Beta")]
        public string Get() => "Hello world v2019-01-16.1.0-Beta!";

        //v3 獨有功能
        //GET: api/v2019-01-16.1.0-Beta/version-test-a/getDatetime
        [Route("getDatetime")]
        public string Get_DateTime() => $"Current time: {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}";
    }
}