import * as React from "react"
import * as ReactDOM from "react-dom"
import { HashRouter } from "react-router-dom";

import App from "./components/App"

ReactDOM.render(
    <HashRouter>
        <App 
            compilerUrl="http://ecclesia.ict.nsc.ru:27944" 
            managementUrl="http://ecclesia.ict.nsc.ru:27942"/>
    </HashRouter>, 
    document.getElementById("root"));
