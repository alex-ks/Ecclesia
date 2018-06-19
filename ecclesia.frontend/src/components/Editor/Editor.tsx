import * as React from "react"
import * as Bootstrap from "reactstrap"
import { IComputationGraph } from "src/models/ComputationGraph"

import WorkflowManager from "src/services/WorkflowManager"
import Compiler from "src/services/Compiler"

import "./Editor.css"

enum CodeSubmitingStatus
{
    Editing,
    Successfull,
    Failed
}

interface IEditorProps 
{
    compilerUrl: string;
    managementUrl: string;
    user: string;
    name?: string;
}

interface IEditorState
{
    name: string;
    version: string;
    source: string;
    codeStatus: CodeSubmitingStatus;
    codeStatusMessage: string;
}

export class Editor extends React.Component<IEditorProps, IEditorState>
{
    constructor(props: IEditorProps)
    {
        super(props);
        let sampleCode = [
            "module A",
            "let a = 1 + 1",
            "let b = a * 2"
        ].join("\n");
        this.state = 
        { 
            name: props.name ? props.name : null,
            version: props.name ? props.name : "1.0.0",
            source: sampleCode, 
            codeStatus: CodeSubmitingStatus.Editing, 
            codeStatusMessage: ""
        };

        if (props.name)
        {
            let manager = new WorkflowManager(props.managementUrl);
            manager.getWorkflowVersions(props.name)
                .then(versions => 
                    {
                        let version = versions[versions.length - 1];
                        this.setState({ version: version });
                        return manager.getSource(props.name, version);
                    })
                .then(code => this.setState({ source: code }));
        }
    }

    compileCode(code: string)
    {
        let compiler = new Compiler(this.props.compilerUrl);
        return compiler.compileCode(code);
    }

    requestExecution(graph: IComputationGraph)
    {
        return new Promise<void>((resolve, reject) => 
        {
            let requester = new XMLHttpRequest();
            let url = this.props.managementUrl + "/api/sessions";

            requester.open('POST', url, false);
            requester.setRequestHeader("Content-Type", "application/json");
            // TODO: add real authorization
            requester.setRequestHeader("Authorization", this.props.user);
            requester.send(JSON.stringify({ computationGraph: graph }));
            if (requester.status == 200)
                resolve();
            else
                reject(new Error([requester.status, requester.responseText].join(": ")));
        });
    }

    handleChange = async (event: React.ChangeEvent<HTMLInputElement>) => 
    {
        this.setState({ source: event.target.value, codeStatus: CodeSubmitingStatus.Editing });
        let compiler = new Compiler(this.props.compilerUrl);
        let checkResult = await compiler.partialCheck(event.target.value);
        if (checkResult.name != this.state.name)
        {
            let manager = new WorkflowManager(this.props.managementUrl);
            let version = "1.0.0";
            await manager.createWorkflow(checkResult.name, event.target.value, version);
            this.setState({ name: checkResult.name, version: version });
        }
    }

    handleSubmit = async (event: React.FormEvent<HTMLButtonElement>) =>
    {
        try
        {
            let result = await this.compileCode(this.state.source);
            let graph = JSON.parse(result) as IComputationGraph;
            await this.requestExecution(graph);
            this.setState(
            {
                codeStatus: CodeSubmitingStatus.Successfull,
                codeStatusMessage: "Workflow has been successfully started"
            });
        }
        catch (e)
        {
            let error = e as Error;
            if (error == null)
            {
                this.setState(
                { 
                    codeStatus: CodeSubmitingStatus.Failed, 
                    codeStatusMessage: "Something failed and we have absolutely no idea why"
                });
            }
            else
            {
                this.setState(
                { 
                    codeStatus: CodeSubmitingStatus.Failed, 
                    codeStatusMessage: error.message
                });
            }
        }
    }

    render()
    {
        return (
            <Bootstrap.Form>
                <div id="codeForm">
                    <Bootstrap.FormGroup>
                        <Bootstrap.Label 
                            for="source"
                            id="codeSubmitLabel">
                            Enter your source code:
                        </Bootstrap.Label>
                        <Bootstrap.Input 
                            type="textarea" 
                            id="source"
                            value={this.state.source}
                            onChange={this.handleChange} />
                        <Bootstrap.Alert 
                            id="submitStatus"
                            color={this.state.codeStatus == CodeSubmitingStatus.Successfull ? "success" : "danger"}
                            hidden={this.state.codeStatus == CodeSubmitingStatus.Editing}>
                            {this.state.codeStatusMessage}
                        </Bootstrap.Alert>
                        <Bootstrap.Button 
                            id="codeSubmitButton"
                            onClick={this.handleSubmit}>
                            Submit
                        </Bootstrap.Button>
                    </Bootstrap.FormGroup>
                </div>
            </Bootstrap.Form>
        );
    }
}
