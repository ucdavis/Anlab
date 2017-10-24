﻿import * as React from "react";
import * as ReactDOM from "react-dom";
import { Button } from "react-toolbox/lib/button";
import Dialog from "react-toolbox/lib/dialog";
import { Input } from "react-toolbox/lib/input";
import { AdditionalEmails } from "./AdditionalEmails";
import { AdditionalInfo } from "./AdditionalInfo";
import { ClientId } from "./ClientId";
import { ClientIdModal, INewClientInfo } from "./ClientIdModal";
import { Commodity } from "./Commodity";
import { IPayment, PaymentSelection } from "./PaymentSelection";
import { Project } from "./Project";
import { Quantity } from "./Quantity";
import { ISampleTypeQuestions, SampleTypeQuestions } from "./SampleTypeQuestions";
import { SampleTypeSelection } from "./SampleTypeSelection";
import { Summary } from "./Summary";
import { ITestItem, TestList } from "./TestList";

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
    testItems: ITestItem[];
    selectedTests: any;
    isValid: boolean;
    isSubmitting: boolean;
    additionalEmails: string[];
    isErrorActive: boolean;
    errorMessage: string;
    status: string;
    clientId: string;
    newClientInfo: INewClientInfo;
    internalProcessingFee: number;
    externalProcessingFee: number;
    defaultEmail: string;
    additionalInfoList: object;
}

export default class OrderForm extends React.Component<{}, IOrderState> {

    private quantityRef: any;
    private projectRef: any;
    private waterPreservativeRef: any;

    constructor(props) {
        super(props);

        const initialState = {
            additionalEmails: [],
            additionalInfo: "",
            additionalInfoList: {},
            clientId: window.App.defaultClientId,
            commodity: "",
            defaultEmail: window.App.defaultEmail,
            errorMessage: "",
            externalProcessingFee: window.App.orderData.externalProcessingFee,
            internalProcessingFee: window.App.orderData.internalProcessingFee,
            isErrorActive: false,
            isSubmitting: false,
            isValid: false,
            newClientInfo: {
                email: window.App.defaultEmail,
                employer: "",
                name: "",
                phoneNumber: "",
            },
            orderId: null,
            payment: { clientType: "uc", account: "" },
            project: "",
            quantity: null,
            sampleType: "Soil",
            sampleTypeQuestions: {
                plantReportingBasis: "Report results on 100% dry weight basis, based on an average of 10% of the samples.",
                soilImported: false,
                waterFiltered: false,
                waterPreservativeAdded: false,
                waterPreservativeInfo: "",
                waterReportedInMgL: false,
            },
            selectedTests: {},
            status: "",
            testItems: window.App.orderData.testItems,
        };

        if (window.App.defaultAccount) {
            initialState.payment.account = window.App.defaultAccount;
            initialState.payment.clientType = "uc";
        } else {
            initialState.payment.clientType = "other";
        }

        if (window.App.orderData.order) {
            // load up existing order
            const orderInfo = JSON.parse(window.App.orderData.order.jsonDetails);

            initialState.quantity = orderInfo.Quantity;
            initialState.additionalInfo = orderInfo.AdditionalInfo;
            initialState.additionalEmails = orderInfo.AdditionalEmails;
            initialState.sampleType = orderInfo.SampleType;
            initialState.sampleTypeQuestions = {
                plantReportingBasis: orderInfo.SampleTypeQuestions.PlantReportingBasis,
                soilImported: orderInfo.SampleTypeQuestions.SoilImported,
                waterFiltered: orderInfo.SampleTypeQuestions.WaterFiltered,
                waterPreservativeAdded: orderInfo.SampleTypeQuestions.WaterPreservativeAdded,
                waterPreservativeInfo: orderInfo.SampleTypeQuestions.WaterPreservativeInfo,
                waterReportedInMgL: orderInfo.SampleTypeQuestions.WaterReportedInMgL,
            },
            initialState.orderId = window.App.OrderId;
            initialState.project = orderInfo.Project;
            initialState.commodity = orderInfo.Commodity;
            initialState.isValid = true;
            initialState.payment.clientType = orderInfo.Payment.ClientType;
            initialState.payment.account = orderInfo.Payment.Account;
            initialState.clientId = orderInfo.ClientId;
            initialState.newClientInfo = {
                email: orderInfo.NewClientInfo.Email,
                employer: orderInfo.NewClientInfo.Employer,
                name: orderInfo.NewClientInfo.Name,
                phoneNumber: orderInfo.NewClientInfo.PhoneNumber,
            };
            initialState.internalProcessingFee = window.App.orderData.internalProcessingFee;
            initialState.externalProcessingFee = window.App.orderData.externalProcessingFee;
            initialState.defaultEmail = window.App.defaultEmail;
            initialState.additionalInfoList = orderInfo.AdditionalInfoList;

            orderInfo.SelectedTests.forEach((test) => { initialState.selectedTests[test.Id] = true; });
        }

        this.state = { ...initialState };
    }

    public render() {
        const {
            payment, selectedTests, sampleType, sampleTypeQuestions, quantity, additionalInfo, project,
            commodity, additionalEmails, status, clientId, newClientInfo, internalProcessingFee, externalProcessingFee,
            defaultEmail, additionalInfoList,
        } = this.state;

        const { filtered, selected } = this._getTests();

        const isUcClient = this.state.payment.clientType === "uc";
        const processingFee = isUcClient ? this.state.internalProcessingFee : this.state.externalProcessingFee;

        return (
            <div>
                <div>
                    <PaymentSelection payment={payment} onPaymentSelected={this._onPaymentSelected} />
                    <SampleTypeSelection sampleType={sampleType} onSampleSelected={this._onSampleSelected} />
                    <SampleTypeQuestions
                        waterPreservativeRef={(inputRef) => { this.waterPreservativeRef = inputRef; }}
                        sampleType={sampleType}
                        questions={sampleTypeQuestions}
                        handleChange={this._onSampleQuestionChanged}
                    />
                    <div className="form_wrap">
                        <label className="form_header">How many samples will you require?</label>
                        <Quantity
                            quantity={quantity}
                            onQuantityChanged={this._onQuantityChanged}
                            quantityRef={(numberRef) => { this.quantityRef = numberRef; }}
                        />
                    </div>
                    <AdditionalEmails
                        addedEmails={additionalEmails}
                        onEmailAdded={this._onEmailAdded}
                        onDeleteEmail={this._onDeleteEmail}
                        defaultEmail={this.state.defaultEmail}
                    />
                    <div className="form_wrap">
                        <label className="form_header">What is the project title for this order?</label>
                        <Project
                            project={project}
                            handleChange={this._handleChange}
                            projectRef={(inputRef) => { this.projectRef = inputRef; }}
                        />
                        <Commodity commodity={commodity} handleChange={this._handleChange} />
                    </div>
                    <ClientIdModal clientInfo={newClientInfo} updateClient={this._updateNewClientInfo} />
                    <ClientId clientId={clientId} handleChange={this._handleChange} />
                    <AdditionalInfo value={additionalInfo} name="additionalInfo" handleChange={this._handleChange} />
                    <TestList
                        items={filtered}
                        payment={payment}
                        selectedTests={selectedTests}
                        onTestSelectionChanged={this._onTestSelectionChanged}
                        additionalInfoList={additionalInfoList}
                        updateAdditionalInfo={this._updateAdditionalInfo}
                    />
                </div>
                <div className="stickyfoot shadowed" data-spy="affix" data-offset-bottom="0">
                    <Summary
                        isCreate={this.state.orderId === null}
                        canSubmit={this.state.isValid && !this.state.isSubmitting}
                        hideError={this.state.isValid || this.state.isSubmitting}
                        testItems={selected}
                        quantity={quantity}
                        payment={payment}
                        onSubmit={this._onSubmit}
                        status={status}
                        processingFee={processingFee}
                        project={this.state.project}
                        focusInput={this._focusInput}
                        quantityRef={this.quantityRef}
                        projectRef={this.projectRef}
                        sampleType={this.state.sampleType}
                        waterPreservativeAdded={this.state.sampleTypeQuestions.waterPreservativeAdded}
                        waterPreservativeInfo={this.state.sampleTypeQuestions.waterPreservativeInfo}
                        waterPreservativeRef={this.waterPreservativeRef}
                    />
                </div>

                <Dialog
                    actions={this._dialogActions}
                    active={this.state.isErrorActive}
                    onEscKeyDown={this._handleDialogToggle}
                    onOverlayClick={this._handleDialogToggle}
                    title="Errors Detected"
                >
                    <p>{this.state.errorMessage}</p>
                </Dialog>
            </div>
        );
    }

    private _validate = () => {
        // default valid
        let valid = true;

        // check quantity
        if (this.state.quantity <= 0 || this.state.quantity > 100) {
            valid = false;
        }

        // check project name
        if (!this.state.project.trim()) {
            valid = false;
        }

        // check special water requirements
        if (this.state.sampleType === "Water"
            && this.state.sampleTypeQuestions.waterPreservativeAdded
            && (!this.state.sampleTypeQuestions.waterPreservativeInfo
                || !this.state.sampleTypeQuestions.waterPreservativeInfo.trim())) {
                    valid = false;
        }

        // check uc account requirements
        if (this.state.payment.clientType === "uc"
            && (this.state.payment.account || this.state.payment.account.trim())) {
            valid = false;
        }

        // push valid
        this.setState({ isValid: valid });
    }

    private _onPaymentSelected = (payment: any) => {
        this.setState({ payment }, this._validate);
    }

    private _onSampleSelected = (sampleType: string) => {
        this.setState({ sampleType }, this._validate);
    }

    private _onTestSelectionChanged = (test: ITestItem, selected: boolean) => {
        this.setState({
            selectedTests: {
                ...this.state.selectedTests,
                [test.id]: selected,
            },
        }, this._validate);
    }

    private _onSampleQuestionChanged = (question: string, answer: any) => {
        this.setState({
            sampleTypeQuestions: { ...this.state.sampleTypeQuestions, [question]: answer },
        }, this._validate);
    }

    private _onQuantityChanged = (quantity?: number) => {
        this.setState({ quantity }, this._validate);
    }

    private _updateNewClientInfo = (info: INewClientInfo) => {
        this.setState({
            newClientInfo: { ...info },
        });
    }

    private _onEmailAdded = (additionalEmail: string) => {
        this.setState({
            additionalEmails: [
                ...this.state.additionalEmails,
                additionalEmail,
            ],
        });
    }

    private _onDeleteEmail = (email2Delete: any) => {
        const index = this.state.additionalEmails.indexOf(email2Delete);
        if (index > -1) {
            const shallowCopy = [...this.state.additionalEmails];
            shallowCopy.splice(index, 1);
            this.setState({ additionalEmails: shallowCopy });
        }
    }

    private _focusInput = (component: any) => {
        const node = ReactDOM.findDOMNode(component).querySelector("input");
        node.focus();
        node.blur();
        node.focus();
    }

    private _updateAdditionalInfo = (id: string, value: string) => {
        const tests = this.state.additionalInfoList;
        tests[id] = value;
        this.forceUpdate();

    }

    private _handleChange = (name, value) => {
        this.setState({ [name]: value }, this._validate);
    }

    private _handleDialogToggle = () => {
        this.setState({ isErrorActive: !this.state.isErrorActive});
    }

    private _dialogActions = [
        { label: "Got It!", onClick: this._handleDialogToggle },
    ];

    private _getTests = () => {
        const { testItems, payment, selectedTests, sampleType, quantity } = this.state;
        const filtered = testItems.filter((item) => item.categories.indexOf(sampleType) !== -1);
        return {
            filtered,
            selected: filtered.filter((item) => !!selectedTests[item.id]),
        };
    }

    private _onSubmit = () => {
        // lock for duplicate submits
        if (this.state.isSubmitting) {
            return;
        }
        this.setState({ isSubmitting: true });

        // find selected tests and associated additional info, map to dictionary
        const selectedTests = this._getTests().selected;
        const selectedCodes = selectedTests.map((t) => t.id);
        let additionalInfoList = Object.keys(this.state.additionalInfoList).map((key) => {
            if (selectedCodes.indexOf(key) > -1) {
                return { key: key, value: this.state.additionalInfoList[key] };
            } else {
                return null;
            }
        });
        additionalInfoList = additionalInfoList.filter((x) => !!x);

        // build order
        const order = {
            additionalEmails: this.state.additionalEmails,
            additionalInfo: this.state.additionalInfo,
            additionalInfoList, // return in dictionary format
            clientId: this.state.clientId,
            commodity: this.state.commodity,
            externalProcessingFee: this.state.externalProcessingFee,
            internalProcessingFee: this.state.internalProcessingFee,
            newClientInfo: this.state.newClientInfo,
            orderId: this.state.orderId,
            payment: this.state.payment,
            project: this.state.project,
            quantity: this.state.quantity,
            sampleType: this.state.sampleType,
            sampleTypeQuestions: this.state.sampleTypeQuestions,
            selectedTests,
        };

        // submit request to server
        const that = this;
        const postUrl = "/Order/Save";
        const returnUrl = "/Order/Confirmation/";
        const antiforgery = $("input[name='__RequestVerificationToken']").val();
        $.post({
            url: postUrl,
            data: { model: order, __RequestVerificationToken: antiforgery },
        }).success((response) => {
            if (response.success === true) {
                const redirectId = response.id;
                window.location.replace(returnUrl + redirectId);
            } else {
                that.setState({ isSubmitting: false, isErrorActive: true, errorMessage: response.message });
            }
        }).error(() => {
            that.setState({ isSubmitting: false, isErrorActive: true, errorMessage: "An internal error occured..." });
        });
    }


}
