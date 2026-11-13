using System.Web.Http;
using System.Web.Http.Cors;

namespace StudentRegistration.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Enable cors
            var cors = new EnableCorsAttribute(
                "https://localhost:44366",
                "*",
                "*"
            );

            config.EnableCors(cors);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Return JSON instead of XML
            config.Formatters.Remove(
                config.Formatters.XmlFormatter
            );
        }
    }
}
