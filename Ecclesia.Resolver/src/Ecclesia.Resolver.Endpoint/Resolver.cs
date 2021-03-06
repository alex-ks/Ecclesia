using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecclesia.Resolver.Storage;
using Ecclesia.Resolver.Storage.Models;

namespace Ecclesia.Resolver.Endpoint
{
    public class Resolver
    {
        private readonly AtomStorage _storage;

        public Resolver(AtomStorage storage)
        {
            _storage = storage;
        }

        private async Task AtomDfs(AtomId atom, HashSet<AtomId> resolved, List<AtomId> ordered)
        {
            var current = await _storage.FetchVersionAsync(atom);
            var deps = await _storage.GetDependenciesAsync(current);
            foreach (var dep in deps)
                await AtomDfs(dep, resolved, ordered);
            if (!resolved.Contains(current))
            {
                resolved.Add(current);
                ordered.Add(current);
            }
        }

        public async Task<IEnumerable<AtomId>> ResolveAsync(IEnumerable<AtomId> atoms)
        {
            var atomsSet = new HashSet<AtomId>();
            var orderedAtoms = new List<AtomId>();

            foreach (var atom in atoms)
                await AtomDfs(atom, atomsSet, orderedAtoms);

            return orderedAtoms;
        }
    }
}