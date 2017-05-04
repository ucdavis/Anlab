import * as React from 'react';
import { ITestItem, TestList } from './TestList';
import { IPayment, PaymentSelection } from './PaymentSelection';
import { SampleTypeSelection } from './SampleTypeSelection';

declare var window: any;

interface IOrderState {
    payment: IPayment;
    sampleType: string;
    testItems: Array<ITestItem>;
}

export default class OrderForm extends React.Component<undefined, IOrderState> {
    state = { payment: { clientType: 'uc' }, sampleType: 'Soil', testItems: window.App.orderData.testItems };

    onPaymentSelected = (payment: any) => {
        this.setState({ ...this.state, payment });
    }
    onSampleSelected = (sampleType: any) => {
        this.setState({ ...this.state, sampleType });
    }
    render() {
        return (
            <div>
                <PaymentSelection payment={this.state.payment} onPaymentSelected={this.onPaymentSelected} />
                <div>
                    <label>Select Sample Type:</label>
                    <SampleTypeSelection sampleType={this.state.sampleType} onSampleSelected={this.onSampleSelected} />
                </div>
                <TestList items={this.state.testItems} payment={this.state.payment} sampleType={this.state.sampleType} />
            </div>
        );
    }
}