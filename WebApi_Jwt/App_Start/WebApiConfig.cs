using Microsoft.Web.Http;
using Microsoft.Web.Http.Routing;
using System.Web.Http;
using System.Web.Http.Routing;

namespace WebApiJwt
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 設定和服務
            var constraintResolver = new DefaultInlineConstraintResolver() { ConstraintMap = { ["apiVersion"] = typeof(ApiVersionRouteConstraint) } };

            // Web API 路由
            config.MapHttpAttributeRoutes(constraintResolver);

            //API Versioning 設定
            config.AddApiVersioning(o =>
            {
                //設 true，會將 API 支援的版本列在 header 中
                o.ReportApiVersions = true;
                //設定 API 預設的版本
                o.DefaultApiVersion = new ApiVersion(2, 0);
                //若使用時沒特別指定版本的，true: 會使用 DefaultApiVersion 設定的版本。
                o.AssumeDefaultVersionWhenUnspecified = true;
            });
            //API Versioning 建議使用 Attribute Routing, 因此不另外設路由對映



            config.Routes.MapHttpRoute(
                name: "ApiCustomRoute",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
