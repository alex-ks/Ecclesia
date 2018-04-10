using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecclesia.Resolver.Orm;
using Ecclesia.Resolver.Storage;
using Ecclesia.Resolver.Storage.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ecclesia.Resolver.StorageTest
{
    public class StorageTest
    {
        [Fact]
        public async Task AtomCreation_CreateAtomNoDeps_AtomWithNoDepsCreated()
        {
            var context = new InMemoryResolverContext();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

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
    }
}
