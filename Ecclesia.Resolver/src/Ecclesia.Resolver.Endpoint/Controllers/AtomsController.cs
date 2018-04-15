using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Ecclesia.Resolver.Endpoint.Models;
using Ecclesia.Resolver.Storage;
using Ecclesia.Resolver.Storage.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ecclesia.Resolver.Endpoint.Controllers
{
    [Route("api/[controller]")]
    public class AtomsController : Controller
    {
        private AtomStorage _storage;

        public AtomsController(AtomStorage storage)
        {
            _storage = storage;
        }

        // Returns content of all requested atoms and all their dependencies
        [HttpGet]
        public IEnumerable<AtomContent> Get([FromQuery] string atoms)
        {
            var reqJson = HttpUtility.UrlDecode(Encoding.UTF8.GetBytes(atoms), Encoding.UTF8);
            var atomsList = JsonConvert.DeserializeObject<IEnumerable<AtomId>>(reqJson);
            return atomsList.Select(x => new AtomContent { Name = x.Name, Kind = x.Kind, Content = "" });
        }

        // Returns content of the requested atom only
        [HttpGet("{kind}/{name}")]
        public async Task<IActionResult> Get(string kind, string name, [FromQuery] string version)
        {
            try
            {
                var atomId = new AtomId
                {
                    Kind = kind,
                    Name = name,
                    Version = version
                };
                
                var content = await _storage.GetContentAsync(atomId);
                
                return Ok(new AtomContent 
                { 
                    Kind = kind, 
                    Name = name,
                    Version = version,
                    Content = Convert.ToBase64String(content)
                });
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }
            catch (FormatException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpHead("{kind}/{name}")]
        public async Task<IActionResult> Head(string kind, string name, [FromQuery] string version)
        {
            try
            {
                var atomId = new AtomId
                {
                    Kind = kind, 
                    Name = name,
                    Version = version
                };
                atomId.Version = await _storage.FetchVersionAsync(atomId);
                var dependencies = await _storage.GetDependenciesAsync(atomId);
                return Ok(new AtomHeader 
                { 
                    Kind = kind, 
                    Name = name, 
                    Version = atomId.Version,
                    Dependencies = dependencies.ToList()
                });
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }
            catch (FormatException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]AtomCreationRequest request)
        {
            try
            {
                var atomId = new AtomId
                {
                    Kind = request.Kind, 
                    Name = request.Name,
                    Version = request.Version
                };
                await _storage.AddAsync(atomId, request.Dependencies, Convert.FromBase64String(request.Content));
                return Ok(atomId);
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }
            catch (FormatException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
