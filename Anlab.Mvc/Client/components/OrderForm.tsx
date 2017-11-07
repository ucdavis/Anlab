import * as React from "react";
import * as ReactDOM from "react-dom";
import { Collapse, Fade } from "react-bootstrap";
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
import Summary from "./Summary";
import { ITestItem, TestList } from "./TestList";

declare var $: any;

export interface IOrderFormProps {
    testItems: ITestItem[];
    defaultAccount: string;
    defaultEmail: string;
    defaultClientId: string;
    orderInfo: any;
    internalProcessingFee: number;
    externalProcessingFee: number;
    orderId?: number;
}

interface IOrderFormState {
    additionalInfo: string;
    project: string;
    filteredTests: ITestItem[];
    commodity: string;
    payment: IPayment;
    quantity?: number;
    sampleType: string;
    sampleTypeQuestions: ISampleTypeQuestions;
    selectedCodes: object;
    selectedTests: ITestItem[];
    isValid: boolean;
    isSubmitting: boolean;
    additionalEmails: string[];
    isErrorActive: boolean;
    errorMessage: string;
    status: string;
    clientId: string;
    clientName: string;
    newClientInfo: INewClientInfo;
    additionalInfoList: object;
}

export default class OrderForm extends React.Component<IOrderFormProps, IOrderFormState> {

    private quantityRef: any;
    private projectRef: any;
    private waterPreservativeRef: any;
    private clientIdRef: any;
    private ucAccountRef: any;

    constructor(props) {
        super(props);

        const initialState: IOrderFormState = {
            additionalEmails: [],
            additionalInfo: "",
            additionalInfoList: {},
            clientId: (this.props.defaultClientId ? this.props.defaultClientId : ""),
            commodity: "",
            errorMessage: "",
            filteredTests: [],
            isErrorActive: false,
            isSubmitting: false,
            isValid: false,
            newClientInfo: {
                email: this.props.defaultEmail,
                employer: "",
                name: "",
                phoneNumber: "",
            },
            clientName: null,
            payment: { clientType: "uc", account: "" },
            project: "",
            quantity: null,
            sampleType: "",
            sampleTypeQuestions: {
                plantReportingBasis: "Report results on 100% dry weight basis, based on an average of 10% of the samples.",
                soilImported: false,
                waterFiltered: false,
                waterPreservativeAdded: false,
                waterPreservativeInfo: "",
                waterReportedInMgL: false,
            },
            selectedCodes: {},
            selectedTests: [],
            status: "",
        };

        if (this.props.defaultAccount) {
            initialState.payment.account = this.props.defaultAccount;
            initialState.payment.clientType = "uc";
        } else {
            initialState.payment.clientType = "";
        }

        const { orderInfo } = this.props;
        if (orderInfo) {

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
            initialState.additionalInfoList = orderInfo.AdditionalInfoList;
            initialState.filteredTests = this.props.testItems.filter((item) => item.categories.indexOf(orderInfo.SampleType) !== -1);

            orderInfo.SelectedTests.forEach((test) => { initialState.selectedCodes[test.Id] = true; });
            initialState.selectedTests = initialState.filteredTests.filter((t) => !!initialState.selectedCodes[t.id]);
        }

        this.state = { ...initialState };
    }

    public render() {
        const { defaultEmail, internalProcessingFee, externalProcessingFee } = this.props;
        const {
            payment, selectedTests, sampleType, sampleTypeQuestions, quantity, additionalInfo, project,
            commodity, additionalEmails, status, clientId, newClientInfo, clientName,
            additionalInfoList, filteredTests, selectedCodes,
        } = this.state;

        const isUcClient = this.state.payment.clientType === "uc";
        const processingFee = isUcClient ? internalProcessingFee : externalProcessingFee;

        return (
            <div>
                <div>
                    <div className="form_wrap">
                        <label className="form_header">Do you have a Client ID?</label>
                        <ClientId
                            clientId={clientId}
                            clientName={clientName}
                            handleChange={this._handleChange}
                            clientIdRef={(inputRef) => { this.clientIdRef = inputRef; }}
                            newClientInfo={newClientInfo}
                            updateNewClientInfo={this._updateNewClientInfo} />
                    </div>
                    <Collapse in={(this.state.clientName != null) || (!!this.state.newClientInfo.name.trim())
                        ||  (!!this.state.payment.clientType.trim())}>
                    <div className="form_wrap">
                        <label className="form_header">How will you pay for your order?</label>
                        <PaymentSelection
                            payment={payment}
                            onPaymentSelected={this._onPaymentSelected}
                            ucAccountRef={(inputRef) => { this.ucAccountRef = inputRef; }} />
                    </div>
                    </Collapse>

                    <Collapse in={!!this.state.payment.clientType.trim() || !!this.state.project.trim()}>
                    <div className="form_wrap">
                        <label className="form_header">What is the project title for this order?</label>
                        <Project
                            project={project}
                            handleChange={this._handleChange}
                            projectRef={(inputRef) => { this.projectRef = inputRef; }}
                        />
                        <Commodity commodity={commodity} handleChange={this._handleChange} />
                    </div>
                    </Collapse>

                    <Collapse in={!!this.state.project.trim() || this.state.quantity > 0 || !!this.state.sampleType.trim()}>
                        <div>
                    <div className="form_wrap">
                        <label className="form_header">Who should be notified for this test?</label>
                        <AdditionalEmails
                            addedEmails={additionalEmails}
                            defaultEmail={defaultEmail}
                            onEmailAdded={this._onEmailAdded}
                            onDeleteEmail={this._onDeleteEmail}
                        />
                    </div>
                    
                    <div className="form_wrap">
                        <label className="form_header">How many samples will you require?</label>
                        <Quantity
                            quantity={quantity}
                            onQuantityChanged={this._onQuantityChanged}
                            quantityRef={(numberRef) => { this.quantityRef = numberRef; }}
                        />
                    </div>
                            </div>
                    </Collapse>

                    <Collapse in={this.state.quantity > 0 || this.state.sampleType !== ""}>
                    <div className="form_wrap">
                        <label className="form_header">What type of samples?</label>
                        <SampleTypeSelection sampleType={sampleType} onSampleSelected={this._onSampleSelected} />
                        <SampleTypeQuestions
                            waterPreservativeRef={(inputRef) => { this.waterPreservativeRef = inputRef; }}
                            sampleType={sampleType}
                            questions={sampleTypeQuestions}
                            handleChange={this._onSampleQuestionChanged}
                        />
                    </div>
                        </Collapse>

                    <Collapse in={this.state.sampleType !== ""}>
                        <div>
                            <div className="form_wrap">
                                <label className="form_header">Do you have any other information to provide?</label>
                                <AdditionalInfo value={additionalInfo} name="additionalInfo" handleChange={this._handleChange} />
                            </div>

                    <div className="form_wrap">
                        <label className="form_header margin-bottom-zero">Which tests would you like to run?</label>
                        <TestList
                            items={filteredTests}
                            selectedCodes={selectedCodes}
                            clientType={payment.clientType}
                            onTestSelectionChanged={this._onTestSelectionChanged}
                            additionalInfoList={additionalInfoList}
                            updateAdditionalInfo={this._updateAdditionalInfo}
                        />
                    </div>

                    <div className="stickyfoot shadowed" data-spy="affix" data-offset-bottom="0">
                        <Summary
                            isCreate={this.props.orderId === null}
                            canSubmit={this.state.isValid && !this.state.isSubmitting}
                            hideError={this.state.isValid || this.state.isSubmitting}
                            selectedTests={selectedTests}
                            quantity={quantity}
                            clientType={payment.clientType}
                            onSubmit={this._onSubmit}
                            status={status}
                            processingFee={processingFee}
                            handleErrors={this._handleErrors}
                        />
                    </div>
                            </div>
                    </Collapse>
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

        //check either client name or new client info
        if (!this.state.clientName
            && (!this.state.newClientInfo.name || !this.state.newClientInfo.name.trim()))
        {
            valid = false;
        }

        // check quantity
        if (this.state.quantity <= 0 || this.state.quantity > 100) {
            valid = false;
        }

        // check project name
        if (!this.state.project || !this.state.project.trim()) {
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
            && (!this.state.payment.account || !this.state.payment.account.trim())) {
            valid = false;
        }

        // push valid
        this.setState({ isValid: valid });
    }

    private _onPaymentSelected = (payment: any) => {
        this.setState({ payment }, this._validate);
    }

    private _onSampleSelected = (sampleType: string) => {
        const filteredTests = this.props.testItems.filter((item) => item.categories.indexOf(sampleType) !== -1);
        const selectedTests = filteredTests.filter((t) => !!this.state.selectedCodes[t.id]);

        this.setState({ filteredTests, selectedTests, sampleType }, this._validate);
    }

    private _onTestSelectionChanged = (test: ITestItem, selected: boolean) => {
        const selectedCodes = {
            ...this.state.selectedCodes,
            [test.id]: selected,
        };
        const selectedTests = this.state.filteredTests.filter((t) => !!selectedCodes[t.id]);

        this.setState({ selectedCodes, selectedTests }, this._validate);
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
        }, this._validate);
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

    private _handleErrors = () => {
        if (this.state.isValid || this.state.isSubmitting) {
            return;
        }
        if ((!this.state.clientName)
            && (!this.state.newClientInfo.name || !this.state.newClientInfo.name.trim())){
            this._focusInput(this.clientIdRef);
        } else if (this.state.payment.clientType === "uc"
            && (!this.state.payment.account || !this.state.payment.account.trim())) {
            this._focusInput(this.ucAccountRef);
        }
        else if (!this.state.project || !this.state.project.trim()) {
            this._focusInput(this.projectRef);
        } else if (this.state.quantity <= 0 || this.state.quantity > 100) {
            this._focusInput(this.quantityRef);
        } else if (this.state.sampleType === "Water"
            && this.state.sampleTypeQuestions.waterPreservativeAdded
            && (!this.state.sampleTypeQuestions.waterPreservativeInfo
                || !this.state.sampleTypeQuestions.waterPreservativeInfo.trim())) {
            this._focusInput(this.waterPreservativeRef);
        }
    }

    private _focusInput = (component: any) => {
        component.focus();
        component.blur();
        component.focus();
    }

    private _updateAdditionalInfo = (id: string, value: string) => {
        this.setState({
            additionalInfoList: {
                ...this.state.additionalInfoList,
                [id]: value,
            },
        }, this._validate);
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

    private _onSubmit = () => {
        // lock for duplicate submits
        if (this.state.isSubmitting) {
            return;
        }
        this.setState({ isSubmitting: true });

        // find selected tests and associated additional info, map to dictionary array
        const selectedCodes = Object.keys(this.state.selectedCodes).filter((k) => !!k);

        // return in dictionary format
        const additionalInfoList = Object.keys(this.state.additionalInfoList)
            .filter((k) => selectedCodes.indexOf(k) > -1)
            .map((k) => ({ key: k, value: this.state.additionalInfoList[k] }));

        // build order
        const order = {
            additionalEmails: this.state.additionalEmails,
            additionalInfo: this.state.additionalInfo,
            additionalInfoList,
            clientId: this.state.clientId,
            commodity: this.state.commodity,
            externalProcessingFee: this.props.externalProcessingFee,
            internalProcessingFee: this.props.internalProcessingFee,
            newClientInfo: this.state.newClientInfo,
            orderId: this.props.orderId,
            payment: this.state.payment,
            project: this.state.project,
            quantity: this.state.quantity,
            sampleType: this.state.sampleType,
            sampleTypeQuestions: this.state.sampleTypeQuestions,
            selectedTests: this.state.selectedTests,
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
