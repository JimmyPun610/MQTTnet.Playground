#### AspNetCore3.1 Server setup for MQTTnet.

##### 1. Setup configuration in "appsettings.json", add below data to your "appsettings.json" file
```json
"KestrelSettings": {
    "MQTTPipeline": 1883,
    "HttpPipeline": 5000,
    "HttpsPipeline" : 5001
  }
```
##### 2. Create a class for MQTT server service (Please refer to [Mqtt/MqttService.cs](https://github.com/JimmyPun610/MQTTnet.Playground/blob/master/MQTTnet.NetCore.Server/Mqtt/MqttService.cs))

##### 3. Setup in "Startup.cs"
```C#
 public void ConfigureServices(IServiceCollection services)
        {
            ...
            //Allow Cors
            services.AddCors();
            //Add Singleton MQTT server object
            services.AddSingleton<Mqtt.MqttService>();
            //Add MQTT Service, you can have more settings on this. Please refer to MQTTnet original documents
            services
             .AddHostedMqttServer(mqttServer => mqttServer.WithoutDefaultEndpoint())
             .AddMqttConnectionHandler()
             .AddConnections();
            ...
        }
```
```C#
  public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
          ```
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //Setup mqtt endpoints for websocket (localhost:{port}/mqtt}
                endpoints.MapMqtt("/mqtt");
            });
            app.UseMqttServer(server => app.ApplicationServices.GetRequiredService<Mqtt.MqttService>());
            ```
        }
```
##### 4. Setup in "Program.cs"
```C#
 public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //Retrieve settings from "appsettings.json"
                    var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())  //location of the exe file
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    IConfigurationRoot configuration = builder.Build();
                    IConfigurationSection kestrelSettings = configuration.GetSection("KestrelSettings");
                    int mqttPipeline = kestrelSettings.GetValue<int>("MQTTPipeline");
                    int httpPipeline = kestrelSettings.GetValue<int>("HttpPipeline");
                    int httpsPipeline = kestrelSettings.GetValue<int>("HttpsPipeline");
                    //https://github.com/chkr1011/MQTTnet/wiki/Server Server setup documnetation
                    //https://forums.asp.net/t/2162724.aspx?Read+appsettings+json+and+use+to+toggle+function+in+program+cs using appsettings.json in program.cs
                    webBuilder
                    .UseKestrel(o =>
                    {
                        o.ListenAnyIP(mqttPipeline, l => l.UseMqtt());
                        o.ListenAnyIP(httpPipeline);
                        o.ListenAnyIP(httpsPipeline, l => l.UseHttps());
                    })
                    .UseStartup<Startup>();
                });
```
##### 5. (Optional) Create controller to access mqtt server via API
```C#
 [Route("api/[controller]")]
    [ApiController]
    public class MqttController : ControllerBase
    {
        private readonly ILogger<MqttController> _logger;
        private readonly IMqttServer _mqttServer;
        public MqttController(ILogger<MqttController> logger, IMqttServer mqttServer)
        {
            this._logger = logger;
            this._mqttServer = mqttServer;
        }
        [HttpGet]
        [Route("ServerStatus")]
        public IActionResult ServerStatus()
        {
            return Ok(_mqttServer.IsStarted);
        }

        [HttpPost]
        [Route("Publish")]
        public async Task<IActionResult> Publish([FromBody, Required] MqttApplicationMessage payload)
        {
            try
            {
                await this._mqttServer.PublishAsync(payload);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
            return Ok();
            
        }
    }
```
##### 6. Project configuration
If Visual Studio in Windows is using in the development, the default Launch of the web app will possibly be IISExpress which will cause an error like "Application is running inside IIS process but is not configured to use IIS server.".
To solve it, edit the Launch from IISExpress to Project.
```
  1. Right click project, select properties
  2. Select "Debug" in left hand side
  3. Change "Launch" from IISExpress to Project
```
