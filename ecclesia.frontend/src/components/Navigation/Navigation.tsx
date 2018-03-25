import * as React from "react"
import * as Bootstrap from "reactstrap"

import "./Navigation.css"


enum SelectionMode
{
    Home, Editor, View, None
}

interface INavigationProps 
{
    vertical?: boolean;
    className?: string;
}

interface INavigationState 
{
    mode: SelectionMode;
}

export class Navigation extends React.Component<INavigationProps, INavigationState>
{
    constructor(props: INavigationProps) 
    {
        super(props);
        this.state = { mode: SelectionMode.None };
    }

    // TODO: how make it right?
    homeSelected = (e: any) => this.setState({ mode: SelectionMode.Home })
    editorSelected = (e: any) => this.setState({ mode: SelectionMode.Editor })
    viewSelected = (e: any) => this.setState({ mode: SelectionMode.View })

    render() : React.ReactNode
    {
        let navClass = "navBar";
        
        if (this.props.className)
            navClass = [navClass, this.props.className].join(" ");

        return (
            <Bootstrap.Nav className={navClass} pills vertical={this.props.vertical}>
                <Bootstrap.NavItem>
                    <Bootstrap.NavLink 
                        href="#/"
                        onClick={this.homeSelected}
                        active={this.state.mode == SelectionMode.Home}>
                        Home
                    </Bootstrap.NavLink>
                </Bootstrap.NavItem>
                <Bootstrap.NavItem>
                    <Bootstrap.NavLink 
                        href="#/edit"
                        onClick={this.editorSelected}
                        active={this.state.mode == SelectionMode.Editor}>
                        Edit workflow
                    </Bootstrap.NavLink>
                </Bootstrap.NavItem>
                <Bootstrap.NavItem>
                    <Bootstrap.NavLink 
                        href="#/view"
                        onClick={this.viewSelected}
                        active={this.state.mode == SelectionMode.View}>
                        View sessions
                    </Bootstrap.NavLink>
                </Bootstrap.NavItem>
            </Bootstrap.Nav>
        );
    }
}