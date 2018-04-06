using System;
using System.Collections.Generic;
using Ecclesia.Resolver.Storage.Models;

namespace Ecclesia.Resolver.Storage
{
    public class AtomStorage
    {
        public IEnumerable<AtomId> GetDependencies(AtomId atom)
        {
            throw new NotImplementedException();
        }

        public byte[] GetContent(AtomId atom)
        {
            throw new NotImplementedException();
        }
    }
}
