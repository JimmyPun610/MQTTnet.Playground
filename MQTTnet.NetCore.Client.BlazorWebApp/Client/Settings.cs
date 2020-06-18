using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MQTTnet.ClientLib;

namespace MQTTnet.NetCore.Client.BlazorWebApp.Client
{
    public class Settings
    {
        public string MqttWebSocket { get; set; }
        public string ApiHost { get; set; }
        public Settings(IConfiguration configuration)
        {
            var settings = configuration.GetSection("Settings");
            this.MqttWebSocket = settings.GetValue<string>("MqttWebSocket");
            this.ApiHost = settings.GetValue<string>("ApiHost");
        }
    }
}
