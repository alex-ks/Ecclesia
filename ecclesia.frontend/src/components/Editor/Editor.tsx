import * as React from "react"
import * as Bootstrap from "reactstrap"
import * as Path from "path"
import { IComputationGraph } from "src/models/ComputationGraph"

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
}

interface IEditorState
{
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
            source: sampleCode, 
            codeStatus: CodeSubmitingStatus.Editing, 
            codeStatusMessage: ""
        };
    }

    compileCode(code: string)
    {
        return new Promise<string>((resolve, reject) => 
        {
            let requester = new XMLHttpRequest();
            requester.open('POST', this.props.compilerUrl, false);
            requester.setRequestHeader("Content-Type", "application/json");
            requester.send(JSON.stringify({ source: code }));

            if (requester.status == 200)
                resolve(requester.responseText);
            else
                reject(new Error([requester.status, requester.responseText].join(": ")));
        });
    }

    requestExecution(graph: IComputationGraph)
    {
        return new Promise<void>((resolve, reject) => 
        {
            let requester = new XMLHttpRequest();
            let url = Path.join(this.props.managementUrl, "/api/sessions");

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

    handleChange = (event: React.ChangeEvent<HTMLInputElement>) => 
    {
        this.setState({ source: event.target.value, codeStatus: CodeSubmitingStatus.Editing });
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
                            onClick={this.handleSubmit}
                            color="danger">
                            Compile
                        </Bootstrap.Button>
                    </Bootstrap.FormGroup>
                </div>
            </Bootstrap.Form>
        );
    }
}
