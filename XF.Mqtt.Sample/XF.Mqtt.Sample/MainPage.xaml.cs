using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XF.Mqtt.Sample
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        ObservableCollection<string> MQMessage = new ObservableCollection<string>();
        public MainPage()
        {
            InitializeComponent();
            MQResultList.ItemsSource = MQMessage;
        }
      
        private async void ConnectButton_Clicked(object sender, EventArgs e)
        {
            
            XF.Mqtt.MqttService.MqttClient.Init("XF.Mqtt.Sample", ServerEntry.Text, int.Parse(PortEntry.Text));
            XF.Mqtt.MqttService.MqttClient.Connected += MqttClient_Connected;
            XF.Mqtt.MqttService.MqttClient.MessageReceived += MqttClient_MessageReceived;
            XF.Mqtt.MqttService.MqttClient.Disconnected += MqttClient_Disconnected;
            await XF.Mqtt.MqttService.MqttClient.Connect();
        }

        private async void MqttClient_Disconnected(object sender, MQTTnet.Client.Disconnecting.MqttClientDisconnectedEventArgs e)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("MQTT Disconnected");
            str.AppendLine($"Client connected : {e.ClientWasConnected}");
            str.AppendLine($"Exception Message : {e.Exception?.Message}");
            WriteLog(str.ToString());
            Observable.Timer(TimeSpan.FromSeconds(5)).Subscribe(async (s) =>
            {
                await XF.Mqtt.MqttService.MqttClient.Reconnect();
            });
        }

        private void MqttClient_MessageReceived(object sender, MQTTnet.MqttApplicationMessageReceivedEventArgs e)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("MQTT MessageReceived!");
            str.AppendLine($"Content Type : {e.ApplicationMessage.ContentType}");
            str.AppendLine($"Payload : {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            WriteLog(str.ToString());
        }

        private void MqttClient_Connected(object sender, MQTTnet.Client.Connecting.MqttClientConnectedEventArgs e)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("MQTT Connected!");
            str.AppendLine($"Result code : {e.AuthenticateResult.ResultCode}");
            WriteLog(str.ToString());
        }

        private void WriteLog(string msg)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                MQMessage.Insert(0, msg);
            });
        }

        private async void SubscribeButton_Clicked(object sender, EventArgs e)
        {
            await XF.Mqtt.MqttService.MqttClient.SubscribeTopic(TopicEntry.Text, MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce);
        }

        private async void UnsubscribeButton_Clicked(object sender, EventArgs e)
        {
            await XF.Mqtt.MqttService.MqttClient.UnsubscribeTopic(TopicEntry.Text);

        }
    }
}
