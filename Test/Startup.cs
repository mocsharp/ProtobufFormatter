using Owin;
using System.Web.Http;

namespace Mocsharp.WebApi.Formatters.Protobuf.Test
{
    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            config.Formatters.Clear();
            config.Formatters.Add(new ProtoBufFormatter());

            config.EnableSystemDiagnosticsTracing();
            appBuilder.UseWebApi(config);
        }
    }
}
