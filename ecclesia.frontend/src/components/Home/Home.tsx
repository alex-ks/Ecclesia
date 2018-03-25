import * as React from "react"
import * as Bootstrap from "reactstrap";


const EditorMode = 1;
const SessionsMode = 2;

export class Home extends React.Component
{
    render()
    {
        return (
            <div>
                <Bootstrap.Jumbotron fluid>
                    <Bootstrap.Container fluid>
                        <h1 className="display-3">
                            Ecclesia - workflow management system
                        </h1>
                        <p className="lead">
                            Write some workflow or see your active and passed sessions
                        </p>
                        <p className="lead">
                            <Bootstrap.Button
                                href="#/editor"
                                style={{margin: "0em 0.5em 0em 0em"}}>
                                Open Editor
                            </Bootstrap.Button>
                            <Bootstrap.Button
                                href="#/sessions"
                                style={{margin: "0em 0em 0em 0.5em"}}>
                                Explore sessions
                            </Bootstrap.Button>
                        </p>
                        <hr />
                        <p>
                            Feel free to investigate code or submit issues in <a href="https://github.com/alex-ks/Ecclesia">our repository</a>.
                        </p>
                    </Bootstrap.Container>
                </Bootstrap.Jumbotron>
            </div>
        );
    }
}