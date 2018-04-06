using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Ecclesia.Resolver.Endpoint.Models;
using Ecclesia.Resolver.Storage.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ecclesia.Resolver.Endpoint.Controllers
{
    [Route("api/[controller]")]
    public class AtomsController : Controller
    {
        [HttpGet]
        public IEnumerable<AtomContent> Get([FromQuery] string atoms)
        {
            var reqJson = HttpUtility.UrlDecode(Encoding.UTF8.GetBytes(atoms), Encoding.UTF8);
            var atomsList = JsonConvert.DeserializeObject<IEnumerable<AtomId>>(reqJson);
            return atomsList.Select(x => new AtomContent { Name = x.Name, Kind = x.Kind, Content = "" });
        }

        [HttpGet("{kind}/{name}")]
        public IActionResult Get(string kind, string name, [FromQuery] string version)
        {
            return Ok(new AtomContent[] { new AtomContent { Kind = kind, Name = name, Content = "c" } });
        }

        [HttpHead("{kind}/{name}")]
        public IActionResult Head(string kind, string name, [FromQuery] string version)
        {
            return Ok(new AtomHeader { Kind = kind, Name = name, Dependencies = new List<AtomId>() });
        }

        [HttpPost]
        public IActionResult Post([FromBody]AtomCreationRequest value)
        {
            return BadRequest("Not implemented");
        }
    }
}
