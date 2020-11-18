using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet.AspNetCore;
using MQTTnet.AspNetCore.Extensions;

namespace MQTTnet.NetCore.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddSingleton<Mqtt.MqttService>();
            services.AddControllers();
            //Add MQTT Service
            services
             .AddHostedMqttServer(mqttServer => mqttServer.WithoutDefaultEndpoint())
             .AddMqttConnectionHandler()
             .AddConnections();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(
                options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
            );
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //Setup mqtt endpoints for websocket (localhost:{port}/mqtt}
                //.NET CORE 3.1 Approach
                //endpoints.MapMqtt("/mqtt");
                //.NET 5 Approach
                app.UseEndpoints(endpoints => { endpoints.MapConnectionHandler<MqttConnectionHandler>("/mqtt", config=>
                    {
                        config.WebSockets.SubProtocolSelector = protocolList => protocolList.FirstOrDefault() ?? string.Empty;
                    }); 
                });
            });
            app.UseMqttServer(server => app.ApplicationServices
                                            .GetRequiredService<Mqtt.MqttService>()
                                            .ConfigureMqttServer(server));
        }
    }
}
