import "isomorphic-fetch";
import * as React from "react";
import { Checkbox, Button } from "react-bootstrap";
import { IOtherPaymentInfo, OtherPaymentInfo } from "./OtherPaymentQuestions";
import { PaymentUcSelection } from "./PaymentUcSelection";
import Input from "./ui/input/input";
import { env } from "../util/env";

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
  changeSelectedUc: (payment: IPayment, ucName: string) => void;
  ucAccountRef: (element: HTMLInputElement) => void;
  otherPaymentInfoRef: (element: HTMLInputElement) => void;
  placingOrder: boolean;
  creatingOrder: boolean;
}

interface IPaymentSelectionState {
  error: string;
  ucName: string;
}

declare const window: Window &
  typeof globalThis & {
    Finjector: any;
  };

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
    nextState: IPaymentSelectionState
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
    if (this.props.payment.clientType !== "uc") {
      return;
    }
    return (
      <div>
        <p className="help-block">
          {env.useCoa && "UC Davis accounts require a valid PPM or GL COA"}
          {!env.useCoa && "UC Davis accounts require the chart."}{" "}
          <strong>
            <a
              href="https://afs.ucdavis.edu/our_services/accounting-e-financial-reporting/intercampus-transactions/other-uc-campus-info.html"
              target="blank"
            >
              Other campus IOC Account requirement instructions can be found
              here
            </a>
          </strong>
        </p>
        {this._renderUcAccountInput()}
      </div>
    );
  };

  private _renderUcAccountInput = () => {
    if (!this.props.creatingOrder) {
      return (
        <div className="flexrow coa-input-row">
          <div className="col-4">
            <Input
              label={
                this.props.payment.isUcdAccount
                  ? "UC Davis Account"
                  : "UC Account"
              }
              name="ucAccount"
              value={this.props.payment.account}
              error={this.state.error}
              maxLength={100}
              onChange={this._handleAccountChange}
              onBlur={this._lookupAccount}
              inputRef={this.props.ucAccountRef}
            />

            {this.props.payment.accountName}
          </div>

          {env.useCoa && this.props.payment.isUcdAccount && (
            <div className="col-3 coa-wrapper">
              <Button className="btn-coa" onClick={this._lookupcoa}>
                COA Picker
              </Button>
            </div>
          )}
        </div>
      );
    } else {
      return (
        <div>
          <PaymentUcSelection
            payment={this.props.payment}
            error={this.state.error}
            handleAccountChange={this._handleAccountChange}
            lookupAccount={this._lookupAccount}
            ucAccountRef={this.props.ucAccountRef}
            ucName={this.state.ucName}
            handleSelectionChange={this._handleSelectionChange}
          />
        </div>
      );
    }
  };

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
  };

  private _renderAgreement = () => {
    if (!this.props.placingOrder || this.props.payment.clientType !== "other") {
      return;
    }
    return (
      <div>
        <Checkbox
          checked={this.props.otherPaymentInfo.agreementRequired}
          onChange={this._changeAgreementReq}
          inline={true}
        >
          {" "}
          I require an agreement{" "}
        </Checkbox>
        {this.props.otherPaymentInfo.agreementRequired && (
          <div>
            <div className="alert alert-warning" role="alert">
              You have selected that you require an agreement. We can proceed
              with a general agreement, which would take a few days, or with a
              formal agreement, which may take six weeks or more to finalize. In
              either case, we cannot start work on this order until the
              agreement is signed. If you choose to continue, you may proceed
              with placing your order and you will be contacted by our lab. You
              may unclick the box to stop the agreement process.{" "}
            </div>
            <div className="alert alert-danger" role="alert">
              PLEASE NOTE: Due to the University updating its Financial System,
              PO Agreements will not be processed from 11/01/23 through
              01/03/24.
            </div>
          </div>
        )}
      </div>
    );
  };

  private _changeAgreementReq = () => {
    this.props.updateOtherPaymentInfoType(
      this.props.payment.clientType,
      !this.props.otherPaymentInfo.agreementRequired
    );
  };

  private _renderOtherInfo = () => {
    if (
      this.props.payment.clientType === "other" ||
      (this.props.payment.clientType === "uc" &&
        !this.props.payment.isUcdAccount)
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
  };

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
  };

  private _accountLookup = () => {
    fetch(`/financial/info?account=${this.props.payment.account}`, {
      credentials: "same-origin",
    })
      .then((response) => {
        if (response === null || response.status !== 200) {
          throw Error("The account you entered could not be found");
        }
        return response;
      })
      .then((response) => response.json())
      .then((response) => {
        if (!response.isValid) {
          this.setState({ error: response.message });
          this.props.onPaymentSelected({
            ...this.props.payment,
            accountName: null,
          });
        } else {
          this.props.onPaymentSelected({
            ...this.props.payment,
            accountName: response.displayName,
          });
        }
      })
      .catch((error: Error) => {
        this.setState({ error: error.message });
        this.props.onPaymentSelected({
          ...this.props.payment,
          accountName: null,
        });
      });
  };

  private _handleChange = (clientType: string) => {
    this._validateAccount(this.props.payment.account, clientType);
    this.props.onPaymentSelected({ ...this.props.payment, clientType });
  };

  private _handleAccountChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const account = e.target.value;
    this._validateAccount(account, this.props.payment.clientType);
    this.props.onPaymentSelected({
      ...this.props.payment,
      account,
      accountName: null,
    });
  };

  private _handleSelectionChange = (ucName: string) => {
    const isUcd = ucName === "UCD";
    this.setState({ ucName });
    this.props.changeSelectedUc(
      {
        ...this.props.payment,
        isUcdAccount: isUcd,
        accountName: null,
      },
      ucName
    );
    this._validateAccount(
      this.props.payment.account,
      this.props.payment.clientType
    );
    if (isUcd) {
      this._accountLookup();
    }
  };

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
  };

  private _lookupcoa = async () => {
    const chart = await window.Finjector.findChartSegmentString();

    if (chart.status === "success") {
      //Call handle account change to set the account
      this._handleAccountChange({
        target: { value: chart.data },
      } as React.ChangeEvent<HTMLInputElement>);
      this._lookupAccount();
    }
  };
}
