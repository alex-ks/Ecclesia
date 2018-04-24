using System.Collections.Generic;

namespace Ecclesia.DataAccessLayer.Models
{
    public class Workflow
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string FriendlyName { get; set; }
        public string AtomName { get; set; }
        public List<WorkflowVersion> Versions { get; set; }
    }
}