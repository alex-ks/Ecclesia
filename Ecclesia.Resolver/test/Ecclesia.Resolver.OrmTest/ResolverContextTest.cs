using System;
using System.Linq;
using System.Text;
using Ecclesia.Resolver.Orm;
using Ecclesia.Resolver.Orm.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ecclesia.Resolver.OrmTest
{
    public class ResolverContextTest
    {
        [Fact]
        public void AtomCreation_CreateSingle_SuccessfullCreation()
        {
            using (var context = new InMemoryResolverContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var atom = new Atom { Kind = "python", Name = "Eeg", Version = "1.0.0" };

                context.Atoms.Add(atom);

                context.SaveChanges();

                var created = context.Atoms.Find(atom.Id);

                Assert.Equal(atom.Id, created.Id);
                Assert.Equal(atom.Name, created.Name);
                Assert.Equal(atom.Kind, created.Kind);
                Assert.Equal(atom.Version, created.Version);
            }
        }

        [Fact]
        public void AtomCreation_CreateDuplicate_Fail()
        {
            using (var context = new InMemoryResolverContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var atom = new Atom { Kind = "python", Name = "Eeg", Version = "1.0.0" };

                context.Atoms.Add(atom);

                context.SaveChanges();

                context.Atoms.Add(atom);

                Assert.Throws<ArgumentException>(() => context.SaveChanges());
            }
        }

        [Fact]
        public void ContentAccess_CreateContent_GetContentViaNavigationProperty()
        {
            long atomId;
            const string Content = "print(\"Hello, eeg!\")";

            using (var context = new InMemoryResolverContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var atom = new Atom { Kind = "python", Name = "Eeg", Version = "1.0.0" };

                context.Atoms.Add(atom);

                context.SaveChanges();

                atomId = atom.Id;

                context.AtomContents.Add(new AtomContent 
                { 
                    AtomId = atom.Id, 
                    Content = Encoding.UTF8.GetBytes(Content) 
                });

                context.SaveChanges();
            }

            using (var context = new InMemoryResolverContext())
            {
                var atom = context.Atoms.Include(a => a.Content).Single(a => a.Id == atomId);
                var content = Encoding.UTF8.GetString(atom.Content.Content);

                Assert.Equal(Content, content);
            }
        }

        [Fact]
        void DependencyAccess_CreateDependentAtom_GetDependencyViaNavigationProperty()
        {
            long atomId, dependentId;

            using (var context = new InMemoryResolverContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var atom = new Atom { Kind = "python", Name = "Eeg", Version = "1.0.0" };
                var dependent = new Atom { Kind = "workflow", Name = "Filter", Version = "1.0.0" };

                context.Atoms.Add(atom);
                context.Atoms.Add(dependent);

                context.SaveChanges();

                context.AtomDependencies.Add(new AtomDependency
                { 
                    DependentId = dependent.Id, 
                    DependencyId = atom.Id
                });

                context.SaveChanges();

                atomId = atom.Id;
                dependentId = dependent.Id;
            }

            using (var context = new InMemoryResolverContext())
            {
                var dependent = context.Atoms
                    .Include(a => a.Dependencies)
                    .ThenInclude(d => d.Dependency)                    
                    .Single(a => a.Id == dependentId);
                Assert.NotEmpty(dependent.Dependencies);

                var atom = dependent.Dependencies.Select(d => d.Dependency).Single();                

                Assert.Equal("Filter", dependent.Name);
                Assert.Equal("Eeg", atom.Name);

                context.Entry(atom).Collection(a => a.Dependent).Load();
                var dep = atom.Dependent.Single();
                context.Entry(dep).Reference(d => d.Dependent).Load();
                Assert.Equal(dependent.Name, dep.Dependent.Name);
            }
        }
    }
}
