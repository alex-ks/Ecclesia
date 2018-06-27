import * as React from "react"
import * as Bootstrap from "reactstrap"
import { IComputationGraph } from "src/models/ComputationGraph"
import { UnControlled as CodeMirror } from "react-codemirror2"

import WorkflowManager from "src/services/WorkflowManager"
import Compiler from "src/services/Compiler"

import "./Editor.css"
import "codemirror/lib/codemirror.css"
import "codemirror/theme/neat.css"
import "codemirror/theme/material.css"
import "codemirror/mode/mllike/mllike"

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
    nameChanged: boolean;
    saved: boolean;
    cursor: CodeMirror.Position;
    dropdownOpen: boolean;
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
            codeStatusMessage: "",
            nameChanged: props.name ? false : true,
            saved: props.name ? true : false,
            cursor: null,
            dropdownOpen: false
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

    compileCode = (code: string) =>
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

    handleChange = async (editor: CodeMirror.Editor, data: CodeMirror.EditorChange, value: string) => 
    {
        this.setState({ source: value, codeStatus: CodeSubmitingStatus.Editing, cursor: data.to });
        let compiler = new Compiler(this.props.compilerUrl);
        let checkResult = await compiler.partialCheck(value);
        if (checkResult.name != this.state.name)
            this.setState({ nameChanged: true, name: checkResult.name });
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
            <div id="codeForm">
                <Bootstrap.Row>
                    <Bootstrap.Col>
                        <Bootstrap.Row>
                            <Bootstrap.Col xs="auto">
                                <Bootstrap.Label
                                    for="source"
                                    id="codeSubmitLabel">
                                    {this.state.name ? this.state.name : "Enter your code here:"}
                                </Bootstrap.Label>
                            </Bootstrap.Col>
                            <Bootstrap.Col>
                                <Bootstrap.Row id="toolbarRow">
                                    <Bootstrap.Col xs="auto">
                                        <Bootstrap.Label 
                                            className="toolbarElement"
                                            style={{color: "#0000007F"}}>
                                            Changes are not saved
                                        </Bootstrap.Label>
                                    </Bootstrap.Col>
                                    <Bootstrap.Col xs="auto">
                                        <Bootstrap.Button
                                            outline
                                            className="toolbarElement">
                                            Save
                                        </Bootstrap.Button>
                                    </Bootstrap.Col>
                                    <Bootstrap.Col xs="auto">
                                        <Bootstrap.Dropdown 
                                            id="versionsList"
                                            isOpen={this.state.dropdownOpen}
                                            toggle={() => this.setState(
                                                prev => ({ dropdownOpen: !prev.dropdownOpen }))}
                                            className="toolbarElement">
                                            <Bootstrap.DropdownToggle caret outline>
                                                1.0.1
                                            </Bootstrap.DropdownToggle>
                                            <Bootstrap.DropdownMenu>
                                                <Bootstrap.DropdownItem>1.0.0</Bootstrap.DropdownItem>
                                                <Bootstrap.DropdownItem>1.0.1</Bootstrap.DropdownItem>
                                            </Bootstrap.DropdownMenu>
                                        </Bootstrap.Dropdown>
                                    </Bootstrap.Col>
                                </Bootstrap.Row>
                            </Bootstrap.Col>                      
                        </Bootstrap.Row>
                        <CodeMirror 
                            className="codemirrorArea"
                            cursor={this.state.cursor}
                            options={{ mode: "mllike", theme: "neat", lineNumbers: true }}
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
                    </Bootstrap.Col>
                    <Bootstrap.Col xs="3">
                        <Bootstrap.Jumbotron 
                            fluid
                            style={{ height: "100%" }}>
                            <Bootstrap.Container fluid>
                                <p className="lead">
                                    Suggestions
                                </p>
                                <Bootstrap.Card>
                                    <Bootstrap.CardBody>
                                        <Bootstrap.CardTitle>fast-ICA</Bootstrap.CardTitle>
                                        <Bootstrap.CardText>
                                            A fast algorithm for Independent Component Analysis
                                        </Bootstrap.CardText>
                                        <Bootstrap.Button>Insert</Bootstrap.Button>
                                    </Bootstrap.CardBody>
                                </Bootstrap.Card>
                            </Bootstrap.Container>
                        </Bootstrap.Jumbotron>
                    </Bootstrap.Col>
                </Bootstrap.Row>
            </div>
        );
    }
}
