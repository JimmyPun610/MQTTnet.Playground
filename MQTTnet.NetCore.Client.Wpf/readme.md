#### AspNetCore3.1 WPF application setup for MQTTnet.

##### 1. Reference MQTTnet.ClientLib (Or you may use MQTTnet client directly)

##### 2. Init MqttClient
```C#
    //Setup the MqttClientOption for initialztion
    var clientOption = new MqttClientOptionsBuilder().WithTcpServer(HostTb.Text, int.Parse(PortTb.Text))
                                          .WithClientId(Guid.NewGuid().ToString())
                                          .WithCleanSession(true)
                                          .Build();
    //Init the client with client option
    MQTTnet.ClientLib.MqttService.MqttClient.Init(Guid.NewGuid().ToString(), clientOption);
    //Add listener for event (optional)
    MQTTnet.ClientLib.MqttService.MqttClient.Connected += MqttClient_Connected;
    MQTTnet.ClientLib.MqttService.MqttClient.MessageReceived += MqttClient_MessageReceived;
    MQTTnet.ClientLib.MqttService.MqttClient.Disconnected += MqttClient_Disconnected;
```

##### 3. Connect to MQTT server
```C#
    await MQTTnet.ClientLib.MqttService.MqttClient.Connect();
```

##### 4. Visit implementation of [MQTTnet.ClientLib.MqttService.cs](https://github.com/JimmyPun610/MQTTnet.Playground/blob/master/MQTTnet.ClientLib/Shared/MqttService.cs) for more methods.
