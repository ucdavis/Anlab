import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter, Switch, Route } from 'react-router-dom';

import './css/site.scss';

const rootElement = document.getElementById('root');

if (rootElement) {
  // <React.StrictMode> should be used when possible.  ReactStrap will need to update context API usage first
  ReactDOM.render(
    <BrowserRouter>
      <React.Fragment>
        <Switch>
          {/* Match any server-side routes and send empty content to let MVC return the view details */}
          <Route path='/:team/Invoices/Create' component={() => <p>hello world</p>} />
        </Switch>
      </React.Fragment>
    </BrowserRouter>,
    rootElement
  );
}
