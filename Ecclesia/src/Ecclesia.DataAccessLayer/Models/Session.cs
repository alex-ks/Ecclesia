using System;
using System.Collections.Generic;
using Ecclesia.Models;
using Newtonsoft.Json;

namespace Ecclesia.DataAccessLayer.Models
{
    public enum SessionState
    {
        Running = 0,
        Complete = 1,
        Aborted = 2
    }

    public class Session
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? LastPolling { get; set; }
        public SessionState State { get; set; }
        public string ExecutionId { get; set; }
        
        public ComputationGraph OriginalGraph
        {
            get { return JsonConvert.DeserializeObject<ComputationGraph>(OriginalGraphJson); }
            set { OriginalGraphJson = JsonConvert.SerializeObject(value); }
        }

        public List<OperationStatus> OperationsStatus
        {
            get { return JsonConvert.DeserializeObject<List<OperationStatus>>(OperationsStatusJson); }
            set { OperationsStatusJson = JsonConvert.SerializeObject(value); }
        }
        
        public Dictionary<string, MnemonicsValue> MnemonicsTable
        {
            get { return JsonConvert.DeserializeObject<Dictionary<string, MnemonicsValue>>(MnemonicsTableJson); }
            set { MnemonicsTableJson = JsonConvert.SerializeObject(value); }
        }

        internal string OriginalGraphJson { get; set; }
        internal string OperationsStatusJson { get; set; }
        internal string MnemonicsTableJson { get; set; }
    }
}