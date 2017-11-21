import * as React from "react";
import { OtherPaymentInput } from "./OtherPaymentInput";

export interface IOtherPaymentInfo {
    paymentType: string;
    companyName: string;
    acName: string;
    acAddr: string;
    acEmail: string;
    acPhone: string;
    poNum: string;
}

interface IOtherPaymentInfoProps {
    otherPaymentInfo: IOtherPaymentInfo;
    updateOtherPaymentInfo: Function;
}

export class OtherPaymentInfo extends React.Component<IOtherPaymentInfoProps, {}> {

    public render() {

        return (
            <div className="flexrow">
                <div className="flexcol">
                    
                <OtherPaymentInput property="paymentType" value={this.props.otherPaymentInfo.paymentType} label="Payment Type" handleChange={this._onOtherInfoChange} />
                <OtherPaymentInput property="companyName" value={this.props.otherPaymentInfo.companyName} label="Company Name" handleChange={this._onOtherInfoChange} />
                <OtherPaymentInput property="acName" value={this.props.otherPaymentInfo.acName} label="Account Contact Name" handleChange={this._onOtherInfoChange} />
                <OtherPaymentInput property="acAddr" value={this.props.otherPaymentInfo.acAddr} label="Account Contact Address" handleChange={this._onOtherInfoChange} />
                </div>
                <div className="flexcol">

                <OtherPaymentInput property="acEmail" value={this.props.otherPaymentInfo.acEmail} label="Account Contact Email" handleChange={this._onOtherInfoChange} />
                <OtherPaymentInput property="acPhone" value={this.props.otherPaymentInfo.acPhone} label="Account Contact Phone Number" handleChange={this._onOtherInfoChange} />
                <OtherPaymentInput property="poNum" value={this.props.otherPaymentInfo.poNum} label="PO # (if applicable)" handleChange={this._onOtherInfoChange} />
                </div>
                <div className="flexcol">
                </div>

            </div>);
    }

    private _onOtherInfoChange = (property: string, value: string) => {
        this.props.updateOtherPaymentInfo(property, value);
    }

}
