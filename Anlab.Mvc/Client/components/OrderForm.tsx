import * as React from 'react';
import { ITestItem, TestList } from './TestList';
import { IPayment, PaymentSelection } from './PaymentSelection';
import { SampleTypeSelection } from './SampleTypeSelection';

declare var window: any;

interface IOrderState {
    payment: IPayment;
    sampleType: string;
    testItems: Array<ITestItem>;
    selectedTests: any;
}

export default class OrderForm extends React.Component<undefined, IOrderState> {
    state = {
        payment: { clientType: 'uc' },
        sampleType: 'Soil',
        testItems: window.App.orderData.testItems,
        selectedTests: { 1: true, 2: true }
    };

    onPaymentSelected = (payment: any) => {
        this.setState({ ...this.state, payment });
    }
    onSampleSelected = (sampleType: string) => {
        this.setState({ ...this.state, sampleType });
    }
    onTestSelectionChanged = (test: ITestItem, selected: Boolean) => {
        this.setState({
            ...this.state,
            selectedTests: {
                ...this.state.selectedTests,
                [test.id]: selected
            }
        });
    }
    render() {
        const { testItems, payment, selectedTests, sampleType } = this.state;
        
        return (
            <div>
                <PaymentSelection payment={payment} onPaymentSelected={this.onPaymentSelected} />
                <div>
                    <label>Select Sample Type:</label>
                    <SampleTypeSelection sampleType={sampleType} onSampleSelected={this.onSampleSelected} />
                </div>
                <TestList items={testItems} payment={payment} selectedTests={selectedTests} sampleType={sampleType} onTestSelectionChanged={this.onTestSelectionChanged} />
            </div>
        );
    }
}