
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Server;
using System;

using System.Text;
using System.Threading.Tasks;


namespace MQTTnet.ClientLib
{
    public class MqttService
    {
        private static MqttService _instance { get; set; }
        public static MqttService MqttClient
        {
            get
            {
                if (_instance == null)
                    _instance = new MqttService();
                return _instance;
            }
        }
        private IMqttClient _mqttClient;
        private IMqttClientOptions mqttClientOptions;

        public event EventHandler<MqttClientConnectedEventArgs> Connected;
        public event EventHandler<MqttClientDisconnectedEventArgs> Disconnected;
        public event EventHandler<MqttApplicationMessageReceivedEventArgs> MessageReceived;

        public bool IsConnected()
        {
            return _mqttClient.IsConnected;
        }
        public async Task<bool> Connect()
        {
            try
            {
                await _mqttClient.ConnectAsync(mqttClientOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            return true;
        }

        public async Task<bool> Reconnect()
        {
            try
            {
                await _mqttClient.ReconnectAsync();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> Disconnect()
        {
            try
            {
                await _mqttClient.DisconnectAsync();
            }
            catch(Exception ex) 
            {
                return false;
            }
            return true;
        }

        public void Init(string clientId, IMqttClientOptions mqttClientOptions)
        {
            try
            {
                this.mqttClientOptions = mqttClientOptions;
                var factory = new MqttFactory();
                _mqttClient = factory.CreateMqttClient();
                _mqttClient.ConnectedHandler = new MqttConnectedHandler(mqttClientConnectedEventArgs =>
                {
                    Connected?.Invoke(this, mqttClientConnectedEventArgs);
                });
                _mqttClient.DisconnectedHandler = new MqttDisconnectedHandler(disconnectEventArgs =>
                {
                    Disconnected?.Invoke(this, disconnectEventArgs);
                });
                _mqttClient.ApplicationMessageReceivedHandler = new MqttMessageReceivedHandler(messageReceivedArgs =>
                {
                    MessageReceived?.Invoke(this, messageReceivedArgs);
                });
            }
            catch (Exception ex)
            {

            }
        }

        public async Task<bool> Subscribe(string topic, MQTTnet.Protocol.MqttQualityOfServiceLevel qos = MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce)
        {
            try
            {
                await _mqttClient.SubscribeAsync(topic, qos);
            }catch(Exception ex)
            {
                return false;
            }
            return true;
        }
        public async Task<bool> Unsubscribe(string topic)
        {
            try
            {
                await _mqttClient.UnsubscribeAsync(topic);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public async Task<bool> Publish(MqttApplicationMessage message)
        {
            try
            {
                await _mqttClient.PublishAsync(message);
            }
            catch(Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
