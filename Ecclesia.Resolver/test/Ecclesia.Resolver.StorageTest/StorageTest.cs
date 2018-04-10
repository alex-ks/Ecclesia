using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecclesia.Resolver.Orm;
using Ecclesia.Resolver.Orm.Models;
using Ecclesia.Resolver.Storage;
using Ecclesia.Resolver.Storage.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ecclesia.Resolver.StorageTest
{
    public class StorageTest
    {
        private ResolverContext InitContext()
        {
            var context = new InMemoryResolverContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task AtomCreation_CreateAtomNoDeps_AtomWithNoDepsCreated()
        {
            var context = InitContext();

            var contentString = "Hello, world!";
            var content = Encoding.UTF8.GetBytes(contentString);

            var atomId = new AtomId
            {
                Kind = "PlainText",
                Name = "Hello",
                Version = "1.0.1"
            };

            using (var storage = new AtomStorage(context))
            {
                await storage.AddAsync(atomId, Enumerable.Empty<AtomId>(), content);
            }

            using (context = new InMemoryResolverContext())
            {
                var dbAtom = context.Atoms.Include(a => a.Content)
                                          .Include(a => a.Dependencies)
                                          .ThenInclude(dep => dep.Dependency)
                                          .Single(a => a.Kind == atomId.Kind 
                                                       && a.Name == atomId.Name 
                                                       && a.Version == atomId.Version);

                Assert.Empty(dbAtom.Dependencies);
                var actualContentString = Encoding.UTF8.GetString(dbAtom.Content.Content);

                Assert.Equal(contentString, actualContentString);
            }
        }

        [Fact]
        public async Task AtomCreation_CreateAtomWithDeps_AtomWithDepsCreated()
        {
            var context = InitContext();

            var contentString = "Hello, world!";
            var content = Encoding.UTF8.GetBytes(contentString);

            var dependency = new AtomId
            {
                Kind = "PlainText",
                Name = "Hello",
                Version = "1.0.1"
            };

            var dependent = new AtomId
            {
                Kind = "Sentence",
                Name = "World",
                Version = "1.0.0"
            };

            var dbAtomDep = new Atom
            {
                Kind = dependency.Kind,
                Name = dependency.Name,
                Version = dependency.Version
            };
            dbAtomDep.Content = new AtomContent
            {
                Atom = dbAtomDep,
                Content = content
            };

            context.Atoms.Add(dbAtomDep);
            context.SaveChanges();

            using (var storage = new AtomStorage(context))
            {
                await storage.AddAsync(dependent, 
                                       Enumerable.Empty<AtomId>().Append(dependency),
                                       content);
            }

            using (context = new InMemoryResolverContext())
            {
                var dbAtom = context.Atoms.Include(a => a.Content)
                                          .Include(a => a.Dependencies)
                                          .ThenInclude(dep => dep.Dependency)
                                          .Single(a => a.Kind == dependent.Kind 
                                                       && a.Name == dependent.Name 
                                                       && a.Version == dependent.Version);

                Assert.Single(dbAtom.Dependencies);
                
                var dependencyAtom = dbAtom.Dependencies.Single().Dependency;
                Assert.Equal(dbAtomDep.Kind, dependencyAtom.Kind);
                Assert.Equal(dbAtomDep.Name, dependencyAtom.Name);
                Assert.Equal(dbAtomDep.Version, dependencyAtom.Version);
            }
        }

        [Fact]
        public void AtomAсquisition_GetContent_ContentGot()
        {
            var context = InitContext();

            var contentString = "Hello, world!";
            var content = Encoding.UTF8.GetBytes(contentString);
            
            var atomId = new AtomId
            {
                Kind = "PlainText",
                Name = "Hello",
                Version = "1.0.1"
            };

            var dbAtom = new Atom
            {
                Kind = atomId.Kind,
                Name = atomId.Name,
                Version = atomId.Version
            };
            dbAtom.Content = new AtomContent
            {
                Atom = dbAtom,
                Content = content
            };

            context.Atoms.Add(dbAtom);
            context.SaveChanges();

            using (var storage = new AtomStorage(context))
            {
                var str = Encoding.UTF8.GetString(storage.GetContent(atomId));
                Assert.Equal(contentString, str);
            }
        }

        [Fact]
        public void AtomAсquisition_GetDependencies_DependenciesGot()
        {
            var context = InitContext();

            var contentString = "Hello, world!";
            var content = Encoding.UTF8.GetBytes(contentString);

            var dependency = new AtomId
            {
                Kind = "PlainText",
                Name = "Hello",
                Version = "1.0.1"
            };

            var dependent = new AtomId
            {
                Kind = "Sentence",
                Name = "World",
                Version = "1.0.0"
            };

            var dbAtomDep = new Atom
            {
                Kind = dependency.Kind,
                Name = dependency.Name,
                Version = dependency.Version
            };
            dbAtomDep.Content = new AtomContent
            {
                Atom = dbAtomDep,
                Content = content
            };

            var dbAtom = new Atom
            {
                Kind = dependent.Kind,
                Name = dependent.Name,
                Version = dependent.Version
            };
            dbAtom.Content = new AtomContent
            {
                Atom = dbAtom,
                Content = content
            };
            dbAtom.Dependencies = new List<AtomDependency>
            {
                new AtomDependency
                {
                    Dependent = dbAtom,
                    Dependency = dbAtomDep
                }
            };

            context.Atoms.Add(dbAtomDep);
            context.Atoms.Add(dbAtom);
            context.SaveChanges();

            using (var storage = new AtomStorage(context))
            {
                var deps = storage.GetDependencies(dependent);
                Assert.Single(deps);

                var dep = deps.Single();

                Assert.Equal(dependency.Kind, dep.Kind);
                Assert.Equal(dependency.Name, dep.Name);
                Assert.Equal(dependency.Version, dep.Version);
            }
        }
    }
}
