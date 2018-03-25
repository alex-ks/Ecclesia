import * as React from "react"
import { Switch, Route } from "react-router-dom"
import * as Bootstrap from "reactstrap"

import Home from "src/components/Home";
import Editor from "src/components/Editor"
import Navigation from "src/components/Navigation"
import Monitor from "src/components/Monitor"

import "./App.css"


interface IAppProps
{
    compilerUrl: string;
    managementUrl: string;
}

interface IAppState { }

const TestUser = "UserTest"

export class App extends React.Component<IAppProps, IAppState>
{
    constructor(props: IAppProps) 
    {
        super(props);
        this.state = { };
    }

    render() : React.ReactNode
    {
        let EditorReady = () => (
            <Editor 
                compilerUrl={this.props.compilerUrl} 
                managementUrl={this.props.managementUrl}
                user={TestUser} />
        );

        let Blank = () => (
            <div>
                Hello, world!
            </div>
        );

        return (
            <div className="container">
                <Navigation brand="Ecclesia" >
                {[
                    { to: "/", label: "Home" },
                    { to: "/editor", label: "Workflow editor" },
                    { to: "/sessions", label: "Session monitor" }
                ]}
                </Navigation>
                <Switch>
                    <Route 
                        exact path="/"
                        component={Home} />
                    <Route 
                        path="/editor"
                        render={props => 
                            <Editor 
                                compilerUrl={this.props.compilerUrl} 
                                managementUrl={this.props.managementUrl}
                                user={TestUser} />} 
                        />
                    <Route 
                        path="/sessions"
                        render={props =>
                            <Monitor
                                user={TestUser}
                                managementUrl={this.props.managementUrl} />} 
                        />
                </Switch>
            </div>
        );
    }
}