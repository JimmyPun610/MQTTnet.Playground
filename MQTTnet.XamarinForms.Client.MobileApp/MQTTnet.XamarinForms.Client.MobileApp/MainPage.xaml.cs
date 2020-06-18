using MQTTnet.Client.Options;
using MQTTnet.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MQTTnet.XamarinForms.Client.MobileApp
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

            MQTTnet.ClientLib.MqttService.MqttClient.Init(Guid.NewGuid().ToString(), new MqttClientOptionsBuilder().WithClientId(Guid.NewGuid().ToString())
                                                                                                                   .WithCleanSession(true)
                                                                                                                   .WithTcpServer(ServerEntry.Text, int.Parse(PortEntry.Text))
                                                                                                                   .Build());
            MQTTnet.ClientLib.MqttService.MqttClient.Connected += MqttClient_Connected;
            MQTTnet.ClientLib.MqttService.MqttClient.MessageReceived += MqttClient_MessageReceived;
            MQTTnet.ClientLib.MqttService.MqttClient.Disconnected += MqttClient_Disconnected;
            await MQTTnet.ClientLib.MqttService.MqttClient.Connect();
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
                await MQTTnet.ClientLib.MqttService.MqttClient.Reconnect();
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
            await MQTTnet.ClientLib.MqttService.MqttClient.Subscribe(TopicEntry.Text, MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce);
        }

        private async void UnsubscribeButton_Clicked(object sender, EventArgs e)
        {
            await MQTTnet.ClientLib.MqttService.MqttClient.Unsubscribe(TopicEntry.Text);

        }

        private async void PublishBtn_Clicked(object sender, EventArgs e)
        {
            string message = await DisplayPromptAsync("Publish Message", "What do you want to publish?");
            if (!string.IsNullOrWhiteSpace(message))
                await MQTTnet.ClientLib.MqttService.MqttClient.Publish(new MqttApplicationMessage
                {
                    Topic = TopicEntry.Text,
                    Payload = Encoding.UTF8.GetBytes(message)
                });
        }
    }
}
