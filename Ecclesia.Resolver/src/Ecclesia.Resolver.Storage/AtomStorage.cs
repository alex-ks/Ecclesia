﻿using System;
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
        private const string AtomAlreadyExists = 
            "Atom with kind = {0}, name = {1} and version = {2} already exists";
        private const string AtomDoesNotExist = 
            "Atom with kind = {0}, name = {1} and version = {2} does not exist";

        private ResolverContext _context;

        private Task<string> LastVersionAsync(string kind, string name)
        {
            var all = from atom in _context.Atoms
                      where atom.Kind == kind && atom.Name == name
                      orderby new AtomId { Kind = kind, Name = name, Version = atom.Version }.SortCriteria().AsTuple() descending
                      select atom.Version;
            return all.FirstOrDefaultAsync();
        }

        public async Task<string> FetchVersionAsync(AtomId atomId)
        {
            var version = atomId.Version ?? await LastVersionAsync(atomId.Kind, atomId.Name);
            if (version == null)
                throw new ArgumentException(string.Format(AtomDoesNotExist, 
                                                          atomId.Kind, 
                                                          atomId.Name, 
                                                          "lastest"));
            return version;
        }

        public async Task<IEnumerable<AtomId>> FetchVersionsAsync(IEnumerable<AtomId> atoms)
        {
            var versions = await Task.WhenAll(atoms.Select(async atomId => await FetchVersionAsync(atomId)));
            return Enumerable.Zip(atoms, 
                                  versions, 
                                  (a, version) => new AtomId { Kind = a.Kind, Name = a.Name, Version = version });
        }

        private async Task<bool> ExistsExactAsync(AtomId atomId)
        {
            var all = from atom in _context.Atoms
                      where atom.Kind == atomId.Kind && atom.Name == atomId.Name && atom.Version == atomId.Version
                      select atom;
            return await all.CountAsync() == 1;
        }

        public AtomStorage(ResolverContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AtomId>> GetDependenciesAsync(AtomId atomId)
        {
            var version = await FetchVersionAsync(atomId);

            try
            {
                var atom = 
                    await _context.Atoms
                        .Include(a => a.Dependencies)
                        .ThenInclude(d => d.Dependency)
                        .SingleAsync(a => a.Kind == atomId.Kind && a.Name == atomId.Name && a.Version == version);

                var deps = from dep in atom.Dependencies
                        select new AtomId
                        {
                            Kind = dep.Dependency.Kind,
                            Name = dep.Dependency.Name,
                            Version = dep.Dependency.Version
                        };

                return deps.ToList();
            }
            catch (InvalidOperationException)
            {
                throw new ArgumentException(string.Format(AtomDoesNotExist, 
                                                          atomId.Kind, 
                                                          atomId.Name, 
                                                          atomId.Version));
            }
        }

        public async Task<byte[]> GetContentAsync(AtomId atomId)
        {
            var version = await FetchVersionAsync(atomId);

            try
            {
                var atom = await _context.Atoms
                    .Include(a => a.Content)
                    .SingleAsync(a => a.Kind == atomId.Kind 
                                    && a.Name == atomId.Name 
                                    && atomId.Version == version);
                return atom.Content.Content;
            } 
            catch (InvalidOperationException)
            {
                throw new ArgumentException(string.Format(AtomDoesNotExist, 
                                                          atomId.Kind, 
                                                          atomId.Name, 
                                                          atomId.Version));
            }
        }

        /// If no version specified and atom does not exist, creates atom with default version "1.0.0"
        /// If no version specified and atom does exist, or there is an atom with specified version,
        /// creates new version with the same major and middle versions and incremented minor version
        public async Task AddAsync(AtomId atomId, IEnumerable<AtomId> dependencies, byte[] content)
        {
            if (await ExistsExactAsync(atomId))
            {
                throw new ArgumentException (string.Format(AtomAlreadyExists, 
                                                           atomId.Kind,
                                                           atomId.Name,
                                                           atomId.Version));
            }

            var version = atomId.Version ?? await LastVersionAsync(atomId.Kind, atomId.Name);
            
            if (atomId.Version == null && version != null)
            {
                VersionUtils.TryParse(version, out int major, out int? middle, out int? minor, out string name);
                version = VersionUtils.Serialize(major, 
                                                 middle.GetValueOrDefault(), 
                                                 minor.GetValueOrDefault() + 1,
                                                 null);
            }
            else if (atomId.Version == null)
                version = AtomId.DefaultVersion;

            var dbAtom = new Atom
            {
                Kind = atomId.Kind,
                Name = atomId.Name,
                Version = version,
            };

            await _context.Atoms.AddAsync(dbAtom);

            dependencies = await FetchVersionsAsync(dependencies);

            var depRefs = dependencies.Select(depId => 
                _context.Atoms.Single(a => 
                    a.Kind == depId.Kind && a.Name == depId.Name && a.Version == depId.Version));
            
            var dbDeps = from dep in depRefs
                         select new AtomDependency
                         {
                             Dependent = dbAtom,
                             Dependency = dep
                         };

            await _context.AtomDependencies.AddRangeAsync(dbDeps);

            var dbContent = new AtomContent
            {
                Atom = dbAtom,
                Content = content
            };

            await _context.AtomContents.AddAsync(dbContent);

            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
