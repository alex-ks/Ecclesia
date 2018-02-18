using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecclesia.LocalExecutor.Endpoint.Models;


namespace Ecclesia.LocalExecutor.Endpoint
{
    public static class SessionUtilities
    {       
        public static void UpdateMnemonicValues(Dictionary<string, MnemonicsValue> values, string[] operationOut, string[] newOut)
        {
            int index = 0;
            foreach (string each in operationOut)
            {
                values.Add(each, new MnemonicsValue { Value = newOut[index], Type = null});
                Console.WriteLine($"UPDATE VALUES: add var: {each} val: {newOut[index]}");
                index++;
            }
        }

        public static bool SessionCompleted(List<OperationStatus> operations)
        {
            return operations.All(id => id.Status == OperationState.Completed);
        }

        public static bool SessionFaild(List<OperationStatus> operations)
        {
            return operations.Exists(id => id.Status == OperationState.Failed);
        }

        public static void OperationRunning(OperationStatus operation)
        {
            operation.Status = OperationState.Running;
        }

        public static void OperationCompleted(OperationStatus operation, string[] outputs)
        {
            operation.Status = OperationState.Completed;
        }

        public static void OperationFaild(OperationStatus operation)
        {
            operation.Status = OperationState.Failed;
        }

        public static List<int> GetIDAvailableOperation(List<OperationStatus> operation, int[][] dependencies)
        {
            List<int> availableOperation = new List<int>();
            int countOperation = dependencies.Length;
            for(int i = 0; i < countOperation; i++)
            {
                if(operation[i].Status == OperationState.Awaits)
                {
                    if (dependencies[i].All(id => operation[id].Status == OperationState.Completed))
                    {
                        availableOperation.Add(i);
                    }                    
                }
            }

            return availableOperation;
        }
    }
}
