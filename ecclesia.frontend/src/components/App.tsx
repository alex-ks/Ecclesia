import * as React from "react"

import Editor from "src/components/Editor/Editor"
import * as Bootstrap from "reactstrap"

enum EditorState
{
    Editing,
    SubmitSuccessfull,
    SubmitFailed
}

let allStates = [EditorState.Editing, EditorState.SubmitSuccessfull, EditorState.SubmitFailed];

interface IAppProps
{
    compilerUrl: string;
    managementUrl: string;
}

interface IAppState
{
    compiledCode: string;
    currentState: EditorState;
    stateIndex: number;
}

export class App extends React.Component<IAppProps, IAppState>
{
    constructor(props: IAppProps) {
        super(props);
        this.state = 
        { 
            compiledCode: "",
            currentState: EditorState.Editing,
            stateIndex: 0
        };
    }

    compileCode = (sourceCode: string) =>
    {
        // let requester = new XMLHttpRequest();
        // requester.open('POST', this.props.compilerUrl, false);
        // requester.setRequestHeader("Content-Type", "application/json");
        // requester.send(JSON.stringify({ source: sourceCode }));
        // let compiled = requester.responseText;
        // this.setState({ compiledCode: compiled });
        this.setState((prevState) => 
        {
            let newIndex = (prevState.stateIndex + 1) % 3;
            return ({  
                currentState: allStates[newIndex],
                stateIndex: newIndex
            });
        });
    }

    render() : React.ReactNode
    {
        let info: JSX.Element = null;
        
        if (this.state.currentState != EditorState.Editing)
        {
            let color = this.state.currentState == EditorState.SubmitSuccessfull 
                ? "success"
                : "danger";
            info = (
            <Bootstrap.Alert color={color}>
                {this.state.currentState == EditorState.SubmitFailed 
                    ? "Exorcizamus te, omnis immundus spiritus, omnis satanica potestas, omnis incursio infernalis adversarii, omnis legio, omnis congregatio et secta diabolica, in nomine et virtute Domini Nostri Jesu + Christi, eradicare et effugare a Dei Ecclesia, ab animabus ad imaginem Dei conditis ac pretioso divini Agni sanguine redemptis. +"
                    : "Ale!"}
            </Bootstrap.Alert>
            );
        }

        return (
            <div className="container">
                {/* <FsInput onInput={this.compileCode} />                
                {info} */}
                <Editor 
                    compilerUrl={this.props.compilerUrl} 
                    managementUrl={this.props.managementUrl}/>
            </div>
        );
    }
}