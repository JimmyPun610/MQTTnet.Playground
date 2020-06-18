using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MQTTnet.Server;

namespace MQTTnet.NetCore.Server.Controllers.Api
{
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
}
