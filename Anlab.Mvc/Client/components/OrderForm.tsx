import * as React from 'react';
import { ITestItem, TestList } from './TestList';
import { IPayment, PaymentSelection } from './PaymentSelection';
import { SampleTypeSelection } from './SampleTypeSelection';
import { Quantity } from './Quantity';
import { Summary } from './Summary';

declare var window: any;
declare var $: any;

interface IOrderState {
    payment: IPayment;
    quantity?: number;
    sampleType: string;
    testItems: Array<ITestItem>;
    selectedTests: any;
    total: number;
    isValid: boolean;
}

export default class OrderForm extends React.Component<undefined, IOrderState> {
    constructor(props) {
        super(props);

        this.state = {
            payment: { clientType: 'uc' },
            quantity: null,
            sampleType: 'Soil',
            testItems: window.App.orderData.testItems,
            selectedTests: { 1: true, 2: false },
            total: 0,
            isValid: false,
        };
    }
    validate = () => {
        const valid = this.state.quantity > 0;
        this.setState({ ...this.state, isValid: valid });
    }
    onPaymentSelected = (payment: any) => {
        this.setState({ ...this.state, payment }, this.validate);
    }
    onSampleSelected = (sampleType: string) => {
        this.setState({ ...this.state, sampleType }, this.validate);
    }
    onTestSelectionChanged = (test: ITestItem, selected: Boolean) => {
        this.setState({
            ...this.state,
            selectedTests: {
                ...this.state.selectedTests,
                [test.id]: selected
            }
        }, this.validate);
    }
    onQuantityChanged = (quantity?: number) => {
        this.setState({ ...this.state, quantity }, this.validate);
    }
    onSubmit = () => {
        $.post({
            url: '/order/create',
            data: JSON.stringify(this.state),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        });
    }
    render() {
        const { testItems, payment, selectedTests, sampleType, quantity } = this.state;
        const filteredTests = testItems.filter(item => item.category === sampleType);

        const selectedItems = filteredTests.filter(item => !!selectedTests[item.id]);

        return (
            <div className="row">
                <div className="col-lg-8">
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
                    <div style={{ height: 600 }}></div>
                </div>
                <div className="col-lg-4">
                    <div data-spy="affix" data-offset-top="60" data-offset-bottom="200">
                        <Summary canSubmit={this.state.isValid} testItems={selectedItems} quantity={quantity} payment={payment} onSubmit={this.onSubmit} />
                    </div>
                </div>
            </div>
        );
    }
}