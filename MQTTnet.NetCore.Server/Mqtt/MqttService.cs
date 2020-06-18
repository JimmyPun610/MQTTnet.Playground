using MQTTnet.AspNetCore;
using MQTTnet.Client.Receiving;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQTTnet.NetCore.Server.Mqtt
{
    public class MqttService :
         IMqttServerClientConnectedHandler,
         IMqttServerClientDisconnectedHandler,
         IMqttServerApplicationMessageInterceptor,
         IMqttServerConnectionValidator,
         IMqttApplicationMessageReceivedHandler,
         IMqttServerStartedHandler
    {
        IMqttServer Server;
        public void ConfigureMqttServerOptions(AspNetMqttServerOptionsBuilder options)
        {
            options.WithConnectionValidator(this);
            options.WithApplicationMessageInterceptor(this);
        }

        public void ConfigureMqttServer(IMqttServer mqtt)
        {
            this.Server = mqtt;
            mqtt.ClientConnectedHandler = this;
            mqtt.ClientDisconnectedHandler = this;
        }

        public Task HandleClientConnectedAsync(MqttServerClientConnectedEventArgs eventArgs)
        {
            return Task.Run(() => { });
        }

        public Task HandleClientDisconnectedAsync(MqttServerClientDisconnectedEventArgs eventArgs)
        {
            return Task.Run(() => { });
        }

        public Task HandleServerStartedAsync(EventArgs eventArgs)
        {
            return Task.Run(() => { });
        }

        public Task InterceptApplicationMessagePublishAsync(MqttApplicationMessageInterceptorContext context)
        {
            return Task.Run(() => { });
        }

        public Task ValidateConnectionAsync(MqttConnectionValidatorContext context)
        {
            return Task.Run(() => { });
        }

        public Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            return Task.Run(() => { });
        }
    }
}
