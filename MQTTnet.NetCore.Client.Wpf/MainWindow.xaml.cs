using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MQTTnet.NetCore.Client.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<string> MQMessage = new ObservableCollection<string>();
        public MainWindow()
        {
            InitializeComponent();
            MQResultList.ItemsSource = MQMessage;
        }

        private async void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            var clientOption = new MqttClientOptionsBuilder().WithTcpServer(HostTb.Text, int.Parse(PortTb.Text))
                                          .WithClientId(Guid.NewGuid().ToString())
                                          .WithCleanSession(true)
                                          .Build();

            MQTTnet.ClientLib.MqttService.MqttClient.Init(Guid.NewGuid().ToString(), clientOption);
            MQTTnet.ClientLib.MqttService.MqttClient.Connected += MqttClient_Connected;
            MQTTnet.ClientLib.MqttService.MqttClient.MessageReceived += MqttClient_MessageReceived;
            MQTTnet.ClientLib.MqttService.MqttClient.Disconnected += MqttClient_Disconnected;
            await MQTTnet.ClientLib.MqttService.MqttClient.Connect();
        }

        private void MqttClient_Disconnected(object sender, MqttClientDisconnectedEventArgs e)
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

        private void MqttClient_MessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("MQTT MessageReceived!");
            str.AppendLine($"Content Type : {e.ApplicationMessage.ContentType}");
            str.AppendLine($"Payload : {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            WriteLog(str.ToString());
        }

        private void MqttClient_Connected(object sender, MqttClientConnectedEventArgs e)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("MQTT Connected!");
            str.AppendLine($"Result code : {e.AuthenticateResult.ResultCode}");
            WriteLog(str.ToString());
        }

        private async void SubscribeBtn_Click(object sender, RoutedEventArgs e)
        {
            await MQTTnet.ClientLib.MqttService.MqttClient.Subscribe(TopicTb.Text, MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce);
        }

        private async void UnSubscribeBtn_Click(object sender, RoutedEventArgs e)
        {
            await MQTTnet.ClientLib.MqttService.MqttClient.Unsubscribe(TopicTb.Text);
        }


        private void WriteLog(string msg)
        {
            MQResultList.Dispatcher.BeginInvoke(new Action(delegate
            {
                MQMessage.Insert(0, msg);
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private async void PublishBtn_Click(object sender, RoutedEventArgs e)
        {
            await MQTTnet.ClientLib.MqttService.MqttClient.Publish(new MqttApplicationMessage
            {
                Topic = TopicTb.Text,
                QualityOfServiceLevel = Protocol.MqttQualityOfServiceLevel.AtLeastOnce,
                Payload = Encoding.UTF8.GetBytes(MessageTb.Text)
            });
        }
    }
}
