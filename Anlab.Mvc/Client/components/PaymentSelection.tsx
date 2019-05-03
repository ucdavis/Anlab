import "isomorphic-fetch";
import * as React from "react";
import { Checkbox } from "react-bootstrap";
import { IOtherPaymentInfo, OtherPaymentInfo } from "./OtherPaymentQuestions";
import { PaymentUcSelection } from "./PaymentUcSelection";
import Input from "./ui/input/input";

export interface IPayment {
  clientType: string;
  account?: string;
  accountName?: string;
  isUcdAccount?: boolean;
}

interface IPaymentSelectionProps {
  payment: IPayment;
  checkChart: Function;
  onPaymentSelected: (payment: IPayment) => void;
  otherPaymentInfo: IOtherPaymentInfo;
  updateOtherPaymentInfo: (property, value) => void;
  updateOtherPaymentInfoType: (clientType, agreementRequired) => void;
  ucAccountRef: (element: HTMLInputElement) => void;
  otherPaymentInfoRef: (element: HTMLInputElement) => void;
  placingOrder: boolean;
}

interface IPaymentSelectionState {
  error: string;
  ucName: string;
}

export class PaymentSelection extends React.Component<
  IPaymentSelectionProps,
  IPaymentSelectionState
> {
  constructor(props) {
    super(props);
    this.state = {
      error: "",
      ucName: "UCD",
    };
    this._lookupAccount();
  }

  public shouldComponentUpdate(
    nextProps: IPaymentSelectionProps,
    nextState: IPaymentSelectionState,
  ) {
    return (
      nextProps.placingOrder !== this.props.placingOrder ||
      nextProps.payment.clientType !== this.props.payment.clientType ||
      nextProps.payment.account !== this.props.payment.account ||
      nextProps.payment.accountName !== this.props.payment.accountName ||
      nextProps.onPaymentSelected !== this.props.onPaymentSelected ||
      nextProps.otherPaymentInfo !== this.props.otherPaymentInfo ||
      nextProps.payment.isUcdAccount !== this.props.payment.isUcdAccount ||
      nextState.error !== this.state.error ||
      nextState.ucName !== this.state.ucName
    );
  }

  public render() {
    const activeDiv =
      "anlab_form_style anlab_form_samplebtn active-bg flexcol active-border active-svg active-text";
    const inactiveDiv = "anlab_form_style anlab_form_samplebtn flexcol";
    const isUcClient = this.props.payment.clientType === "uc";
    const isCC = this.props.payment.clientType === "creditcard";
    const isOther = this.props.payment.clientType === "other";

    return (
      <div>
        <div className="flexrow">
          <div
            className={isUcClient ? activeDiv : inactiveDiv}
            onClick={() => this._handleChange("uc")}
          >
            <h3>UC Funds</h3>
            <p>Required to receive the UC Rate.</p>
          </div>
          <div
            className={isCC ? activeDiv : inactiveDiv}
            onClick={() => this._handleChange("creditcard")}
          >
            <h3>Credit Card</h3>
            <p>
              Your credit card information will be collected at a later date.
            </p>
          </div>
          <div
            className={isOther ? activeDiv : inactiveDiv}
            onClick={() => this._handleChange("other")}
          >
            <h3>Other</h3>
            <p>Payment by PO or Bank Transfer.</p>
          </div>
        </div>
        {this.props.placingOrder && this._renderheading()}
        {this.props.placingOrder && this._renderUcAccount()}
        {this.props.placingOrder && this._renderOtherInfo()}
        {this._renderAgreement()}
      </div>
    );
  }

  private _renderUcAccount = () => {
    if (this.props.payment.clientType === "uc") {
      return (
        <div>
          <PaymentUcSelection
            payment={this.props.payment}
            error={this.state.error}
            handleAccountChange={this._handleAccountChange}
            lookupAccount={this._lookupAccount}
            ucAccountRef={this.props.ucAccountRef}
            handleSelectionchange={this._handleSelectionChange}
          />
        </div>
      );
    }
  }

  private _renderheading = () => {
    if (
      this.props.payment.clientType === "uc" ||
      this.props.payment.clientType === "other"
    ) {
      return (
        <div>
          <h2 className="form_header">
            Please provide billing account information:
          </h2>
        </div>
      );
    }
  }

  private _renderAgreement = () => {
    if (!this.props.placingOrder || this.props.payment.clientType !== "other") {
      return;
    }
    return (
      <Checkbox
        checked={this.props.otherPaymentInfo.agreementRequired}
        onChange={this._changeAgreementReq}
        inline={true}
      >
        {" "}
        I require an agreement{" "}
      </Checkbox>
    );
  }

  private _changeAgreementReq = () => {
    this.props.updateOtherPaymentInfoType(
      this.props.payment.clientType,
      !this.props.otherPaymentInfo.agreementRequired,
    );
  }

  private _renderOtherInfo = () => {
    if (
      this.props.payment.clientType === "other" ||
      (this.props.payment.clientType === "uc" &&
        this.props.payment.account != null &&
        !!this.props.payment.account.trim() &&
        !this.props.checkChart(this.props.payment.account.charAt(0)))
    ) {
      return (
        <OtherPaymentInfo
          otherPaymentInfo={this.props.otherPaymentInfo}
          updateOtherPaymentInfo={this.props.updateOtherPaymentInfo}
          otherPaymentInfoRef={this.props.otherPaymentInfoRef}
          clientType={this.props.payment.clientType}
        />
      );
    }
  }

  private _lookupAccount = () => {
    if (
      this.state.error ||
      !this.props.payment.account ||
      !this.props.checkChart(this.props.payment.account.charAt(0))
    ) {
      this.props.onPaymentSelected({
        ...this.props.payment,
        accountName: null,
      });
      return;
    }

    this._accountLookup();
  }

  private _accountLookup = () => {
    fetch(`/financial/info?account=${this.props.payment.account}`, {
      credentials: "same-origin",
    })
      .then((response) => {
        if (!response.ok) {
          throw Error("The account you entered could not be found");
        }
        return response;
      })
      .then((response) => response.json())
      .then((accountName) => {
        this.props.onPaymentSelected({
          ...this.props.payment,
          accountName,
        });
      })
      .catch((error: Error) => {
        this.setState({ error: error.message });
        this.props.onPaymentSelected({
          ...this.props.payment,
          accountName: null,
        });
      });
  }

  private _handleChange = (clientType: string) => {
    this._validateAccount(this.props.payment.account, clientType);
    this.props.onPaymentSelected({ ...this.props.payment, clientType });
  }

  private _handleAccountChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const account = e.target.value;
    this._validateAccount(account, this.props.payment.clientType);
    this.props.onPaymentSelected({
      ...this.props.payment,
      account,
      accountName: null,
    });
  }

  private _handleSelectionChange = (isUcd: boolean) => {
    this.props.onPaymentSelected({
      ...this.props.payment,
      isUcdAccount: isUcd,
      accountName: null,
    });
    this._validateAccount(
      this.props.payment.account,
      this.props.payment.clientType,
    );
    if (isUcd) {
      this._accountLookup();
    }
  }

  private _validateAccount = (account: string, clientType: string) => {
    if (clientType !== "uc") {
      this.setState({ error: "" });
      return;
    }

    if (account && account.trim()) {
      this.setState({ error: "" });
      return;
    }

    this.setState({ error: "Account is required" });
  }
}
