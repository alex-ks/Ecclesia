using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecclesia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Ecclesia.LocalExecutor.Endpoint.Controllers
{
    [Route("api/[controller]")]
    public class SessionController : Controller
    {
        private SessionManager _manager;
        private readonly ILogger _logger;

        public SessionController(SessionManager manager, Logger<SessionController> logger)
        {
            _manager = manager;
            _logger = logger;
        }
        
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {            
            try
            {
                var session = _manager.GetSessionStatus(id);
                return Ok(session);
            }
            catch (ArgumentException e)
            {
                _logger.LogError(e.ToString());
                return BadRequest(new { Error = e.Message });
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]SessionStartRequest request)
        {
            if (request == null)
            {
                _logger.LogError("Could not parse user request");
                return BadRequest(new { Error = "Incorrect input" });
            }

            var sessionId = _manager.StartSession(request.ComputationGraph);

            return Ok(new { SessionId = sessionId });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _manager.StopSession(id);
                return Ok();
            }
            catch (ArgumentException e)
            {
                _logger.LogError(e.ToString());
                return BadRequest(new { Error = e.Message });
            }
        }
    }
}
