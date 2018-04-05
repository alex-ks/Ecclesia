namespace Ecclesia.Resolver.DataAccess.Models
{
    public class AtomContent
    {
        public long AtomId { get; set; }
        public Atom Atom { get; set; }

        public byte[] Content { get; set; }
    }
}