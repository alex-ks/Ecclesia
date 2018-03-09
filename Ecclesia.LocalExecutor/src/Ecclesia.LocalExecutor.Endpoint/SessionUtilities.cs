using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecclesia.Models;


namespace Ecclesia.LocalExecutor.Endpoint
{
    public static class SessionUtilities
    {       
        public static void RegisterOperationResult(this SessionStatus session, string[] operationOut, string[] newOut)
        {
            foreach (var (name, value) in Enumerable.Zip(operationOut, newOut, (name, value) => (name, value)))
            {
                session.MnemonicsTable.Add(name, new MnemonicsValue 
                { 
                    Value = value, 
                    DataType = new DataType { Name = "any" } 
                });
            }
        }

        public static bool IsCompleted(this SessionStatus session)
        {
            return session.OperationStatus.All(id => id.State == OperationState.Completed);
        }

        public static bool IsFailed(this SessionStatus session)
        {
            return session.OperationStatus.Exists(id => id.State == OperationState.Failed);
        }

        public static void SetRunning(this OperationStatus operation)
        {
            operation.State = OperationState.Running;
        }

        public static void SetCompleted(this OperationStatus operation)
        {
            operation.State = OperationState.Completed;
        }

        public static void SetFaild(this OperationStatus operation)
        {
            operation.State = OperationState.Failed;
        }

        public static void SetAborted(this OperationStatus operation)
        {
            operation.State = OperationState.Aborted;
        }

        public static List<string> GetMnemonicsValues(this SessionStatus session, IEnumerable<string> names)
        {
            return names.Select(name => session.MnemonicsTable[name].Value).ToList();
        }

        public static List<int> GetAvailableOperationsIds(this SessionStatus session, int[][] dependencies)
        {
            var operations = session.OperationStatus;
            var ids = from pair in Enumerable.Zip(operations, 
                                                  dependencies, 
                                                  (operation, depList) => ((operation, depList)))
                      where pair.operation.State == OperationState.Awaits
                          && pair.depList.All(id => operations[id].State == OperationState.Completed)
                      select pair.operation.Id;
            return ids.ToList();
        }
    }
}
