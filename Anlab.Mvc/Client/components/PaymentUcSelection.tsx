import "isomorphic-fetch";
import * as React from "react";
import { getOptions } from "../util/ucAccountsHelper";
import Input from "./ui/input/input";

export interface IPayment {
  clientType: string;
  account?: string;
  accountName?: string;
  isUcdAccount?: boolean;
}

interface IPaymentUcSelectionState {
  ucName: string;
}

interface IPaymentUcSelectionProps {
    payment: IPayment;
    error: string;
    handleAccountChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
    lookupAccount: () => void;
    ucAccountRef: (element: HTMLInputElement) => void;
    handleSelectionchange: (isUcdAccount: boolean) => void;
}

export class PaymentUcSelection extends React.Component<
  IPaymentUcSelectionProps,
  IPaymentUcSelectionState
> {
  constructor(props) {
    super(props);
    this.state = {
      ucName: "UCD",
    };
  }

  public render() {

    const options = getOptions();

    return (
        <div>
            <div>
                <label>Select UC</label>
                <select
                    className="form-control"
                    value={this.state.ucName}
                    onChange={(e) => this._handleSelectionChange(e.target.value)}
                >
                <option value="UCB">UCB</option>
                <option value="UCSF">UCSF</option>
                <option value="UCD">UCD</option>
                <option value="UCLA">UCLA</option>
                <option value="UCR">UCR</option>
                <option value="UCSD">UCSD</option>
                <option value="UCSC">UCSC</option>
                <option value="UCSB">UCSB</option>
                <option value="UCI">UCI</option>
                <option value="UCM">UCM</option>
                <option value="MOP">M-OP</option>
                </select>
            </div>
            <div className="row">
                <div className="col-5">
                    <Input
                        label={`${this.state.ucName} Account`}
                        value={this.props.payment.account}
                        error={this.props.error}
                        maxLength={50}
                        onChange={this.props.handleAccountChange}
                        onBlur={this.props.lookupAccount}
                        inputRef={this.props.ucAccountRef}
                    />
                {this.props.payment.accountName}
                </div>
                {this._renderDetails(options[this.state.ucName])}
            </div>
      </div>
    );
  }

  private _handleSelectionChange = (uc: string) => {
      this.setState({ ucName: uc });
      this.props.handleSelectionchange(uc === "UCD");
  }

  private _renderDetails = (option: any) => {
    if (!option) {
      return;
    }
    return(
        <div className="col-5">
            <Input
                label={`Example ${option.name} Account`}
                value={option.example}
                disabled={true}
                feedback={option.detail}
            />
        </div>
    );
  }
}
