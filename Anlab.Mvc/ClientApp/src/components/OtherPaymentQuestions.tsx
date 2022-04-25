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
  agreementRequired: boolean;
}

interface IOtherPaymentInfoProps {
  otherPaymentInfo: IOtherPaymentInfo;
  updateOtherPaymentInfo: Function;
  otherPaymentInfoRef: any;
  clientType: string;
}

export class OtherPaymentInfo extends React.Component<
  IOtherPaymentInfoProps,
  {}
> {
  public render() {
    return (
      <div className="flexrow">
        <div className="flexcol">
          <OtherPaymentInput
            property="companyName"
            value={this.props.otherPaymentInfo.companyName}
            label={
              this.props.otherPaymentInfo.paymentType === "IOC"
                ? "Campus Name"
                : "Company Name"
            }
            handleChange={this._onOtherInfoChange}
            required={true}
            inputRef={this.props.otherPaymentInfoRef}
          />
          <OtherPaymentInput
            property="acName"
            value={this.props.otherPaymentInfo.acName}
            label="Billing Contact Name"
            handleChange={this._onOtherInfoChange}
            required={true}
          />
          <OtherPaymentInput
            property="acAddr"
            value={this.props.otherPaymentInfo.acAddr}
            label="Billing Contact Address"
            handleChange={this._onOtherInfoChange}
            required={true}
          />
        </div>
        <div className="flexcol">
          <OtherPaymentInput
            property="acEmail"
            value={this.props.otherPaymentInfo.acEmail}
            label="Billing Contact Email"
            handleChange={this._onOtherInfoChange}
            required={true}
          />
          <OtherPaymentInput
            property="acPhone"
            value={this.props.otherPaymentInfo.acPhone}
            label="Billing Contact Phone Number"
            handleChange={this._onOtherInfoChange}
            required={true}
          />
          {this.props.clientType === "other" && (
            <OtherPaymentInput
              property="poNum"
              value={this.props.otherPaymentInfo.poNum}
              label="PO #"
              handleChange={this._onOtherInfoChange}
              required={this.props.otherPaymentInfo.paymentType === "PO"}
            />
          )}
        </div>
      </div>
    );
  }

  private _onOtherInfoChange = (property: string, value: string) => {
    this.props.updateOtherPaymentInfo(property, value);
  };
}
