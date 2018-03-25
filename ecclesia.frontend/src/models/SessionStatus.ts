import IDictionary from "src/models/IDictionary"
import { IMnemonicValue } from "src/models/ComputationGraph";

export enum OperationState
{
    Awaits = "Awaits",
    Running = "Running",
    Completed = "Completed",
    Aborted = "Aborted",
    Failed = "Failed"
}

export interface IOperationStatus
{
    id: number;
    state: OperationState;
}

export interface ISessionStatus
{
    sessionId: number;
    operationStatus: IOperationStatus[];
    mnemonicsTable: IDictionary<IMnemonicValue>;
    startTime: Date | string;
}

export enum SessionState
{
    Running, Completed, Failed, Aborted
}

export function getState(sessionStatus: ISessionStatus)
{
    if (sessionStatus.operationStatus.some(op => op.state == OperationState.Failed))
        return SessionState.Failed;
    if (sessionStatus.operationStatus.some(op => op.state == OperationState.Aborted))
        return SessionState.Aborted;
    if (sessionStatus.operationStatus.every(op => op.state == OperationState.Completed))
        return SessionState.Completed;
    return SessionState.Running;
}