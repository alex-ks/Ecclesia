import * as React from "react"
import * as ReactRouter from "react-router-dom"
import * as Bootstrap from "reactstrap"

import "./Navigation.css"


interface ILink
{
    to: string;
    label: string;
}

interface INavigationProps 
{
    brand: string;
    className?: string;
    children: ILink[];
}

interface INavigationState 
{
    isOpen: boolean;
}

export class Navigation extends React.Component<INavigationProps, INavigationState>
{
    constructor(props: INavigationProps) 
    {
        super(props);
        this.state = { isOpen: false };
    }

    // TODO: how make it right?
    getRoute = () =>
    {
        let fullPath = window.location.href;
        return fullPath.substr(fullPath.indexOf('#') + 1);
    }

    toggle = () => this.setState(prev => ({ isOpen: !prev.isOpen }));

    render() : React.ReactNode
    {
        let navClass = "navBar";
        
        if (this.props.className)
            navClass = [navClass, this.props.className].join(" ");
        
        let items = this.props.children.map((child, index) => (
            <li key={index.toString()}>
                <Bootstrap.NavItem>
                    <Bootstrap.NavLink 
                        href={`#${child.to}`}
                        active={this.getRoute() === child.to}>
                        {child.label}
                    </Bootstrap.NavLink>
                </Bootstrap.NavItem>
            </li>
        ));

        return (
            <Bootstrap.Navbar className={navClass} color="faded" light expand="md">
                <Bootstrap.NavbarBrand href="/">
                    {this.props.brand}
                </Bootstrap.NavbarBrand>
                <Bootstrap.NavbarToggler onClick={this.toggle} />
                <Bootstrap.Collapse isOpen={this.state.isOpen} navbar>
                    <Bootstrap.Nav className="ml-auto" navbar>
                        {items}
                    </Bootstrap.Nav>
                </Bootstrap.Collapse>
            </Bootstrap.Navbar>
        );
    }
}