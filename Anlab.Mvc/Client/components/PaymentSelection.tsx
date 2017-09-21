import * as React from 'react';
import Input from 'react-toolbox/lib/input';

export interface IPayment {
    clientType: string;
    account?: string;
}

interface IPaymentProps {
    payment: IPayment;
    onPaymentSelected: Function;
    accountRef?: any;
}



export class PaymentSelection extends React.Component<IPaymentProps, any> {
    _renderUcAccount = () => {
        if (this.props.payment.clientType === 'uc') {
            return (
                <Input ref={this.props.accountRef} required={true} type="text" label="UC Account" value={this.props.payment.account} maxLength={10} onChange={this
                    .handleAccountChange}/>
            );
        }
    }
    handleChange = (clientType: string) => {
        var updatedPaymentInfo = { ...this.props.payment, clientType };
        this.props.onPaymentSelected(updatedPaymentInfo);
    }

    handleAccountChange = (account: string) => {
        var updatedPaymentInfo = { ...this.props.payment, account };
        this.props.onPaymentSelected(updatedPaymentInfo);
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
