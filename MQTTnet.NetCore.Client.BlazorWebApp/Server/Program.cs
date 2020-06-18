using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MQTTnet.NetCore.Client.BlazorWebApp.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                      .UseKestrel(o =>
                      {
                          o.ListenAnyIP(15000);
                          o.ListenAnyIP(15001, l => l.UseHttps());
                      })
                    .UseStartup<Startup>();
                });
    }
}
