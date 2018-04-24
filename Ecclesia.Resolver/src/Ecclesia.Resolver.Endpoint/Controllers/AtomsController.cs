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
        private readonly AtomStorage _storage;
        private readonly Resolver _resolver;

        public AtomsController(AtomStorage storage, Resolver resolver)
        {
            _storage = storage;
            _resolver = resolver;
        }

        // Returns content of all requested atoms and all their dependencies
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string atoms)
        {
            try
            {
                var reqJson = HttpUtility.UrlDecode(Encoding.UTF8.GetBytes(atoms), Encoding.UTF8);
                var givenAtoms = JsonConvert
                    .DeserializeObject<IEnumerable<AtomIdArgument>>(reqJson)
                    .Select(atom => new AtomId(atom.Kind, atom.Name, atom.Version));

                var allAtoms = await _resolver.ResolveAsync(givenAtoms);
                var contents = await Task.WhenAll(allAtoms.Select(atom => _storage.GetContentAsync(atom)));

                var result = from pair in Enumerable.Zip(allAtoms, contents, (atom, content) => (atom, content))
                             select new AtomContent
                             {
                                 Kind = pair.atom.Kind,
                                 Name = pair.atom.Name,
                                 Version = pair.atom.Version,
                                 Content = Convert.ToBase64String(pair.content)
                             };
                return Ok(result);
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

        // Returns content of the requested atom only
        [HttpGet("{kind}/{name}")]
        public async Task<IActionResult> Get(string kind, string name, [FromQuery] string version)
        {
            try
            {
                var atomId = await _storage.FetchVersionAsync(new AtomId(kind, name, version));
                
                var content = await _storage.GetContentAsync(atomId);
                
                return Ok(new AtomContent 
                { 
                    Kind = kind, 
                    Name = name,
                    Version = atomId.Version,
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

        [HttpGet("{kind}/{name}/info")]
        public async Task<IActionResult> GetInfo(string kind, string name, [FromQuery] string version)
        {
            try
            {
                var atomId = await _storage.FetchVersionAsync(new AtomId(kind, name, version));
                var dependencies = await _storage.GetDependenciesAsync(atomId);
                return Ok(new AtomInfo 
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
            if (request == null)
                return BadRequest("Invalid atom creation request format");

            try
            {
                var atomId = new AtomId(request.Kind, request.Name, request.Version);
                var realId = await _storage.AddAsync(atomId, 
                                                     request.Dependencies, 
                                                     Convert.FromBase64String(request.Content ?? string.Empty));
                return Ok(realId);
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
