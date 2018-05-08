using System;
using System.Linq;
using System.Threading.Tasks;
using Ecclesia.Resolver.Orm;
using Ecclesia.Resolver.Storage;
using Ecclesia.Resolver.Storage.Models;
using Xunit;

namespace Ecclesia.Resolver.EndpointTest
{
    public class ResoverTest
    {
        private ResolverContext InitContext()
        {
            var context = new InMemoryResolverContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task DependencyResolution_NotDependentAtomPassed_AtomOnlyReturned()
        {
            var storage = new AtomStorage(InitContext());
            var atomId = await storage.AddAsync(new AtomId("text", "hello"), Enumerable.Empty<AtomId>(), new byte[0]);

            var resolver = new Resolver.Endpoint.Resolver(storage);
            var resolved = await resolver.ResolveAsync(new [] { atomId });

            Assert.Single(resolved);
            var resolvedId = resolved.Single();
            Assert.Equal(atomId, resolvedId);
        }

        [Fact]
        public async Task DependecyResolutionOrdering_SingleAtomOrdered_CorrectOrderReturned()
        {
            var storage = new AtomStorage(InitContext());
            var si = await storage.AddAsync(new AtomId("fsdecl", "SI"), 
                                            Enumerable.Empty<AtomId>(), 
                                            new byte[0]);
            var eeg = await storage.AddAsync(new AtomId("fsdecl", "Eeg"), 
                                             Enumerable.Empty<AtomId>().Append(si), 
                                             new byte[0]);
            var workflow = await storage.AddAsync(new AtomId("filomena", "IcaFiltering"), 
                                                  Enumerable.Empty<AtomId>().Append(eeg), 
                                                  new byte[0]);

            var resolver = new Resolver.Endpoint.Resolver(storage);
            var resolved = await resolver.ResolveAsync(new [] { workflow });

            Assert.Equal(new [] { si, eeg, workflow }, resolved);
        }

        [Fact]
        public async Task DependecyResolutionOrdering_SingleAtomDuplicateDeps_CorrectOrderReturned()
        {
            var storage = new AtomStorage(InitContext());
            var si = await storage.AddAsync(new AtomId("fsdecl", "SI"), 
                                            Enumerable.Empty<AtomId>(), 
                                            new byte[0]);
            var eeg = await storage.AddAsync(new AtomId("fsdecl", "Eeg"), 
                                             Enumerable.Empty<AtomId>().Append(si), 
                                             new byte[0]);
            var workflow = await storage.AddAsync(new AtomId("filomena", "IcaFiltering"), 
                                                  Enumerable.Empty<AtomId>().Append(eeg).Append(si), 
                                                  new byte[0]);

            var resolver = new Resolver.Endpoint.Resolver(storage);
            var resolved = await resolver.ResolveAsync(new [] { workflow });

            Assert.Equal(new [] { si, eeg, workflow }, resolved);
        }

        [Fact]
        public async Task DependecyResolutionOrdering_MultipleAtomsDependent_CorrectOrderReturned()
        {
            var storage = new AtomStorage(InitContext());
            var si = await storage.AddAsync(new AtomId("fsdecl", "SI"), 
                                            Enumerable.Empty<AtomId>(), 
                                            new byte[0]);
            var eeg = await storage.AddAsync(new AtomId("fsdecl", "Eeg"), 
                                             Enumerable.Empty<AtomId>().Append(si), 
                                             new byte[0]);

            var resolver = new Resolver.Endpoint.Resolver(storage);
            var resolved = await resolver.ResolveAsync(new [] { eeg, si });

            Assert.Equal(new [] { si, eeg }, resolved);
        }
    }
}
