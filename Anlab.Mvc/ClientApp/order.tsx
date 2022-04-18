import * as React from "react";
import * as ReactDOM from "react-dom";
import OrderForm, { IOrderFormProps } from "./components/OrderForm";

declare var window: any;

// build defaults and test list from window
const props = {
    defaultAccount: window.App.defaults.defaultAccount,
    defaultClientId: window.App.defaults.defaultClientId,
    defaultClientIdName: window.App.defaults.defaultClientIdName,
    defaultEmail: window.App.defaults.defaultEmail,
    defaultCopyEmail: window.App.defaults.defaultCopyEmail,
    defaultSubEmail: window.App.defaults.defaultSubEmail,
    defaultAcName: window.App.defaults.defaultAcName,
    defaultAcAddr: window.App.defaults.defaultAcAddr,
    defaultAcPhone: window.App.defaults.defaultAcPhone,
    defaultAcEmail: window.App.defaults.defaultAcEmail,
    defaultCompanyName: window.App.defaults.defaultCompanyName,

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
