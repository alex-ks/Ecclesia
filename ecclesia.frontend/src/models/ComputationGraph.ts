import IDictionary from "src/models/IDictionary"

export interface IDataType
{
    name: string;
    parameters?: IDataType[];
}

export function validateDataType(dType: IDataType)
{
    if ("name" in dType)
        return true;
    return false;
}

export interface IOperation
{ 
    id: number; 
    name: string; 
    input: string[]; 
    output: string[]; 
    parameters?: IDataType[]; 
}

export function validateOperation(op: IOperation)
{
    if (!("id" in op))
        return false;
    if (!("name" in op))
        return false;
    if (!("input" in op))
        return false;
    if (!("output" in op))
        return false;
    return true;
}

export interface IMnemonicValue
{ 
    dataType: IDataType; 
    value: string 
}

export function validateMnemonicValue(val: IMnemonicValue)
{
    if (!("value" in val))
        return false;
    if (!("dataType" in val))
        return false;
    return true;
}

export interface IComputationGraph 
{ 
    operations: IOperation[]; 
    dependencies: number[][];
    mnemonicsTable: IDictionary<IMnemonicValue>
}

export function validateComputationGraph(graph: IComputationGraph)
{
    if (!("operations" in graph))
        return false;
    if (!graph.operations.every(op => validateOperation(op)))
        return false;
    throw new Error("Not implemented");
}
