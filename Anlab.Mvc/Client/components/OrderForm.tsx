import * as React from 'react';
import Dialog from 'react-toolbox/lib/dialog';
import { ITestItem, TestList } from './TestList';
import { IPayment, PaymentSelection } from './PaymentSelection';
import { SampleTypeSelection } from './SampleTypeSelection';
import { Quantity } from './Quantity';
import { Summary } from './Summary';
import { AdditionalInfo } from './AdditionalInfo';
import { Project } from "./Project";
import { AdditionalEmails } from "./AdditionalEmails";
import { LabFields } from './LabFields';

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
    additionalEmails: Array<string>;
    grind: boolean;
    foreignSoil: boolean;
    filterWater: boolean;
    isErrorActive: boolean;
    errorMessage: string;
    isFromLab: boolean;
    status: string;
    labComments: string;
    adjustmentAmount: number;
}

export default class OrderForm extends React.Component<undefined, IOrderState> {
    constructor(props) {
        super(props);

        const initialState = {
            orderId: null,
            additionalInfo: '',
            payment: { clientType: 'uc', account: '' },
            quantity: null,
            sampleType: 'Soil',
            testItems: window.App.orderData.testItems,
            selectedTests: { },
            isValid: false,
            isSubmitting: false,
            project: '',
            additionalEmails: [],
            grind: false,
            foreignSoil: false,
            filterWater: false,
            isErrorActive: false,
            errorMessage: '',
            isFromLab: false,
            status: '',
            labComments: '',
            adjustmentAmount: 0,
        };

        if (window.App.defaultAccount) {            
            initialState.payment.account = window.App.defaultAccount;
            initialState.payment.clientType = 'uc';
        } else {
            initialState.payment.clientType = 'other';
        }
        if (window.App.IsFromLab === true) {
            initialState.isFromLab = true;
            initialState.status = window.App.Status;
        }

        if (window.App.orderData.order) {
            // load up existing order
            const orderInfo = JSON.parse(window.App.orderData.order.jsonDetails);

            initialState.quantity = orderInfo.Quantity;
            initialState.additionalInfo = orderInfo.AdditionalInfo;
            initialState.additionalEmails = orderInfo.AdditionalEmails;
            initialState.sampleType = orderInfo.SampleType;
            initialState.orderId = window.App.OrderId;
            initialState.project = orderInfo.Project;
            initialState.isValid = true;
            initialState.grind = orderInfo.Grind;
            initialState.foreignSoil = orderInfo.ForeignSoil;
            initialState.filterWater = orderInfo.FilterWater;
            initialState.payment.clientType = orderInfo.Payment.ClientType;
            initialState.payment.account = orderInfo.Payment.Account;
            initialState.labComments = orderInfo.LabComments;
            initialState.adjustmentAmount = orderInfo.AdjustmentAmount;

            orderInfo.SelectedTests.forEach(test => { initialState.selectedTests[test.Id] = true; });
        }

        this.state = { ...initialState };
    }
    validate = () => {
        const valid = this.state.quantity > 0 && this.state.quantity <= 100 && !!this.state.project.trim();
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

    onEmailAdded = (additionalEmail: string) => {
        this.setState({
                ...this.state,
                additionalEmails: [
                    ...this.state.additionalEmails,
                    additionalEmail
                ]
            }
        );
    }

    onDeleteEmail = (email2Delete: any) => {
        const index = this.state.additionalEmails.indexOf(email2Delete);
        if (index > -1) {
            const shallowCopy = [...this.state.additionalEmails];
            shallowCopy.splice(index, 1);
            this.setState({ ...this.state, additionalEmails: shallowCopy });
        }
    }


    handleChange = (name, value) => {
        this.setState({ ...this.state, [name]: value }, this.validate);
    };

    handleDialogToggle = () => {
        this.setState({ ...this.state, isErrorActive: !this.state.isErrorActive});
    }

    dialogActions = [
        { label: "Got It!", onClick: this.handleDialogToggle }
    ];

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
        let postUrl = '/Order/Save';
        let returnUrl = '/Order/Confirmation/';
        if (this.state.isFromLab) {
            postUrl = '/Lab/Save';
            returnUrl = '/Lab/Confirmation/';
        }
        this.setState({ ...this.state, isSubmitting: true });
        const selectedTests = this.getTests().selected;
        const order = {
            orderId: this.state.orderId,
            quantity: this.state.quantity,
            additionalInfo: this.state.additionalInfo,
            additionalEmails: this.state.additionalEmails,
            project: this.state.project,
            payment: this.state.payment,
            sampleType: this.state.sampleType,
            grind: this.state.grind,
            foreignSoil: this.state.foreignSoil,
            filterWater: this.state.filterWater,
            labComments: this.state.labComments,
            adjustmentAmount: this.state.adjustmentAmount,
            selectedTests,
        }
        const that = this;
        $.post({
            
            url: postUrl,
            data: JSON.stringify(order),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        }).success((response) => {
            if (response.success === true) {
                const redirectId = response.id;
                window.location.replace(returnUrl + redirectId);
            } else {
                that.setState({ ...that.state, isSubmitting: false, isErrorActive: true, errorMessage: response.message });
            }
        }).error(() => {
            that.setState({ ...that.state, isSubmitting: false, isErrorActive: true, errorMessage: "An internal error occured..." });
        });
    }

    renderLab = () => {
        if (this.state.isFromLab) {
            return (<LabFields labComments={this.state.labComments} adjustmentAmount={this.state.adjustmentAmount} handleChange={this.handleChange} />);
        }
    }


    render() {
        const { payment, selectedTests, sampleType, quantity, additionalInfo, project, additionalEmails, grind, foreignSoil, filterWater, isFromLab, status, adjustmentAmount } = this.state;

        const { filtered, selected} = this.getTests();

        return (
            <div className="row">
                <div className="col-8 whiteblock">


                    <PaymentSelection payment={payment} onPaymentSelected={this.onPaymentSelected} />

                    <SampleTypeSelection sampleType={sampleType} onSampleSelected={this.onSampleSelected} />

                    <div className="form_wrap">
                        <label className="form_header">How many samples will you require?</label>
                        <Quantity quantity={quantity} onQuantityChanged={this.onQuantityChanged} />
                    </div>
                    <AdditionalEmails addedEmails={additionalEmails} onEmailAdded={this.onEmailAdded} onDeleteEmail={this.onDeleteEmail}/>
                    <Project project={project} handleChange={this.handleChange} />
                    <AdditionalInfo additionalInfo={additionalInfo} handleChange={this.handleChange} />
                    {this.renderLab()}
                    <TestList items={filtered} payment={payment} selectedTests={selectedTests} onTestSelectionChanged={this.onTestSelectionChanged} />
                    <div style={{ height: 600 }}></div>
                </div>
                <div className="col-lg-4">
                    <div data-spy="affix" data-offset-top="60" data-offset-bottom="200">
                        <Summary
                            isCreate={this.state.orderId === null}
                            canSubmit={this.state.isValid && !this.state.isSubmitting}
                            hideError={this.state.isValid || this.state.isSubmitting}
                            testItems={selected}
                            quantity={quantity}
                            payment={payment}
                            onSubmit={this.onSubmit}
                            grind={(grind && sampleType !== "Water")}
                            foreignSoil={(foreignSoil && sampleType === "Soil")}
                            filterWater={(filterWater && sampleType === "Water")}
                            isFromLab={isFromLab}
                            status={status}
                            adjustmentAmount={adjustmentAmount} />
                    </div>
                </div>

                <Dialog
                    actions={this.dialogActions}
                    active={this.state.isErrorActive}
                    onEscKeyDown={this.handleDialogToggle}
                    onOverlayClick={this.handleDialogToggle}
                    title='Errors Detected'>
                    <p>{this.state.errorMessage}</p>
                </Dialog>
            </div>
        );
    }
}
