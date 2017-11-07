import "isomorphic-fetch";
import * as React from "react";
import Input from "./ui/input/input";

export interface IPayment {
    clientType: string;
    account?: string;
}

interface IPaymentSelectionProps {
    payment: IPayment;
    onPaymentSelected: (payment: IPayment) => void;
}

interface IPaymentSelectionState {
  accountName: string;
  error: string;
}

export class PaymentSelection extends React.Component<IPaymentSelectionProps, IPaymentSelectionState> {
    constructor(props) {
        super(props);
        this.state = {
            accountName: null,
            error: "",
        };
    }

    public shouldComponentUpdate(nextProps: IPaymentSelectionProps, nextState: IPaymentSelectionState) {
        return (
            nextProps.payment.clientType !== this.props.payment.clientType ||
            nextProps.payment.account !== this.props.payment.account ||
            nextProps.onPaymentSelected !== this.props.onPaymentSelected ||
            nextState.accountName !== this.state.accountName ||
            nextState.error !== this.state.error
        );
    }

    public render() {
        const activeDiv = "anlab_form_style col-5 active-border active-text active-bg";
        const inactiveDiv = "anlab_form_style col-5";
        const isUcClient = this.props.payment.clientType === "uc";

        return (
            <div className="form_wrap">
                <h2 className="form_header">How will you pay for your order?</h2>
                <div className="row">
                    <div
                      className={!isUcClient ? activeDiv : inactiveDiv}
                      onClick={() => this._handleChange("creditcard")}
                    >
                        <h3>Credit Card</h3>
                        <p>It's amazing what you can do with a little love in your heart.</p>
                    </div>
                    <span className="dividing_span col-2 t-center align-middle">or</span>
                    <div
                      className={isUcClient ? activeDiv : inactiveDiv}
                      onClick={() => this._handleChange("uc")}
                    >
                        <h3>UC Funds</h3>
                        <p>It's amazing what you can do with a little love in your heart</p>
                    </div>
                </div>
                {this._renderUcAccount()}
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
                    />
                    {this.state.accountName}
                </div>
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
            .then((accountName) => this.setState({ accountName }))
            .catch((error: Error) => {
                this.setState({ accountName: null, error: error.message, });
                const account = null;
                this.props.onPaymentSelected({ ...this.props.payment, account });
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
