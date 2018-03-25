import * as React from "react"
import * as Bootstrap from "reactstrap"

import { IOperationStatus, OperationState } from "src/models/SessionStatus"

import "./Operations.css"


function getBadge(op: OperationState)
{
    switch (op)
    {
        case OperationState.Awaits:
            return <Bootstrap.Badge color="info">awaits</Bootstrap.Badge>;

        case OperationState.Running:
            return <Bootstrap.Badge color="primary">running</Bootstrap.Badge>;
        
        case OperationState.Completed:
            return <Bootstrap.Badge color="success">completed</Bootstrap.Badge>;

        case OperationState.Failed:
            return <Bootstrap.Badge color="danger">failed</Bootstrap.Badge>;

        case OperationState.Aborted:
            return <Bootstrap.Badge color="warning">aborted</Bootstrap.Badge>;

        default:
            return <Bootstrap.Badge color="dark">unknown</Bootstrap.Badge>;
    }
}

interface IOperationsProps
{
    operations: IOperationStatus[];
}

export class Operations extends React.Component<IOperationsProps>
{
    render()
    {
        let operations = this.props.operations.map((op, index) =>
        {
            return (
                <Bootstrap.ListGroupItem key={index.toString()}>
                    Operation #{op.id} "{op.name}"
                    {getBadge(op.state)}
                </Bootstrap.ListGroupItem>
            );
        });

        return (
            <Bootstrap.UncontrolledDropdown className="centerElement">
                <Bootstrap.DropdownToggle color="light" className="fullWidth" caret >
                    Operations
                </Bootstrap.DropdownToggle>
                <Bootstrap.DropdownMenu >
                    <div className="dropDown">
                        <Bootstrap.ListGroup>
                            {operations}
                        </Bootstrap.ListGroup>
                    </div>
                </Bootstrap.DropdownMenu>
            </Bootstrap.UncontrolledDropdown>
        );
    }
}