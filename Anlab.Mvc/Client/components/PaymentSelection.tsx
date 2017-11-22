import "isomorphic-fetch";
import * as React from "react";
import Input from "./ui/input/input";
import { OtherPaymentInfo, IOtherPaymentInfo } from "./OtherPaymentQuestions";

export interface IPayment {
    clientType: string;
    account?: string;
    accountName?: string;
}

interface IPaymentSelectionProps {
    payment: IPayment;
    onPaymentSelected: (payment: IPayment) => void;
    otherPaymentInfo: IOtherPaymentInfo;
    updateOtherPaymentInfo: (property, value) => void;
    ucAccountRef: (element: HTMLInputElement) => void;
    placingOrder: boolean;
}

interface IPaymentSelectionState {
  accountName: string;
  error: string;
}

export class PaymentSelection extends React.Component<IPaymentSelectionProps, IPaymentSelectionState> {
    constructor(props) {
        super(props);
        this.state = {
            accountName: props.payment.accountName,
            error: "",
        };
    }

    public shouldComponentUpdate(nextProps: IPaymentSelectionProps, nextState: IPaymentSelectionState) {
        return (
            nextProps.placingOrder !== this.props.placingOrder ||
            nextProps.payment.clientType !== this.props.payment.clientType ||
            nextProps.payment.account !== this.props.payment.account ||
            nextProps.onPaymentSelected !== this.props.onPaymentSelected ||
            nextProps.otherPaymentInfo !== this.props.otherPaymentInfo || 
            nextState.accountName !== this.state.accountName ||
            nextState.error !== this.state.error 
        );
    }

    public render() {
        const activeDiv = "anlab_form_style anlab_form_samplebtn active-bg flexcol active-border active-svg active-text";
        const inactiveDiv = "anlab_form_style anlab_form_samplebtn flexcol";
        const isUcClient = this.props.payment.clientType === "uc";
        const isCC = this.props.payment.clientType === "creditcard";
        const isOther = this.props.payment.clientType === "other";

        return (
            <div>

                <div className="flexrow">
                    <div
                        className={isCC ? activeDiv : inactiveDiv}
                        onClick={() => this._handleChange("creditcard")}
                    >
                        <h3>Credit Card</h3>
                        <p>It's amazing what you can do with a little love in your heart.</p>
                    </div>
                    <div
                        className={isUcClient ? activeDiv : inactiveDiv}
                        onClick={() => this._handleChange("uc")}
                    >
                        <h3>UC Funds</h3>
                        <p>It's amazing what you can do with a little love in your heart</p>
                    </div>
                    <div
                        className={isOther ? activeDiv : inactiveDiv}
                        onClick={() => this._handleChange("other")}
                    >
                        <h3>Other</h3>
                        <p>It's amazing what you can do with a little love in your heart</p>
                    </div>
                </div>
                {this.props.placingOrder && this._renderUcAccount()}
                {this.props.placingOrder && this._renderOtherInfo()}

            </div>
        );
    }

    private _renderUcAccount = () => {
        if (this.props.payment.clientType === "uc") {
            return (
                <div>
                    <Input
                      label="UC Account"
                      value={this.props.payment.account}
                      error={this.state.error}
                      maxLength={50}
                      onChange={this._handleAccountChange}
                      onBlur={this._lookupAccount}
                      inputRef={this.props.ucAccountRef}
                    />
                    {this.state.accountName}
                </div>
            );
        }
    }

    private _renderOtherInfo = () => {
        if (this.props.payment.clientType === "other" || (this.props.payment.clientType === "uc" && this.props.payment.account != null &&
            !!this.props.payment.account.trim() && !this._checkChart(this.props.payment.account.charAt(0)))) {
            return (
                <OtherPaymentInfo otherPaymentInfo={this.props.otherPaymentInfo} updateOtherPaymentInfo={this.props.updateOtherPaymentInfo} />
                );
        }
    }



    private _checkChart = (chart: string) => {
        return (chart === "L" || chart === "l" || chart === "3") ;
    }

    private _lookupAccount = () => {
        if (this.state.error
          || !this.props.payment.account
          || !this._checkChart(this.props.payment.account.charAt(0))) {
            this.setState({ accountName: null });
            this.props.onPaymentSelected({ ...this.props.payment, accountName: null });
          return;
        }

        fetch(`/financial/info?account=${this.props.payment.account}`, { credentials: "same-origin" })
            .then((response) => {
                if (!response.ok) {
                    throw Error("The account you entered could not be found");
                }
                return response;
            })
            .then((response) => response.json())
            .then((accountName) => {
                this.setState({ accountName });
                this.props.onPaymentSelected({ ...this.props.payment, accountName: accountName });
            })
            .catch((error: Error) => {
                this.setState({ accountName: null, error: error.message });
 
                this.props.onPaymentSelected({ ...this.props.payment, account: null, accountName: null });
            });
    }

    private _handleChange = (clientType: string) => {
        this._validateAccount(this.props.payment.account, clientType);
        this.props.onPaymentSelected({ ...this.props.payment, clientType });
    }

    private _handleAccountChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const account = e.target.value;
        this._validateAccount(account, this.props.payment.clientType);
        this.props.onPaymentSelected({ ...this.props.payment, account });
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
