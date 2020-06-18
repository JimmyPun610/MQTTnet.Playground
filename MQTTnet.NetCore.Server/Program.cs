using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet.AspNetCore;

namespace MQTTnet.NetCore.Server
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
                    var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())  //location of the exe file
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    IConfigurationRoot configuration = builder.Build();
                    IConfigurationSection kestrelSettings = configuration.GetSection("KestrelSettings");
                    int mqttPipeline = kestrelSettings.GetValue<int>("MQTTPipeline");
                    int httpPipeline = kestrelSettings.GetValue<int>("HttpPipeline");
                    int httpsPipeline = kestrelSettings.GetValue<int>("HttpsPipeline");
                    //https://github.com/chkr1011/MQTTnet/wiki/Server Server setup documnetation
                    //https://forums.asp.net/t/2162724.aspx?Read+appsettings+json+and+use+to+toggle+function+in+program+cs using appsettings.json in program.cs
                    webBuilder
                    .UseKestrel(o =>
                    {
                        o.ListenAnyIP(mqttPipeline, l => l.UseMqtt());
                        o.ListenAnyIP(httpPipeline);
                        o.ListenAnyIP(httpsPipeline, l => l.UseHttps());
                    })
                    .UseStartup<Startup>();
                });
    }
}
