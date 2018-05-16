using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecclesia.Endpoint.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class WorkflowsController : Controller
    {
        [HttpGet]
        [Route("last")]
        public async Task<IActionResult> GetLastWorkflows([FromQuery] int count)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWorkflows()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("{name}/versions")]
        public async Task<IActionResult> GetWorkflowVersions(string name)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("{name}")]
        public async Task<IActionResult> GetSource(string name, [FromQuery] string version)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("{name}")]
        public async Task<IActionResult> CreateVersion(string friendlyName, 
                                                       [FromQuery] string version, 
                                                       string content)
        {
            throw new NotImplementedException();
        }
    }
}