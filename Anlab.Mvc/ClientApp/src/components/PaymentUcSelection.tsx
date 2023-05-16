import "isomorphic-fetch";
import * as React from "react";
import { getOptions } from "../util/ucAccountsHelper";
import { env } from "../util/env";
import Input from "./ui/input/input";
import { Button } from "react-bootstrap";

export interface IPayment {
  clientType: string;
  account?: string;
  accountName?: string;
  isUcdAccount?: boolean;
}

interface IPaymentUcSelectionProps {
  payment: IPayment;
  error: string;
  ucName: string;
  handleAccountChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
  lookupAccount: () => void;
  ucAccountRef: (element: HTMLInputElement) => void;
  handleSelectionChange: (ucName: string) => void;
}

declare const window: Window &
  typeof globalThis & {
    Finjector: any;
  };

export class PaymentUcSelection extends React.Component<
  IPaymentUcSelectionProps,
  {}
> {
  public render() {
    const options = getOptions(env.useCoa);

    return (
      <div>
        <div>
          <label>Select UC</label>
          <select
            className="form-control"
            value={this.props.ucName}
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
          </select>
        </div>
        {this._renderDetails(options[this.props.ucName])}
        <div className="flexrow coa-input-row">
          <div className="col-4">
            <Input
              label={`${this.props.ucName} Account`}
              value={this.props.payment.account}
              error={this.props.error}
              maxLength={100}
              onChange={this.props.handleAccountChange}
              onBlur={this.props.lookupAccount}
              inputRef={this.props.ucAccountRef}
            />

            {this.props.payment.accountName}
          </div>
          {env.useCoa && this.props.ucName === "UCD" && (
            <div className="col-3 coa-wrapper">
              <Button className="btn-coa" onClick={this._lookupcoa}>
                COA Picker
              </Button>
            </div>
          )}
        </div>
      </div>
    );
  }

  private _handleSelectionChange = (uc: string) => {
    this.props.handleSelectionChange(uc);
  };

  private _renderDetails = (option: any) => {
    if (!option) {
      return;
    }
    return (
      <div className="mt-2 mb-2">
        <Input
          label={`Example ${option.name} Account`}
          value={option.example}
          disabled={true}
          feedback={option.detail}
        />
      </div>
    );
  };
  private _lookupcoa = async () => {
    const chart = await window.Finjector.findChartSegmentString();

    if (chart.status === "success") {
      this.props.handleAccountChange({
        target: { value: chart.data },
      } as React.ChangeEvent<HTMLInputElement>);
      //then call this.props.lookupAccount
      this.props.lookupAccount();
    }
  };
}
