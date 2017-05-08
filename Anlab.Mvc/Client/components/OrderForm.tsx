import * as React from 'react';
import { ITestItem, TestList } from './TestList';
import { IPayment, PaymentSelection } from './PaymentSelection';
import { SampleTypeSelection } from './SampleTypeSelection';
import { Quantity } from './Quantity';
import { Summary } from './Summary';

declare var window: any;

interface IOrderState {
    payment: IPayment;
    quantity?: number;
    sampleType: string;
    testItems: Array<ITestItem>;
    selectedTests: Array<ITestItem>;
    total: number;
}

export default class OrderForm extends React.Component<undefined, IOrderState> {
    constructor(props) {
        super(props);

        this.state = {
            payment: { clientType: 'uc' },
            quantity: null,
            sampleType: 'Soil',
            testItems: window.App.orderData.testItems,
            selectedTests: [],
            total: 0
        };
    }

    onPaymentSelected = (payment: any) => {
        this.setState({ ...this.state, payment });
    }
    onSampleSelected = (sampleType: string) => {
        this.setState({ ...this.state, sampleType });
    }
    onTestSelectionChanged = (selectedTests: Array<ITestItem>) => {
        this.setState({
            ...this.state,
            selectedTests
        });
    }
    onQuantityChanged = (quantity?: number) => {
        this.setState({ ...this.state, quantity });
    }
    render() {
        const { testItems, payment, selectedTests, sampleType, quantity } = this.state;
        const filteredTests = testItems.filter(item => item.category === sampleType);
        const selectedItems = selectedTests.filter(item => item.category === sampleType);

        return (
            <div>
                <PaymentSelection payment={payment} onPaymentSelected={this.onPaymentSelected} />
                <div>
                    <label>Select Sample Type:</label>
                    <SampleTypeSelection sampleType={sampleType} onSampleSelected={this.onSampleSelected} />
                </div>
                <div>
                    <label>Quantity:</label>
                    <Quantity quantity={quantity} onQuantityChanged={this.onQuantityChanged} />
                </div>
                <TestList items={filteredTests} payment={payment} selectedTests={selectedTests} onTestSelectionChanged={this.onTestSelectionChanged} />
                <Summary testItems={selectedItems} quantity={quantity} payment={payment} />
            </div>
        );
    }
}