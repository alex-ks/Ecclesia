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

        public async Task<IActionResult> GetLastWorkflows()
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> GetWorkflowVersions(string friendlyName)
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> GetSource(string friendlyName, string version)
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> CreateVersion(string friendlyName, string version, string content)
        {
            throw new NotImplementedException();
        }
    }
}