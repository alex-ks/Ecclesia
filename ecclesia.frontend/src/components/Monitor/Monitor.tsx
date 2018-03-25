import * as Path from "path"
import * as React from "react"
import * as Bootstrap from "reactstrap"
import { ISessionStatus, OperationState } from "src/models/SessionStatus"

import Session from "src/components/Session"

interface IMonitorProps
{
    user: string;
    managementUrl: string;
}

interface IMonitorState
{
    sessions: ISessionStatus[];
    error: string;
    hasErrors: boolean;
}

const PollTimeout = 1000;

export class Monitor extends React.Component<IMonitorProps, IMonitorState>
{    
    private timerId: any;

    constructor(props: IMonitorProps)
    {
        super(props);
        this.state = { sessions: [], error: "", hasErrors: false };
    }

    private async loadSessions()
    {
        return new Promise<ISessionStatus[]>((resolve, reject) =>
        {
            let requester = new XMLHttpRequest();
            let url = Path.join(this.props.managementUrl, "/api/sessions");

            try
            {
                requester.open('GET', url, false);
                // TODO: add real authorization
                requester.setRequestHeader("Authorization", this.props.user);
                requester.send();
                if (requester.status == 200)
                {
                    resolve(JSON.parse(requester.responseText) as ISessionStatus[]);
                }
                else
                    reject(new Error([requester.status, requester.responseText].join(": ")));
            }
            catch (e)
            {
                let error = e as Error;
                reject(new Error(`Cannot get sessions: ${error.message}`));
            }
        });
    }

    updateSessions = async () =>
    {
        try
        {
            let sessions = await this.loadSessions();
            for (let session of sessions)
            {
                session.startTime = new Date(session.startTime as string);
            }
            this.setState({ hasErrors: false, sessions: sessions });
        }
        catch (e)
        {
            let error = e as Error;
            this.setState({ hasErrors: true, error: error.message });
        }
    }

    componentWillMount()
    {
        this.timerId = setInterval(this.updateSessions, PollTimeout);
        this.updateSessions();
    }

    componentWillUnmount()
    {
        clearInterval(this.timerId);
    }

    render()
    {
        let sessions = this.state.sessions.map((session, index) => (
            <Bootstrap.ListGroupItem key={index.toString()}>
                <Session status={session} />
            </Bootstrap.ListGroupItem>
        ));

        return (
            <div id="monitor">
                <Bootstrap.Alert 
                    id="pollingStatus"
                    color="danger"
                    hidden={!this.state.hasErrors}>
                    {this.state.error}
                </Bootstrap.Alert>
                <Bootstrap.ListGroup>
                    {sessions}
                </Bootstrap.ListGroup>
            </div>
        )
    }
}