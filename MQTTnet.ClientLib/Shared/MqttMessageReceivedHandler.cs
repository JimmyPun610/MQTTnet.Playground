using MQTTnet;
using MQTTnet.Client.Receiving;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MQTTnet.ClientLib
{
    internal class MqttMessageReceivedHandler : IMqttApplicationMessageReceivedHandler
    {
        Action<MqttApplicationMessageReceivedEventArgs> _messageReceived;
        public MqttMessageReceivedHandler(Action<MqttApplicationMessageReceivedEventArgs> messageReceived)
        {
            _messageReceived = messageReceived;
        }
        public async Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
        {
            _messageReceived?.Invoke(eventArgs);
        }
    }
}
