using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Mocsharp.WebApi.Formatters.Protobuf.Test
{
    public class ServerFixture : IDisposable
    {
        private IDisposable server;
        public int Port { get; set; } = 9000;
        public ServerFixture()
        {
            string baseAddress = $"http://localhost:{Port}/";

            // Start OWIN host 
            server = WebApp.Start<Startup>(url: baseAddress);

        }
        public void Dispose()
        {
            server.Dispose();
        }
    }
}
