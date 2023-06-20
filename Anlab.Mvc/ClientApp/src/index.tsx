import React from "react";
import ReactDOM from "react-dom";
import { BrowserRouter, Switch, Route } from "react-router-dom";

import "bootstrap/dist/css/bootstrap.css";
import "./css/site.scss";
import { Order } from "./order";

const rootElement = document.getElementById("root");

if (rootElement) {
  // <React.StrictMode> should be used when possible.  ReactStrap will need to update context API usage first
  ReactDOM.render(
    <BrowserRouter>
      <Switch>
        {/* Match any server-side routes and send empty content to let MVC return the view details */}
        <Route exact path="/Order/Create" component={() => <Order></Order>} />
        <Route exact path="/Order/Edit/:id" component={() => <Order></Order>} />
        <Route path="*">
          <div></div>
        </Route>
      </Switch>
    </BrowserRouter>,
    rootElement
  );
}
