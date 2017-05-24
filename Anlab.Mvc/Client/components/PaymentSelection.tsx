import * as React from 'react';
import { RadioGroup, RadioButton } from 'react-toolbox/lib/radio';
import Input from 'react-toolbox/lib/input';
import TooltipWrapper  from './tooltipWrapper/tooltipWrapper';

export interface IPayment {
    clientType: string;
    account?: string;
}

interface IPaymentProps {
    payment: IPayment;
    onPaymentSelected: Function;
}



export class PaymentSelection extends React.Component<IPaymentProps, any> {
    _renderUcAccount = () => {
        if (this.props.payment.clientType !== 'uc') {
            return null;
        }

        return (
            <Input type="text" label="UC Account" value={this.props.payment.account} maxLength={10} onChange={this.handleAccountChange}/>
        );
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
        return (
            <TooltipWrapper tooltip="This is some descriptive text for the payment section" tooltipDelay={1000} tooltipPosition={"left"}>
                <RadioGroup name='comic' value={this.props.payment.clientType} onChange={this.handleChange}>
                    <RadioButton className="anlab_radio" label='Paying with UC Account' value='uc' />
                    <RadioButton className="anlab_radio" label='Paying with Credit Card' value='other'/>
                </RadioGroup>

                {this._renderUcAccount()}
                <h2 className="form_header">How will you pay for your order?</h2>
                <div className="row">
                <div className="anlab_payment_option col-5"><h3>Credit Card</h3><p>It's amazing what you can do with a little love in your heart.</p></div>
                <span className="dividing_span col-2 t-center align-middle">or</span>
                <div className="anlab_payment_option col-5"><h3>UC Funds</h3><p>It's amazing what you can do with a little love in your heart</p></div></div>

            </TooltipWrapper>
        );
    }
}
