import * as React from 'react';
import { ITestItem, TestList } from './TestList';
import { IPayment, PaymentSelection } from './PaymentSelection';

declare var window: any;

interface IOrderState {
    payment: IPayment;
    testItems: Array<ITestItem>;
}

export default class OrderForm extends React.Component<undefined, IOrderState> {
    state = { payment: { clientType: 'other' }, testItems: window.App.orderData.testItems };

    onPaymentSelected = (payment: any) => {
        this.setState({ ...this.state, payment });
    }
    render() {
        return (
            <div>
                <PaymentSelection payment={this.state.payment} onPaymentSelected={this.onPaymentSelected} />
                <TestList items={this.state.testItems} payment={this.state.payment} />
            </div>
        );
    }
}