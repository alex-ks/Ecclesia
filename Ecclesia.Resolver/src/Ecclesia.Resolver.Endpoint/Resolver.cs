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

        public async Task<IEnumerable<AtomId>> ResolveAsync(IEnumerable<AtomId> atoms)
        {
            var atomsSet = new HashSet<AtomId>(atoms);
            var atomsQueue = new Queue<AtomId>(atoms);

            while (atomsQueue.Count != 0)
            {
                var atom = atomsQueue.Dequeue();
                var deps = await _storage.GetDependenciesAsync(atom);
                
                foreach (var dep in deps)
                {
                    atomsSet.Add(dep);
                    atomsQueue.Enqueue(dep);
                }
            }

            return atomsSet;
        }
    }
}