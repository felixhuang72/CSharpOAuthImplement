using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Web.Http;

using WebApiJwt;

namespace WebApiJWT
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //configure our application to return camel -case JSON(thisIsCamelCase), instead of the default pascal -case (ThisIsPascalCase)
            var formatters = GlobalConfiguration.Configuration.Formatters;
            var jsonFormatter = formatters.JsonFormatter;
            var settings = jsonFormatter.SerializerSettings;
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
