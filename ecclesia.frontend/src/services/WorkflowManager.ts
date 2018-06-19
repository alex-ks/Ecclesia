import IDictionary from "src/models/IDictionary";

export default class WorkflowManager
{
    private static sources: IDictionary<string> = {
        "1.0.0": [
            "module A",
            "let a = 1 + 1",
            "let b = a * 2"
        ].join("\n"),

        "1.0.1": [
            "module A",
            "let a = 2 + 2",
            "let b = a * 2"
        ].join("\n"),
    };

    private url: string;

    public constructor(url: string)
    {
        this.url = url;
    }

    public getLastWorkflows(count: number): Promise<string[]>
    {
        throw Error("Not implemented");
    }

    public getAllWorkflows(): Promise<string[]>
    {
        throw Error("Not implemented");
    }

    public getWorkflowVersions(name: string): Promise<string[]>
    {
        return new Promise<string[]>((resolve, reject) =>
        {
            resolve(Object.keys(WorkflowManager.sources));
        });
    }

    public getSource(name: string, version?: string): Promise<string>
    {
        return new Promise<string>((resolve, reject) =>
        {
            if (version)
                resolve(WorkflowManager.sources[version]);
            else
            {
                let keys = Object.keys(WorkflowManager.sources);
                resolve(WorkflowManager.sources[keys[keys.length - 1]]);
            }
        });
    }

    public async createVersion(workflowName: string, version: string, content: string)
    {
        WorkflowManager.sources[version] = content;
    }

    public async createWorkflow(workflowName: string, content: string, version?: string)
    {
        throw Error("Not implemented");
    }
}