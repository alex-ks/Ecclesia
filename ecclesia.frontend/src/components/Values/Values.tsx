import * as React from "react"
import * as Bootstrap from "reactstrap"

import IDictionary from "src/models/IDictionary"
import { IMnemonicValue } from "src/models/ComputationGraph"

import "./Values.css"


interface IValuesProps
{
    values: IDictionary<IMnemonicValue>;
}

export class Values extends React.Component<IValuesProps>
{
    render()
    {
        let elements: JSX.Element[] = []
        for (let label in this.props.values)
        {
            if (label.indexOf("@") == 0)
                continue;
            elements.push(
                <tr>
                    <td>{label}</td>
                    <td>{this.props.values[label].value}</td>
                </tr>
            );
        }

        return (
            <Bootstrap.UncontrolledDropdown>
                <Bootstrap.DropdownToggle color="light" className="fullWidth" caret >
                    Values
                </Bootstrap.DropdownToggle>
                <Bootstrap.DropdownMenu >
                    <div className="dropDown">
                        <Bootstrap.Table>
                            <thead>
                                <tr>
                                    <th>Name</th>
                                    <th>Value</th>
                                </tr>
                            </thead>
                            <tbody>
                                {elements}
                            </tbody>
                        </Bootstrap.Table>
                    </div>
                </Bootstrap.DropdownMenu>
            </Bootstrap.UncontrolledDropdown>
        );
    }
}
