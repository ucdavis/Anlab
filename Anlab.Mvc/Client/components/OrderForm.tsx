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
import { ClientId } from "./ClientId";
import { Button } from "react-toolbox/lib/button";

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
    isErrorActive: boolean;
    errorMessage: string;
    status: string;
    clientId: string;
    internalProcessingFee: number;
    externalProcessingFee: number;
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
            isErrorActive: false,
            errorMessage: '',
            status: '',
            clientId: '',
            internalProcessingFee: window.App.orderData.internalProcessingFee,
            externalProcessingFee: window.App.orderData.externalProcessingFee
        };

        if (window.App.defaultAccount) {
            initialState.payment.account = window.App.defaultAccount;
            initialState.payment.clientType = 'uc';
        } else {
            initialState.payment.clientType = 'other';
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
            initialState.payment.clientType = orderInfo.Payment.ClientType;
            initialState.payment.account = orderInfo.Payment.Account;
            initialState.clientId = orderInfo.ClientId;
            initialState.internalProcessingFee = window.App.orderData.internalProcessingFee;
            initialState.externalProcessingFee = window.App.orderData.externalProcessingFee;

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
        const filtered = testItems.filter(item => item.categories.indexOf(sampleType) !== -1);
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
            selectedTests,
            clientId: this.state.clientId,
            internalProcessingFee: this.state.internalProcessingFee,
            externalProcessingFee: this.state.externalProcessingFee
        }
        const that = this;
        var antiforgery = $("input[name='__RequestVerificationToken']").val();
        $.post({
            url: postUrl,
            data: { model: order, __RequestVerificationToken: antiforgery }
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

    render() {
        const { payment, selectedTests, sampleType, quantity, additionalInfo, project, additionalEmails, status, clientId, internalProcessingFee, externalProcessingFee } = this.state;

        const { filtered, selected } = this.getTests();

        const processingFee = this.state.payment.clientType === 'uc' ? this.state.internalProcessingFee : this.state.externalProcessingFee;

        return (
            <div className="row">
                <div className="col-10 whiteblock">


                    <PaymentSelection payment={payment} onPaymentSelected={this.onPaymentSelected} />

                    <SampleTypeSelection sampleType={sampleType} onSampleSelected={this.onSampleSelected} />

                    <div className="form_wrap">
                        <label className="form_header">How many samples will you require?</label>
                        <Quantity quantity={quantity} onQuantityChanged={this.onQuantityChanged} />
                    </div>
                    <AdditionalEmails addedEmails={additionalEmails} onEmailAdded={this.onEmailAdded} onDeleteEmail={this.onDeleteEmail}/>
                    <Project project={project} handleChange={this.handleChange} />
                    <ClientId clientId={clientId} handleChange={this.handleChange} />
                    <AdditionalInfo additionalInfo={additionalInfo} handleChange={this.handleChange} />
                    <TestList items={filtered} payment={payment} selectedTests={selectedTests} onTestSelectionChanged={this.onTestSelectionChanged} />

                </div>
                <div className="stickyfoot shadowed" data-spy="affix" data-offset-top="200" data-offset-bottom="0">
                  <div className="ordersum">
                    <div><h3>Order Total: $99</h3>
                    <a role="button" data-toggle="collapse" data-target="#collapseExample" aria-expanded="false" aria-controls="collapseExample">
                      Order Details
                    </a></div>

                    <Button className="btn btn-order" disabled={this.state.isValid && !this.state.isSubmitting} onClick={this.onSubmit} > Place Order</Button>
                  </div>

                <div className="collapse" id="collapseExample">
                  <div className="well">
                    <Summary
                        isCreate={this.state.orderId === null}
                        canSubmit={this.state.isValid && !this.state.isSubmitting}
                        hideError={this.state.isValid || this.state.isSubmitting}
                        testItems={selected}
                        quantity={quantity}
                        payment={payment}
                        onSubmit={this.onSubmit}
                        status={status}
                        processingFee={processingFee} />
                  </div>
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
