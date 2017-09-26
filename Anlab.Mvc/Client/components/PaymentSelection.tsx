import * as React from 'react';
import Input from 'react-toolbox/lib/input';
import 'isomorphic-fetch';

export interface IPayment {
    clientType: string;
    account?: string;
}

interface IPaymentProps {
    payment: IPayment;
    onPaymentSelected: Function;
}



export class PaymentSelection extends React.Component<IPaymentProps, any> {
    constructor(props) {
        super(props);
        this.state = {
            error: "",
            accountName: null
        };


    }
    _renderUcAccount = () => {
        if (this.props.payment.clientType === 'uc') {
            return (
                <div>
                    <Input type="text" label="UC Account" error={this.state.error} value={this.props.payment.account} maxLength={15} onChange={this.handleAccountChange} onBlur={this.lookupAccount} />
                    {this.state.accountName}
                </div>
            );
        }
    }

    checkChart = (chart: string) => {
        if (chart === "L" || chart === "l" || chart === "3")
            return true;
        else
            return false;
    }

    lookupAccount = () => {
        if (!this.state.error && this.props.payment.account !== null && this.checkChart(this.props.payment.account.charAt(0)))
        {
            fetch(`/financial/${this.props.payment.account}`, { credentials: 'same-origin' })
                .then(response => {
                    if (!response.ok) {
                        throw Error();
                    }
                    return response;
                })
                .then(response => response.json())
                .then(accountName => this.setState({ accountName }))
                .catch(error => {
                    this.setState({ accountName: null, error: "The account you entered could not be found" });
                });
        }
        else
        {
            this.setState({ accountName: null });
        }
    }

    handleChange = (clientType: string) => {
        if (clientType === 'uc') {
            this.validateAccount(this.props.payment.account, clientType);
        }
        var updatedPaymentInfo = { ...this.props.payment, clientType };
        this.props.onPaymentSelected(updatedPaymentInfo);
    }

    handleAccountChange = (account: string) => {
        this.validateAccount(account, this.props.payment.clientType);
        var updatedPaymentInfo = { ...this.props.payment, account };
        this.props.onPaymentSelected(updatedPaymentInfo);
    }

    validateAccount = (account: string, clientType: string) => {
        if (clientType !== 'uc') {
            this.setState({ ...this.state, error: "" });
        } else {
            if (account === '' || account == undefined) {
                this.setState({ ...this.state, error: "Account is required" });
            } else {
                const re = /^(\w)-(\w{7})\/?(\w{5})?$/;
                if (!re.test((account))) {
                    this.setState(
                        { ...this.state, error: "The account must be in the format X-XXXXXXX or X-XXXXXXX/XXXXX" });
                } else {
                    this.setState({ ...this.state, error: "" });
                };
            }
        }
    }


    render() {
        const activeDiv = "anlab_form_style col-5 active-border active-text active-bg";
        const inactiveDiv = "anlab_form_style col-5";
        return (
            <div className="form_wrap">
                <h2 className="form_header">How will you pay for your order?</h2>
                <div className="row">
                    <div className={this.props.payment.clientType !== 'uc' ? activeDiv : inactiveDiv} onClick={() => this.handleChange("other")}>
                        <h3>Credit Card</h3>
                        <p>It's amazing what you can do with a little love in your heart.</p>                        
                    </div>
                    <span className="dividing_span col-2 t-center align-middle">or</span>
                    <div className={this.props.payment.clientType === 'uc' ? activeDiv : inactiveDiv} onClick= {()=>this.handleChange("uc")}>
                        <h3>UC Funds</h3>
                        <p>It's amazing what you can do with a little love in your heart</p>
                    </div>
                </div>
                {this._renderUcAccount()}
            </div>
        );
    }
}
