import * as React from "react"
import * as Bootstrap from "reactstrap"

import { ISessionStatus, SessionState, getState } from "src/models/SessionStatus"

import "./Session.css"

interface ISessionProps
{
    status: ISessionStatus;
}

function getBadge(state: SessionState)
{
    switch (state)
    {
        case SessionState.Running:
            return <Bootstrap.Badge color="primary">running</Bootstrap.Badge>;
        
        case SessionState.Completed:
            return <Bootstrap.Badge color="success">completed</Bootstrap.Badge>;

        case SessionState.Failed:
            return <Bootstrap.Badge color="danger">failed</Bootstrap.Badge>;

        case SessionState.Aborted:
            return <Bootstrap.Badge color="warning">aborted</Bootstrap.Badge>;

        default:
            return <Bootstrap.Badge color="dark">unknown</Bootstrap.Badge>;
    }
}

function serializeDate(date: Date)
{
    const options = 
    {
        weekday: "long", 
        year: "numeric", 
        month: "short",  
        day: "numeric", 
        hour: "2-digit", 
        minute: "2-digit"
    };
    return date.toLocaleTimeString([], options);
}

export class Session extends React.Component<ISessionProps>
{
    render()
    {
        return (
            <Bootstrap.UncontrolledDropdown>
                <Bootstrap.DropdownToggle className="dropToggle" color="light" caret >
                    Session #{this.props.status.sessionId} {getBadge(getState(this.props.status))}
                </Bootstrap.DropdownToggle>
                <Bootstrap.DropdownMenu>
                    <div className="dropDown fullWidth">
                        <div className="fullWidth">
                            Start time: {serializeDate(this.props.status.startTime as Date)}
                        </div>
                    </div>
                </Bootstrap.DropdownMenu>
            </Bootstrap.UncontrolledDropdown>
        )
    }
}