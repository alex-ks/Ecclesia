using System;
using System.Collections.Generic;
using Ecclesia.Resolver.Endpoint.Models;

namespace Ecclesia.Resolver.Endpoint
{
    public class DependencyResolver
    {
        IEnumerable<AtomId> GetDependencies(AtomId atom)
        {
            throw new NotImplementedException();
        }

        IEnumerable<AtomId> GetDependencies(IEnumerable<AtomId> atoms)
        {
            throw new NotImplementedException();
        }
    }
}