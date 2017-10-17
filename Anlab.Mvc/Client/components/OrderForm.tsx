import * as React from 'react';
import Dialog from 'react-toolbox/lib/dialog';
import { ITestItem, TestList } from './TestList';
import { IPayment, PaymentSelection } from './PaymentSelection';
import { SampleTypeSelection } from './SampleTypeSelection';
import { SampleTypeQuestions, ISampleTypeQuestions } from './SampleTypeQuestions';
import { Quantity } from './Quantity';
import { Summary } from './Summary';
import { AdditionalInfo } from './AdditionalInfo';
import { Project } from "./Project";
import { AdditionalEmails } from "./AdditionalEmails";
import { ClientId } from "./ClientId";
import { ClientIdModal, INewClientInfo } from "./ClientIdModal";
import { Commodity } from "./Commodity";
import { Button } from "react-toolbox/lib/button";
import * as ReactDOM from "react-dom";
import { Input } from "react-toolbox/lib/input";

declare var window: any;
declare var $: any;

interface IOrderState {
    orderId?: number;
    additionalInfo: string;
    project: string;
    commodity: string;
    payment: IPayment;
    quantity?: number;
    sampleType: string;
    sampleTypeQuestions: ISampleTypeQuestions;
    testItems: Array<ITestItem>;
    selectedTests: any;
    isValid: boolean;
    isSubmitting: boolean;
    additionalEmails: Array<string>;
    isErrorActive: boolean;
    errorMessage: string;
    status: string;
    clientId: string;
    newClientInfo: INewClientInfo;
    internalProcessingFee: number;
    externalProcessingFee: number;
    defaultEmail: string;
    additionalInfoList: Object;
}

export default class OrderForm extends React.Component<undefined, IOrderState> {

    private quantityRef: any;
    private projectRef: any;

    constructor(props) {
        super(props);

        const initialState = {
            orderId: null,
            additionalInfo: '',
            payment: { clientType: 'uc', account: '' },
            quantity: null,
            sampleType: 'Soil',
            sampleTypeQuestions: {
                soilImported: false,
                plantReportingBasis: "Report results on 100% dry weight basis, based on an average of 10% of the samples.",
                waterFiltered: false,
                waterPreservativeAdded: false,
                waterPreservativeInfo: "",
                waterReportedInMgL: false,
            },
            testItems: window.App.orderData.testItems,
            selectedTests: {},
            isValid: false,
            isSubmitting: false,
            project: '',
            commodity: '',
            additionalEmails: [],
            isErrorActive: false,
            errorMessage: '',
            status: '',
            clientId: window.App.defaultClientId,
            newClientInfo: {
                name: '',
                employer: '',
                email: window.App.defaultEmail,
                phoneNumber: ''
            },
            internalProcessingFee: window.App.orderData.internalProcessingFee,
            externalProcessingFee: window.App.orderData.externalProcessingFee,
            defaultEmail: window.App.defaultEmail,
            additionalInfoList: {}
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
            initialState.commodity = orderInfo.Commodity;
            initialState.isValid = true;
            initialState.payment.clientType = orderInfo.Payment.ClientType;
            initialState.payment.account = orderInfo.Payment.Account;
            initialState.clientId = orderInfo.ClientId;
            initialState.newClientInfo = {
                name: orderInfo.NewClientInfo.Name,
                employer: orderInfo.NewClientInfo.Employer,
                email: orderInfo.NewClientInfo.Email,
                phoneNumber: orderInfo.NewClientInfo.PhoneNumber
            };
            initialState.internalProcessingFee = window.App.orderData.internalProcessingFee;
            initialState.externalProcessingFee = window.App.orderData.externalProcessingFee;
            initialState.defaultEmail = window.App.defaultEmail;
            initialState.additionalInfoList = orderInfo.AdditionalInfoList;

            orderInfo.SelectedTests.forEach(test => { initialState.selectedTests[test.Id] = true; });
        }

        this.state = { ...initialState };
    }
    validate = () => {
        let valid = this.state.quantity > 0 && this.state.quantity <= 100 && !!this.state.project.trim();
        if (valid) {
            if (this.state.payment.clientType === 'uc' && (this.state.payment.account === '' || this.state.payment.account == undefined)) {
                valid = false;                
            }
        }
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

    onSampleQuestionChanged = (question: string, answer: any) => {
        this.setState({
            ...this.state,
            sampleTypeQuestions: { ...this.state.sampleTypeQuestions, [question]: answer }
        });
    }

    onQuantityChanged = (quantity?: number) => {
        this.setState({ ...this.state, quantity }, this.validate);
    }

    updateNewClientInfo = (info: INewClientInfo) => {
        this.setState({...this.state,
            newClientInfo: { ...info }
        });
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

    focusInput = (component: any) => {
        var node = ReactDOM.findDOMNode(component).querySelector('input');
        node.focus();
        node.blur();
        node.focus();
    }

    updateAdditionalInfo = (id: string, value: string) => {
        const tests = this.state.additionalInfoList;
        tests[id] = value;
        this.forceUpdate();

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
        const selectedCodes = selectedTests.map(t => t.id);
        let additionalInfoList = Object.keys(this.state.additionalInfoList).map(key => {
            if (selectedCodes.indexOf(key) > -1)
                return { "key": key, "value": this.state.additionalInfoList[key] }
            else
                return null;
        });
        additionalInfoList = additionalInfoList.filter(x => !!x);

        const order = {
            orderId: this.state.orderId,
            quantity: this.state.quantity,
            additionalInfo: this.state.additionalInfo,
            additionalInfoList, // return in dictionary format
            additionalEmails: this.state.additionalEmails,
            project: this.state.project,
            commodity: this.state.commodity,
            payment: this.state.payment,
            sampleType: this.state.sampleType,
            sampleTypeQuestions: this.state.sampleTypeQuestions,
            selectedTests,
            clientId: this.state.clientId,
            newClientInfo: this.state.newClientInfo,
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
        const { payment, selectedTests, sampleType, sampleTypeQuestions, quantity, additionalInfo, project, commodity, additionalEmails, status, clientId, newClientInfo, internalProcessingFee, externalProcessingFee, defaultEmail, additionalInfoList } = this.state;

        const { filtered, selected } = this.getTests();

        const processingFee = this.state.payment.clientType === 'uc' ? this.state.internalProcessingFee : this.state.externalProcessingFee;

        return (
            <div>
                <div>
                    <PaymentSelection payment={payment} onPaymentSelected={this.onPaymentSelected} />

                    <SampleTypeSelection sampleType={sampleType} onSampleSelected={this.onSampleSelected} />
                    <SampleTypeQuestions sampleType={sampleType} questions={sampleTypeQuestions} handleChange={this.onSampleQuestionChanged} />

                    <div className="form_wrap">
                        <label className="form_header">How many samples will you require?</label>
                        <Quantity quantity={quantity} onQuantityChanged={this.onQuantityChanged} quantityRef={(numberRef) => { this.quantityRef = numberRef }} />
                    </div>
                    <AdditionalEmails addedEmails={additionalEmails} onEmailAdded={this.onEmailAdded} onDeleteEmail={this.onDeleteEmail} defaultEmail={this.state.defaultEmail} />
                    <div className="form_wrap">
                        <label className="form_header">What is the project title for this order?</label>
                        <Project project={project} handleChange={this.handleChange} projectRef={(inputRef) => { this.projectRef = inputRef }} />
                        <Commodity commodity={commodity} handleChange={this.handleChange} />
                    </div>
                    <ClientIdModal clientInfo={newClientInfo} updateClient={this.updateNewClientInfo} />
                    <ClientId clientId={clientId} handleChange={this.handleChange} />
                    <AdditionalInfo additionalInfo={additionalInfo} handleChange={this.handleChange} />
                    <TestList items={filtered} payment={payment} selectedTests={selectedTests} onTestSelectionChanged={this.onTestSelectionChanged} additionalInfoList={additionalInfoList} updateAdditionalInfo={this.updateAdditionalInfo} />

                </div>
                <div className="stickyfoot shadowed" data-spy="affix" data-offset-bottom="0">

                <Summary
                    isCreate={this.state.orderId === null}
                    canSubmit={this.state.isValid && !this.state.isSubmitting}
                    hideError={this.state.isValid || this.state.isSubmitting}
                    testItems={selected}
                    quantity={quantity}
                    payment={payment}
                    onSubmit={this.onSubmit}
                    status={status}
                    processingFee={processingFee}
                    project={this.state.project}
                    focusInput={this.focusInput}
                    quantityRef={this.quantityRef}
                    projectRef={this.projectRef} />
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
