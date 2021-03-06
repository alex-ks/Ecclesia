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
            // Given
            var context = InitContext();

            var contentString = "Hello, world!";
            var content = Encoding.UTF8.GetBytes(contentString);

            var atomId = new AtomId("PlainText", "Hello", "1.0.1");

            // When
            using (var storage = new AtomStorage(context))
            {
                await storage.AddAsync(atomId, Enumerable.Empty<AtomId>(), content);
            }

            // Then
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
            // Given
            var context = InitContext();

            var contentString = "Hello, world!";
            var content = Encoding.UTF8.GetBytes(contentString);

            var dependency = new AtomId("PlainText", "Hello", "1.0.1");
            var dependent = new AtomId("Sentence", "World", "1.0.0");

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

            // When
            using (var storage = new AtomStorage(context))
            {
                await storage.AddAsync(dependent, 
                                       Enumerable.Empty<AtomId>().Append(dependency),
                                       content);
            }

            // Then
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
        public async Task AtomAсquisition_GetContent_ContentGot()
        {
            // Given
            var context = InitContext();

            var contentString = "Hello, world!";
            var content = Encoding.UTF8.GetBytes(contentString);
            
            var atomId = new AtomId("PlainText", "Hello", "1.0.1");

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

            // When
            using (var storage = new AtomStorage(context))
            {
                var str = Encoding.UTF8.GetString(await storage.GetContentAsync(atomId));
            // Then
                Assert.Equal(contentString, str);
            }
        }

        [Fact]
        public async Task AtomAсquisition_GetDependencies_DependenciesGot()
        {
            // Given
            var context = InitContext();

            var contentString = "Hello, world!";
            var content = Encoding.UTF8.GetBytes(contentString);

            var dependency = new AtomId("PlainText", "Hello", "1.0.1");
            var dependent = new AtomId("Sentence", "World", "1.0.0");

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

            // When
            using (var storage = new AtomStorage(context))
            {
                var deps = await storage.GetDependenciesAsync(dependent);
                Assert.Single(deps);

                var dep = deps.Single();

            // Then
                Assert.Equal(dependency.Kind, dep.Kind);
                Assert.Equal(dependency.Name, dep.Name);
                Assert.Equal(dependency.Version, dep.Version);
            }
        }

        [Fact]
        public async Task VersionWildcard_CreateAtomWithoutPredcessors_CreatedAtomWithDefaultVersion()
        {
            // Given
            var context = InitContext();

            var contentString = "Hello, world!";
            var content = Encoding.UTF8.GetBytes(contentString);

            var atomId = new AtomId("PlainText", "Hello");

            // When
            using (var storage = new AtomStorage(context))
            {
                await storage.AddAsync(atomId, Enumerable.Empty<AtomId>(), content);
            }

            // Then
            using (context = new InMemoryResolverContext())
            {
                var dbAtom = context.Atoms.Single(a => a.Kind == atomId.Kind 
                                                       && a.Name == atomId.Name);

                Assert.Equal(AtomId.DefaultVersion, dbAtom.Version);
            }
        }

        [Theory]
        [InlineData("1.0.4-name", "1.0.5")]
        [InlineData("2-name", "2.0.1")]
        [InlineData("3.1", "3.1.1")]
        public async Task VersionWildcard_CreateAtomWithPredcessor_CreatedAtomWithMinorIncremented(
            string predcessorVersion, 
            string expectedVersion)
        {
            // Given
            var context = InitContext();

            var contentString = "Hello, world!";
            var content = Encoding.UTF8.GetBytes(contentString);

            var atomId = new AtomId("PlainText", "Hello");

            var dbAtom = new Atom
            {
                Kind = atomId.Kind,
                Name = atomId.Name,
                Version = predcessorVersion
            };

            context.Atoms.Add(dbAtom);
            context.SaveChanges();

            // When
            using (var storage = new AtomStorage(context))
            {
                await storage.AddAsync(atomId, Enumerable.Empty<AtomId>(), content);
            }

            // Then
            using (context = new InMemoryResolverContext())
            {
                var addedAtom = context.Atoms.Single(a => a.Kind == atomId.Kind 
                                                          && a.Name == atomId.Name
                                                          && a.Version != predcessorVersion);
                    
                Assert.Equal(expectedVersion, addedAtom.Version);
            }
        }

        [Theory]
        [InlineData("1.0.0", "1.0.1")]
        [InlineData("1.0.1", "1.1.0")]
        [InlineData("1.1.1", "2.0.0")]
        public async Task VersionWildcard_CreateAtomWithWildcardInDeps_AtomWithLastDepsCreated(
            string oldDependencyVersion,
            string lastDependencyVersion)
        {
            // Given
            var context = InitContext();

            var contentString = "Hello, world!";
            var content = Encoding.UTF8.GetBytes(contentString);

            var dependency = new AtomId("PlainText", "Hello");
            var dependent = new AtomId("Sentence", "World", "1.0.0");

            var dbAtomOldDep = new Atom
            {
                Kind = dependency.Kind,
                Name = dependency.Name,
                Version = oldDependencyVersion
            };
            dbAtomOldDep.Content = new AtomContent
            {
                Atom = dbAtomOldDep,
                Content = content
            };

            var dbAtomLastDep = new Atom
            {
                Kind = dependency.Kind,
                Name = dependency.Name,
                Version = lastDependencyVersion
            };
            dbAtomLastDep.Content = new AtomContent
            {
                Atom = dbAtomLastDep,
                Content = content
            };

            context.Atoms.Add(dbAtomOldDep);
            context.Atoms.Add(dbAtomLastDep);
            context.SaveChanges();

            // When
            using (var storage = new AtomStorage(context))
            {
                await storage.AddAsync(dependent, 
                                       Enumerable.Empty<AtomId>().Append(dependency),
                                       content);
            }

            // Then
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
                Assert.Equal(dbAtomLastDep.Kind, dependencyAtom.Kind);
                Assert.Equal(dbAtomLastDep.Name, dependencyAtom.Name);
                Assert.Equal(dbAtomLastDep.Version, dependencyAtom.Version);
            }
        }

        [Fact]
        public async Task VersionWildcard_GetContent_LastContentGot()
        {
            // Given
            var context = InitContext();

            var contentString = "Hello, world!";
            var content = Encoding.UTF8.GetBytes(contentString);
            
            var atomId = new AtomId("PlainText", "Hello");
            string version = "1.0.1";

            var dbAtom = new Atom
            {
                Kind = atomId.Kind,
                Name = atomId.Name,
                Version = version
            };
            dbAtom.Content = new AtomContent
            {
                Atom = dbAtom,
                Content = content
            };

            context.Atoms.Add(dbAtom);
            context.SaveChanges();

            // When
            using (var storage = new AtomStorage(context))
            {
                var str = Encoding.UTF8.GetString(await storage.GetContentAsync(atomId));
            // Then
                Assert.Equal(contentString, str);
            }
        }
    }
}
