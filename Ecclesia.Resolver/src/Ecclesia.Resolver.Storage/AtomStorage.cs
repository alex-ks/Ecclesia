using System;
using System.Linq;
using System.Collections.Generic;
using Ecclesia.Resolver.Orm;
using Ecclesia.Resolver.Orm.Models;
using Ecclesia.Resolver.Storage.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Ecclesia.Resolver.Storage
{
    public class AtomStorage : IDisposable
    {
        private ResolverContext _context;

        public AtomStorage(ResolverContext context)
        {
            _context = context;
        }

        public IEnumerable<AtomId> GetDependencies(AtomId atomId)
        {
            var atom = 
                _context.Atoms
                    .Include(a => a.Dependencies)
                    .ThenInclude(d => d.Dependency)
                    .Single(a => a.Kind == atomId.Kind && a.Name == atomId.Name && atomId.Version == a.Version);

            var deps = from dep in atom.Dependencies
                       select new AtomId
                       {
                           Kind = dep.Dependency.Kind,
                           Name = dep.Dependency.Name,
                           Version = dep.Dependency.Version
                       };

            return deps.ToList();
        }

        public byte[] GetContent(AtomId atomId)
        {
            var atom = _context.Atoms.Include(a => a.Content).Single(a => 
                a.Kind == atomId.Kind && a.Name == atomId.Name && atomId.Version == a.Version);
            return atom.Content.Content;
        }

        public async Task AddAsync(AtomId atomId, IEnumerable<AtomId> dependencies, byte[] content)
        {
            var dbAtom = new Atom
            {
                Kind = atomId.Kind,
                Name = atomId.Name,
                Version = atomId.Version,
            };

            _context.Atoms.Add(dbAtom);

            var depRefs = dependencies.Select(depId => 
                _context.Atoms.Single(a => 
                    a.Kind == depId.Kind && a.Name == depId.Name && depId.Version == a.Version));
            
            var dbDeps = from dep in depRefs
                         select new AtomDependency
                         {
                             Dependent = dbAtom,
                             Dependency = dep
                         };

            _context.AtomDependencies.AddRange(dbDeps);

            var dbContent = new AtomContent
            {
                Atom = dbAtom,
                Content = content
            };

            _context.AtomContents.Add(dbContent);

            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
