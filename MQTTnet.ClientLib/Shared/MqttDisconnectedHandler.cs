
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MQTTnet.ClientLib
{
    internal class MqttDisconnectedHandler : IMqttClientDisconnectedHandler
    {
        Action<MqttClientDisconnectedEventArgs> _disconnectedAction;

        public MqttDisconnectedHandler(Action<MqttClientDisconnectedEventArgs> disconnectAction)
        {
            _disconnectedAction = disconnectAction;
        }

        public async Task HandleDisconnectedAsync(MqttClientDisconnectedEventArgs eventArgs)
        {
            _disconnectedAction?.Invoke(eventArgs);
        }
    }
}
