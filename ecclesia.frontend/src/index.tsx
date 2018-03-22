import * as React from "react"
import * as ReactDOM from "react-dom"

import { App } from "./components/App"

ReactDOM.render(
    <App 
        compilerUrl="http://ecclesia.ict.nsc.ru:27944" 
        managementUrl="http://ecclesia.ict.nsc.ru:27942"/>, 
    document.getElementById("root"));
