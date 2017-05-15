import * as React from 'react';
import { ITestItem, TestList } from './TestList';
import { IPayment, PaymentSelection } from './PaymentSelection';
import { SampleTypeSelection } from './SampleTypeSelection';
import { Quantity } from './Quantity';
import { Summary } from './Summary';
import { AdditionalInfo } from './AdditionalInfo';
import { Project } from "./project";
declare var window: any;
declare var $: any;

interface IOrderState {
    orderId?: number;
    additionalInfo: string;
    project: string;
    payment: IPayment;
    quantity?: number;
    sampleType: string;
    testItems: Array<ITestItem>;
    selectedTests: any;
    isValid: boolean;
    isSubmitting: boolean;
}

export default class OrderForm extends React.Component<undefined, IOrderState> {
    constructor(props) {
        super(props);

        const initialState = {
            orderId: null,
            additionalInfo: '',
            payment: { clientType: 'uc' },
            quantity: null,
            sampleType: 'Soil',
            testItems: window.App.orderData.testItems,
            selectedTests: { },
            isValid: false,
            isSubmitting: false,
            project: '',
        };

        if (window.App.orderData.order) {
            // load up existing order
            const orderInfo = JSON.parse(window.App.orderData.order.jsonDetails);

            initialState.quantity = orderInfo.Quantity;
            initialState.additionalInfo = orderInfo.AdditionalInfo;
            initialState.sampleType = orderInfo.SampleType;
            initialState.orderId = orderInfo.Id;
            initialState.project = window.App.orderData.order.Project;
            //TODO: save and load additional info
            
            orderInfo.SelectedTests.forEach(test => { initialState.selectedTests[test.Id] = true; } );
        }

        this.state = { ...initialState };
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

    handleChange = (name, value) => {
        this.setState({ ...this.state, [name]: value }, this.validate);
    };

    getTests = () => {
        const { testItems, payment, selectedTests, sampleType, quantity } = this.state;
        const filtered = testItems.filter(item => item.category === sampleType);
        return {
            filtered,
            selected: filtered.filter(item => !!selectedTests[item.id])
        };
    }
    onSubmit = () => {
        
        if (this.state.isSubmitting) {
            return;
        }
        this.setState({ ...this.state, isSubmitting: true });
        const selectedTests = this.getTests().selected;
        const order = {
            quantity: this.state.quantity,
            additionalInfo: this.state.additionalInfo,
            project: this.state.project,
            payment: this.state.payment,
            sampleType: this.state.sampleType,
            selectedTests,
        }
        var that = this;
        $.post({
            url: '/order/create',
            data: JSON.stringify(order),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        }).success(function (response) {
            if (response.success === true) {
                var redirectId = response.id;
                window.location.replace("/Order/Confirmation/" + redirectId);
            } else {
                alert("There was a problem saving your order.");
                that.setState({ ...this.state, isSubmitting: false });
            }
            
            }).error(function () {
                alert("An error occured...");
                that.setState({ ...this.state, isSubmitting: false });
                        
        });
    }
    render() {
        const { testItems, payment, selectedTests, sampleType, quantity, additionalInfo, project } = this.state;
        
        const { filtered, selected} = this.getTests();

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
                    <Project project={project} handleChange={this.handleChange} />
                    <AdditionalInfo additionalInfo={additionalInfo} handleChange={this.handleChange} />
                    <TestList items={filtered} payment={payment} selectedTests={selectedTests} onTestSelectionChanged={this.onTestSelectionChanged} />
                    <div style={{ height: 600 }}></div>
                </div>
                <div className="col-lg-4">
                    <div data-spy="affix" data-offset-top="60" data-offset-bottom="200">
                        <Summary canSubmit={this.state.isValid && !this.state.isSubmitting} testItems={selected} quantity={quantity} payment={payment} onSubmit={this.onSubmit} />
                    </div>
                </div>
            </div>
        );
    }
}