import * as React from "react"
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
    vertical?: boolean;
    className?: string;
    children: ILink[];
}

interface INavigationState 
{
    selectionId: number;
    isOpen: boolean;
}

export class Navigation extends React.Component<INavigationProps, INavigationState>
{
    constructor(props: INavigationProps) 
    {
        super(props);
        this.state = { selectionId: -1, isOpen: false };
    }

    // TODO: how make it right?
    select = (id: number) => this.setState({ selectionId: id });

    toggle = () => this.setState(prev => ({ isOpen: !prev.isOpen }));

    render() : React.ReactNode
    {
        let navClass = "navBar";
        
        if (this.props.className)
            navClass = [navClass, this.props.className].join(" ");
        
        let items = this.props.children.map((child, index) => (
            <Bootstrap.NavItem>
                <Bootstrap.NavLink 
                    href={`#${child.to}`}
                    onClick={() => this.select(index)}
                    active={this.state.selectionId == index}>
                    {child.label}
                </Bootstrap.NavLink>
            </Bootstrap.NavItem>
        ));

        return (
            <Bootstrap.Navbar className={navClass} color="faded" light expand="md">
                <Bootstrap.NavbarBrand href="/">{this.props.brand}</Bootstrap.NavbarBrand>
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