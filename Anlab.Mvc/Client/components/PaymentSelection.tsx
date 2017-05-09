﻿import * as React from 'react';
import { RadioGroup, RadioButton } from 'react-toolbox/lib/radio';
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
    handleChange = (clientType: string) => {
        var updatedPaymentInfo = { ...this.props.payment, clientType };

        this.props.onPaymentSelected(updatedPaymentInfo);
    }



    render() {
        return (
            <TooltipWrapper tooltip="This is some descriptive text for the payment section" tooltipDelay={1000} tooltipPosition={"left"}>
                <RadioGroup name='comic' value={this.props.payment.clientType} onChange={this.handleChange}>
                    <RadioButton label='Paying with UC Account' value='uc'/>
                    <RadioButton label='Paying with Credit Card' value='other'/>
                </RadioGroup>
            </TooltipWrapper>
        );
    }
}