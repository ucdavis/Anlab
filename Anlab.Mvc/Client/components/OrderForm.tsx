import * as React from "react";
import * as ReactDOM from "react-dom";
import { Collapse, Fade } from "react-bootstrap";
import { Modal, Button } from "react-bootstrap";
import { AdditionalEmails } from "./AdditionalEmails";
import { AdditionalInfo } from "./AdditionalInfo";
import { ClientId } from "./ClientId";
import { ClientIdModal, INewClientInfo } from "./ClientIdModal";
import { Commodity } from "./Commodity";
import { IPayment, PaymentSelection } from "./PaymentSelection";
import { IOtherPaymentInfo } from "./OtherPaymentQuestions";
import { Project } from "./Project";
import { Quantity } from "./Quantity";
import {
  ISampleTypeQuestions,
  SampleTypeQuestions
} from "./SampleTypeQuestions";
import { SampleTypeSelection } from "./SampleTypeSelection";
import Summary from "./Summary";
import { ITestItem, TestList } from "./TestList";
import { ViewMode } from "./ViewMode";

declare var $: any;

export interface IOrderFormProps {
  testItems: ITestItem[];
  defaultAccount: string;
  defaultEmail: string;
  defaultClientId: string;
  defaultClientIdName: string;
  defaultAcName: string;
  defaultAcAddr: string;
  defaultAcPhone: string;
  defaultAcEmail: string;
  defaultCompanyName: string;
  orderInfo: any;
  internalProcessingFee: number;
  externalProcessingFee: number;
  orderId?: number;
}

interface IOrderFormState {
  placingOrder: boolean;
  additionalInfo: string;
  project: string;
  filteredTests: ITestItem[];
  commodity: string;
  payment: IPayment;
  otherPaymentInfo: IOtherPaymentInfo;
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

export default class OrderForm extends React.Component<
  IOrderFormProps,
  IOrderFormState
> {
  private quantityRef: any;
  private projectRef: any;
  private waterPreservativeRef: any;
  private clientIdRef: any;
  private ucAccountRef: any;
  private otherPaymentInfoRef: any;

  constructor(props) {
    super(props);

    const initialState: IOrderFormState = {
      additionalEmails: [],
      additionalInfo: "",
      additionalInfoList: {},
      clientId: this.props.defaultClientId ? this.props.defaultClientId : "",
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
        phoneNumber: ""
      },
      clientName: this.props.defaultClientIdName
        ? this.props.defaultClientIdName
        : null,
      payment: { clientType: "uc", account: "" },
      otherPaymentInfo: {
        paymentType: this.props.defaultAccount ? "IOC" : "",
        companyName: this.props.defaultCompanyName || "",
        acName: this.props.defaultAcName || "",
        acAddr: this.props.defaultAcAddr || "",
        acEmail: this.props.defaultAcEmail || "",
        acPhone: this.props.defaultAcPhone || "",
        poNum: "",
        agreementRequired: false
      },
      placingOrder: true,
      project: "",
      quantity: null,
      sampleType: "",
      sampleTypeQuestions: {
        plantReportingBasis:
          "Report results on 100% dry weight basis, based on an average of 10% of the samples.",
        soilImported: false,
        waterFiltered: false,
        waterPreservativeAdded: false,
        waterPreservativeInfo: "",
        waterReportedInMgL: false
      },
      selectedCodes: {},
      selectedTests: [],
      status: ""
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
      (initialState.sampleTypeQuestions = {
        plantReportingBasis: orderInfo.SampleTypeQuestions.PlantReportingBasis,
        soilImported: orderInfo.SampleTypeQuestions.SoilImported,
        waterFiltered: orderInfo.SampleTypeQuestions.WaterFiltered,
        waterPreservativeAdded:
          orderInfo.SampleTypeQuestions.WaterPreservativeAdded,
        waterPreservativeInfo:
          orderInfo.SampleTypeQuestions.WaterPreservativeInfo,
        waterReportedInMgL: orderInfo.SampleTypeQuestions.WaterReportedInMgL
      }),
        (initialState.project = orderInfo.Project);
      initialState.commodity = orderInfo.Commodity;
      initialState.isValid = true;
      initialState.payment.clientType = orderInfo.Payment.ClientType;
      initialState.payment.account = orderInfo.Payment.Account;
      initialState.payment.accountName = orderInfo.Payment.AccountName;
      if (orderInfo.OtherPaymentInfo) {
        initialState.otherPaymentInfo = {
          acAddr: orderInfo.OtherPaymentInfo.AcAddr,
          acEmail: orderInfo.OtherPaymentInfo.AcEmail,
          acName: orderInfo.OtherPaymentInfo.AcName,
          acPhone: orderInfo.OtherPaymentInfo.AcPhone,
          companyName: orderInfo.OtherPaymentInfo.CompanyName,
          paymentType: orderInfo.OtherPaymentInfo.PaymentType,
          poNum: orderInfo.OtherPaymentInfo.PoNum,
          agreementRequired: false
        };
      }
      initialState.clientId = orderInfo.ClientId;
      initialState.newClientInfo = {
        email: orderInfo.NewClientInfo.Email,
        employer: orderInfo.NewClientInfo.Employer,
        name: orderInfo.NewClientInfo.Name,
        phoneNumber: orderInfo.NewClientInfo.PhoneNumber
      };
      initialState.clientName = "";
      initialState.additionalInfoList = orderInfo.AdditionalInfoList;
      initialState.filteredTests = this.props.testItems.filter(
        item => item.categories.indexOf(orderInfo.SampleType) !== -1
      );

      orderInfo.SelectedTests.forEach(test => {
        initialState.selectedCodes[test.Id] = true;
      });
      initialState.selectedTests = initialState.filteredTests.filter(
        t => !!initialState.selectedCodes[t.id]
      );
    }

    this.state = { ...initialState };
  }

  public render() {
    const {
      defaultEmail,
      internalProcessingFee,
      externalProcessingFee
    } = this.props;
    const {
      payment,
      selectedTests,
      sampleType,
      sampleTypeQuestions,
      quantity,
      additionalInfo,
      project,
      commodity,
      additionalEmails,
      status,
      clientId,
      newClientInfo,
      clientName,
      additionalInfoList,
      filteredTests,
      selectedCodes,
      placingOrder,
      otherPaymentInfo
    } = this.state;

    const isUcClient = this.state.payment.clientType === "uc";
    const processingFee = isUcClient
      ? internalProcessingFee
      : externalProcessingFee;

    return (
      <div>
        <div>
          <div className="form_wrap">
            <ViewMode
              placingOrder={placingOrder}
              switchView={this._switchViews}
            />
          </div>

          <Collapse in={placingOrder}>
            <div className="form_wrap">
              <label className="form_header">Do you have a Client ID?</label>
              <ClientId
                clientId={clientId}
                clientName={clientName}
                handleChange={this._handleChange}
                clientIdRef={inputRef => {
                  this.clientIdRef = inputRef;
                }}
                newClientInfo={newClientInfo}
                updateNewClientInfo={this._updateNewClientInfo}
              />
            </div>
          </Collapse>

          <Collapse
            in={
              !placingOrder ||
              (this.state.clientName != null ||
                (this.state.newClientInfo.name != null &&
                  !!this.state.newClientInfo.name.trim()) ||
                !!this.state.payment.clientType.trim())
            }
          >
            <div className="form_wrap">
              <label className="form_header">
                How will you pay for your order?
              </label>
              <PaymentSelection
                placingOrder={placingOrder}
                payment={payment}
                checkChart={this._checkUcChart}
                getCampus={this._getCampus}
                onPaymentSelected={this._onPaymentSelected}
                otherPaymentInfo={otherPaymentInfo}
                updateOtherPaymentInfo={this._updateOtherPaymentInfo}
                updateOtherPaymentInfoType={this._changeOtherPaymentInfoType}
                otherPaymentInfoRef={inputRef => {
                  this.otherPaymentInfoRef = inputRef;
                }}
                ucAccountRef={inputRef => {
                  this.ucAccountRef = inputRef;
                }}
              />
            </div>
          </Collapse>

          <Collapse
            in={
              placingOrder &&
              (!!this.state.payment.clientType.trim() ||
                !!this.state.project.trim())
            }
          >
            <div className="form_wrap">
              <label className="form_header">
                What is the project title associated with this order?
              </label>
              <Project
                project={project}
                handleChange={this._handleChange}
                projectRef={inputRef => {
                  this.projectRef = inputRef;
                }}
              />
              <Commodity
                commodity={commodity}
                handleChange={this._handleChange}
              />
            </div>
          </Collapse>

          <Collapse
            in={
              !placingOrder ||
              (!!this.state.project.trim() ||
                this.state.quantity > 0 ||
                !!this.state.sampleType.trim())
            }
          >
            <div>
              {placingOrder && (
                <div className="form_wrap">
                  <label className="form_header">
                    Who should receive emails and results for this sample
                    submission?
                  </label>
                  <AdditionalEmails
                    addedEmails={additionalEmails}
                    defaultEmail={defaultEmail}
                    onEmailAdded={this._onEmailAdded}
                    onDeleteEmail={this._onDeleteEmail}
                  />
                </div>
              )}

              <div className="form_wrap">
                <label className="form_header">
                  How many samples will you be submitting?
                </label>
                <Quantity
                  quantity={quantity}
                  onQuantityChanged={this._onQuantityChanged}
                  quantityRef={numberRef => {
                    this.quantityRef = numberRef;
                  }}
                />
              </div>
            </div>
          </Collapse>

          <Collapse
            in={
              !placingOrder ||
              this.state.quantity > 0 ||
              this.state.sampleType !== ""
            }
          >
            <div className="form_wrap">
              <label className="form_header">What type of samples?</label>
              <SampleTypeSelection
                sampleType={sampleType}
                onSampleSelected={this._onSampleSelected}
              />
              {placingOrder && (
                <SampleTypeQuestions
                  waterPreservativeRef={inputRef => {
                    this.waterPreservativeRef = inputRef;
                  }}
                  sampleType={sampleType}
                  questions={sampleTypeQuestions}
                  handleChange={this._onSampleQuestionChanged}
                />
              )}
            </div>
          </Collapse>

          <Collapse in={this.state.sampleType !== ""}>
            <div>
              {placingOrder && (
                <div className="form_wrap">
                  <label className="form_header">
                    Do you have any comments?
                  </label>
                  <AdditionalInfo
                    value={additionalInfo}
                    name="additionalInfo"
                    handleChange={this._handleChange}
                  />
                </div>
              )}

              <div className="form_wrap">
                <label className="form_header margin-bottom-zero">
                  What tests would you like?
                </label>
                <p className="help-block">
                  Note: Tests selected are assigned to all the samples in the
                  order
                </p>
                <TestList
                  items={filteredTests}
                  selectedCodes={selectedCodes}
                  clientType={payment.clientType}
                  onTestSelectionChanged={this._onTestSelectionChanged}
                  additionalInfoList={additionalInfoList}
                  updateAdditionalInfo={this._updateAdditionalInfo}
                />
              </div>
            </div>
          </Collapse>

          <div className="stickyfoot" data-spy="affix" data-offset-bottom="0">
            <div className="summary-container shadowed">
              <Summary
                isCreate={this.props.orderId === null}
                canSubmit={this.state.isValid}
                isSubmitting={this.state.isSubmitting}
                hideError={
                  !placingOrder || this.state.isValid || this.state.isSubmitting
                }
                selectedTests={selectedTests}
                quantity={quantity}
                clientType={payment.clientType}
                onSubmit={this._onSubmit}
                status={status}
                processingFee={processingFee}
                handleErrors={this._handleErrors}
                placingOrder={placingOrder}
                switchViews={this._switchViews}
              />
            </div>
          </div>
        </div>

        <Modal show={this.state.isErrorActive}>
          <Modal.Header>
            <Modal.Title>Errors Detected</Modal.Title>
          </Modal.Header>
          <Modal.Body>{this.state.errorMessage}</Modal.Body>
          <Modal.Footer>
            <Button onClick={this._handleDialogToggle}>Got it!</Button>
          </Modal.Footer>
        </Modal>
      </div>
    );
  }

  private _validate = () => {
    // default valid
    let valid = true;

    //check either client name or new client info
    if (
      this.state.clientName == null &&
      (!this.state.newClientInfo.name || !this.state.newClientInfo.name.trim())
    ) {
      valid = false;
    }
    if (
      this.state.payment.clientType === "uc" &&
      this._checkUcChart(this.state.payment.account.charAt(0)) &&
      this.state.payment.accountName == null
    ) {
      valid = false;
    }
    if (
      this.state.payment.clientType === "uc" &&
      !this._checkUcChart(this.state.payment.account.charAt(0)) &&
      !this._checkOtherPaymentInfo()
    ) {
      valid = false;
    }

    if (
      this.state.payment.clientType === "other" &&
      !this._checkOtherPaymentInfo()
    ) {
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
    if (
      this.state.sampleType === "Water" &&
      this.state.sampleTypeQuestions.waterPreservativeAdded &&
      (!this.state.sampleTypeQuestions.waterPreservativeInfo ||
        !this.state.sampleTypeQuestions.waterPreservativeInfo.trim())
    ) {
      valid = false;
    }

    // check uc account requirements
    if (
      this.state.payment.clientType === "uc" &&
      (!this.state.payment.account || !this.state.payment.account.trim())
    ) {
      valid = false;
    }

    // push valid
    this.setState({ isValid: valid });
  };

  private _onPaymentSelected = (payment: any) => {
    this._changeOtherPaymentInfoType(
      payment.clientType,
      this.state.otherPaymentInfo.agreementRequired
    );
    this.setState({ payment }, this._validate);
  };

  private _checkUcChart = (chart: string) => {
    return (
      chart === "L" ||
      chart === "l" ||
      chart === "3" ||
      chart === "S" ||
      chart === "s" ||
      chart === "M" ||
      chart === "m"
    );
  };
    private _getCampus = (chart: string) => {
        if (!chart) {
            return ("");
        }
        const chrt = chart.toUpperCase();
        switch (chrt) {
        case "1":
        case "J":
            return ("UC Berkeley");
        case "2":
        case "K":
            return ("UC San Francisco");
        case "4":
                return ("UC Los Angeles");
        case "5":
        case "N":
                return ("UC Riverside");
        case "6":
        case "O":
                return ("UC San Diego");
        case "7":
        case "P":
                return ("UC Santa Cruz");
        case "8":
        case "Q":
            return ("UC Santa Barbara");
        case "9":
        case "R":
                return ("UC Irvine");
        case "0":
        case "S":
                return ("UC Merced");
        default:
            return ("");
        }
    };

  private _onSampleSelected = (sampleType: string) => {
    var agree = true;
    if (this.state.placingOrder && this.state.selectedTests.length > 0) {
      agree = confirm(
        "You may only choose tests from one category. Your previous progress will not be saved on submit."
      );
    }
    if (agree) {
      const filteredTests = this.props.testItems.filter(
        item => item.categories.indexOf(sampleType) !== -1
      );
      const selectedTests = filteredTests.filter(
        t => !!this.state.selectedCodes[t.id]
      );

      this.setState(
        { filteredTests, selectedTests, sampleType },
        this._validate
      );
    }
  };

  private _onTestSelectionChanged = (test: ITestItem, selected: boolean) => {
    const selectedCodes = {
      ...this.state.selectedCodes,
      [test.id]: selected
    };
    const selectedTests = this.state.filteredTests.filter(
      t => !!selectedCodes[t.id]
    );

    this.setState({ selectedCodes, selectedTests }, this._validate);
  };

  private _onSampleQuestionChanged = (question: string, answer: any) => {
    this.setState(
      {
        sampleTypeQuestions: {
          ...this.state.sampleTypeQuestions,
          [question]: answer
        }
      },
      this._validate
    );
  };

  private _onQuantityChanged = (quantity?: number) => {
    this.setState({ quantity }, this._validate);
  };

  private _updateNewClientInfo = (info: INewClientInfo) => {
    this.setState(
      {
        newClientInfo: { ...info }
      },
      this._validate
    );
  };

  private _onEmailAdded = (additionalEmail: string) => {
    this.setState({
      additionalEmails: [...this.state.additionalEmails, additionalEmail]
    });
  };

  private _onDeleteEmail = (email2Delete: any) => {
    const index = this.state.additionalEmails.indexOf(email2Delete);
    if (index > -1) {
      const shallowCopy = [...this.state.additionalEmails];
      shallowCopy.splice(index, 1);
      this.setState({ additionalEmails: shallowCopy });
    }
  };

  private _handleErrors = () => {
    if (this.state.isValid || this.state.isSubmitting) {
      return;
    }
    if (
      this.state.clientName == null &&
      (!this.state.newClientInfo.name || !this.state.newClientInfo.name.trim())
    ) {
      this._focusInput(this.clientIdRef);
    } else if (
      this.state.payment.clientType === "uc" &&
      (!this.state.payment.account ||
        !this.state.payment.account.trim() ||
        (this._checkUcChart(this.state.payment.account.charAt(0)) &&
          this.state.payment.accountName == null))
    ) {
      this._focusInput(this.ucAccountRef);
    } else if (
      (this.state.payment.clientType === "other" &&
        !this._checkOtherPaymentInfo()) ||
      (this.state.payment.clientType === "uc" &&
        !this._checkUcChart(this.state.payment.account.charAt(0)) &&
        !this._checkOtherPaymentInfo())
    ) {
      this._focusInput(this.otherPaymentInfoRef);
    } else if (!this.state.project || !this.state.project.trim()) {
      this._focusInput(this.projectRef);
    } else if (this.state.quantity <= 0 || this.state.quantity > 100) {
      this._focusInput(this.quantityRef);
    } else if (
      this.state.sampleType === "Water" &&
      this.state.sampleTypeQuestions.waterPreservativeAdded &&
      (!this.state.sampleTypeQuestions.waterPreservativeInfo ||
        !this.state.sampleTypeQuestions.waterPreservativeInfo.trim())
    ) {
      this._focusInput(this.waterPreservativeRef);
    }
  };

  private _focusInput = (component: any) => {
    component.focus();
    component.blur();
    component.focus();
  };

  private _updateAdditionalInfo = (id: string, value: string) => {
    this.setState(
      {
        additionalInfoList: {
          ...this.state.additionalInfoList,
          [id]: value
        }
      },
      this._validate
    );
  };

  private _updateOtherPaymentInfo = (property, value) => {
    if (property === "agreementRequired") {
      this._changeOtherPaymentInfoType("other", value);
      return;
    }
    this.setState(
      {
        ...this.state,
        otherPaymentInfo: {
          ...this.state.otherPaymentInfo,
          [property]: value
        }
      },
      this._validate
    );
  };

  private _changeOtherPaymentInfoType = (
    clientType: string,
    agreementRequired: boolean
  ) => {
    var paymentType = "";
    if (clientType === "uc") paymentType = "IOC";
    else if (clientType === "other")
      paymentType = agreementRequired ? "Agreement" : "PO";
    this.setState(
      {
        otherPaymentInfo: {
          ...this.state.otherPaymentInfo,
          agreementRequired: agreementRequired,
          paymentType: paymentType
        }
      },
      this._validate
    );
  };

  private _checkOtherPaymentInfo = () => {
    return (
      !!this.state.otherPaymentInfo.acAddr.trim() &&
      !!this.state.otherPaymentInfo.acEmail.trim() &&
      !!this.state.otherPaymentInfo.acName.trim() &&
      !!this.state.otherPaymentInfo.acPhone.trim() &&
      !!this.state.otherPaymentInfo.companyName.trim() &&
      !!this.state.otherPaymentInfo.paymentType.trim() &&
      (this.state.payment.clientType === "uc" ||
        this.state.otherPaymentInfo.agreementRequired ||
        !!this.state.otherPaymentInfo.poNum.trim())
    );
  };

  private _handleChange = (name, value) => {
    this.setState({ [name]: value }, this._validate);
  };

  private _switchViews = (viewName: string) => {
    if (
      (this.state.placingOrder && viewName === "browse") ||
      (!this.state.placingOrder && viewName === "order")
    ) {
      window.scrollTo(0, 0);
      this.setState({ placingOrder: !this.state.placingOrder });
    }
  };

  private _handleDialogToggle = () => {
    this.setState({ isErrorActive: !this.state.isErrorActive });
  };

  private _onSubmit = () => {
    // lock for duplicate submits
    if (this.state.isSubmitting) {
      return;
    }
    this.setState({ isSubmitting: true });

    // find selected tests and associated additional info, map to dictionary array
    const selectedCodes = Object.keys(this.state.selectedCodes).filter(
      k => !!k
    );

    // return in dictionary format
    const additionalInfoList = Object.keys(this.state.additionalInfoList)
      .filter(k => selectedCodes.indexOf(k) > -1)
      .map(k => ({ key: k, value: this.state.additionalInfoList[k] }));

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
      otherPaymentInfo: this.state.otherPaymentInfo,
      payment: this.state.payment,
      project: this.state.project,
      quantity: this.state.quantity,
      sampleType: this.state.sampleType,
      sampleTypeQuestions: this.state.sampleTypeQuestions,
      selectedTests: this.state.selectedTests
    };
    //If a CC, clear out the otherPaymentInfo to avoid triggering validation server side.
    if (
      order.payment.clientType === "creditcard" ||
      (order.payment.clientType === "uc" &&
        this._checkUcChart(order.payment.account.charAt(0)))
    ) {
      order.otherPaymentInfo = null;
    }

    // submit request to server
    const that = this;
    const postUrl = "/Order/Save";
    const returnUrl = "/Order/Confirmation/";
    const antiforgery = $("input[name='__RequestVerificationToken']").val();
    $.post({
      url: postUrl,
      data: { model: order, __RequestVerificationToken: antiforgery }
    })
      .success(response => {
        if (response.success === true) {
          const redirectId = response.id;
          window.location.replace(returnUrl + redirectId);
        } else {
          that.setState({
            isSubmitting: false,
            isErrorActive: true,
            errorMessage: response.message
          });
        }
      })
      .error(() => {
        that.setState({
          isSubmitting: false,
          isErrorActive: true,
          errorMessage: "An internal error occured..."
        });
      });
  };
}
