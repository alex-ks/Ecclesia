using System;

namespace Ecclesia.DataAccessLayer.Models
{
    public class WorkflowVersion
    {
        public long WorkflowId { get; set; }
        public Workflow Workflow { get; set; }
        public string VersionName { get; set; }
        public DateTime CreationDate { get; set; }
    }
}