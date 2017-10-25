import * as React from "react";
import * as ReactDOM from "react-dom";
import OrderForm, { IOrderFormProps } from "./components/OrderForm";

declare var window: any;

// build test list form window
const props = {
    testItems: window.App.orderData.testItems,
    defaultAccount: window.App.defaultAccount,
    defaultEmail: window.App.defaultEmail,
    defaultClientId: window.App.defaultClientId,
} as IOrderFormProps;

function renderApp() {
    ReactDOM.render(
        <OrderForm {...props} />,
        document.getElementById("react-app"),
    );
}

renderApp();

// Allow Hot Module Replacement
if (module.hot) {
    module.hot.accept("./components/OrderForm", () => {
        renderApp();
    });
}
