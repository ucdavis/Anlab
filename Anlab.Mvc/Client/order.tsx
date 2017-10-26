import * as React from "react";
import * as ReactDOM from "react-dom";
import OrderForm, { IOrderFormProps } from "./components/OrderForm";

declare var window: any;

// build defaults and test list from window
const props = {
    defaultAccount: window.App.defaultAccount,
    defaultClientId: window.App.defaultClientId,
    defaultEmail: window.App.defaultEmail,
    testItems: window.App.orderData.testItems,
    internalProcessingFee: window.App.orderData.internalProcessingFee,
    externalProcessingFee: window.App.orderData.externalProcessingFee,
    orderId: window.App.orderId,
} as IOrderFormProps;

// existing order info
if (window.App.orderData.order) {
    props.orderInfo = JSON.parse(window.App.orderData.order.jsonDetails);
}

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
