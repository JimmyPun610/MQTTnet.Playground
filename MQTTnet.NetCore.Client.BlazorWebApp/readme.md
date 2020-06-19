#### Blazor web app setup for MQTTnet.

##### 1. Reference MQTTnet.ClientLib to Client project (Or you may use MQTTnet client directly)

##### 2. Create "appsettings.json" in "wwwwroot" folder
```json
{
  "Settings": {
    "MqttWebSocket": "localhost:5001/mqtt",
    "ApiHost": "https://localhost:5001/"
  }
}
```

##### 3. Create classs "Settings.cs"
```c#
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
```

##### 4. Setup in Program.cs, create singleton for settings and mqtt client
```C#
  public static async Task Main(string[] args)
        {
            ...
            builder.Services.AddSingleton<Settings>();
            builder.Services.AddSingleton<MQTTnet.ClientLib.MqttService>();
            ...
        }
```

##### 5. Init MqttClient (See [Index.razor](https://github.com/JimmyPun610/MQTTnet.Playground/blob/master/MQTTnet.NetCore.Client.BlazorWebApp/Client/Pages/Index.razor) for more information)
```C#
    //Setup the MqttClientOption for initialztion
    //Only web socket is supported in blazor
    var mqttClientOption = new MQTTnet.Client.Options.MqttClientOptionsBuilder().WithCleanSession(true)
                                                         .WithClientId(Guid.NewGuid().ToString())
                                                         .WithWebSocketServer(mqttWebSocket)
                                                         //as app.UseHttpsRedirection() is applied in Server, WithTls must be apply
                                                         .WithTls()
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
