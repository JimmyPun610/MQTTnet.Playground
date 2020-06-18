
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQTTnet.ClientLib
{
    internal class MqttConnectedHandler : IMqttClientConnectedHandler
    {
        Action<MqttClientConnectedEventArgs> _connectedAction;
        public MqttConnectedHandler(Action<MqttClientConnectedEventArgs> connectedAction)
        {
            _connectedAction = connectedAction;
        }

        public async Task HandleConnectedAsync(MqttClientConnectedEventArgs eventArgs)
        {
            _connectedAction?.Invoke(eventArgs);
        }
    }
}
