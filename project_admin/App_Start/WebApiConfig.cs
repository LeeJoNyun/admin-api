using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;

namespace project_admin
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 구성 및 서비스
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            // Web API 경로
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Formatters.JsonFormatter.SupportedMediaTypes
           .Add(new MediaTypeHeaderValue("text/html"));
        }
    }
}
