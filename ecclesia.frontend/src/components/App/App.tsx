import * as React from "react"
import { NavLink, Switch, Route, Link } from "react-router-dom"
import * as Bootstrap from "reactstrap"

import Editor from "src/components/Editor"
import Navigation from "src/components/Navigation"

import "./App.css"

enum AppMode
{
    Home, Editor, View
}

interface IAppProps
{
    compilerUrl: string;
    managementUrl: string;
}

interface IAppState 
{
    mode: AppMode;
}

export class App extends React.Component<IAppProps, IAppState>
{
    constructor(props: IAppProps) 
    {
        super(props);
        this.state = { mode: AppMode.Home };
    }

    render() : React.ReactNode
    {
        let EditorReady = () => (
            <Editor 
                compilerUrl={this.props.compilerUrl} 
                managementUrl={this.props.managementUrl}
                user="UserTest" />
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
                        component={EditorReady} />
                    <Route 
                        path="/editor"
                        component={EditorReady} />
                    <Route 
                        path="/sessions"
                        component={Blank} />
                </Switch>
            </div>
        );
    }
}