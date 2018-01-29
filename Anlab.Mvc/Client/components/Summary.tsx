import * as React from "react";
import NumberFormat from "react-number-format";
import {Button} from "react-bootstrap";
import { IPayment } from "./PaymentSelection";
import { ITestItem } from "./TestList";

export interface ISummaryProps {
    selectedTests: ITestItem[];
    quantity: number;
    clientType: string;
    onSubmit: Function;
    canSubmit: boolean;
    isCreate: boolean;
    hideError: boolean;
    status: string;
    processingFee: number;
    handleErrors: Function;
    placingOrder: boolean;
    switchViews: Function;
}

const numberFormatOptions = {
  decimalPrecision: 2,
  displayType: "text",
  prefix: "$",
  thousandSeparator: true,
};

export default class Summary extends React.PureComponent<ISummaryProps, {}> {

    public render() {
      if (this.props.selectedTests.length === 0) {
          return null;
      }

      const saveText = "Save & Review";
      const infoText = "You will be able to review this information before placing your order";
      const errorText = "Please correct any errors and complete any required fields before you proceed";

      return (
          <div>
              <div className="ordersum">
                  <div>
                      <h3>Order Total: <NumberFormat value={this._totalCost()} {...numberFormatOptions} /></h3>
                      <a role="button" data-toggle="collapse" data-target="#testSummary" aria-expanded="false" aria-controls="testSummary">
                          Order Details
                      </a>
                  </div>
                  <div>
                      {this._renderErrorButton()}
                      {this._renderSubmitButton()}
                  </div>
              </div>

              <div className="collapse" id="testSummary">
                  <div className="well">
                        <table className="table">
                            <thead>
                                <tr>
                                    <th>Analysis</th>
                                    <th>Fee {this._renderToolTipIcon("Price Per Sample")}</th>
                                    <th>Price {this._renderToolTipIcon("Fee * Quantity")}</th>
                                    <th>Setup</th>
                                    <th>Total {this._renderToolTipIcon("Price + Setup")}</th>
                                </tr>
                            </thead>
                            <tbody>
                                {this._renderTests()}
                            </tbody>
                            <tfoot>
                                <tr>
                                    <th>Processing Fee</th>
                                    <td colSpan={3} />
                                    <td><NumberFormat value={this.props.processingFee} {...numberFormatOptions}/></td>
                                </tr>
                                <tr>
                                    <td colSpan={4} />
                                    <td><NumberFormat id="totalCost" value={this._totalCost()} {...numberFormatOptions} /></td>
                                </tr>
                            </tfoot>
                        </table>
                        {this.props.canSubmit ? <div className="alert alert-info">{infoText}</div> : null}
                        {!this.props.hideError ? <div className="alert alert-danger">{errorText}</div> : null}

                  </div>
              </div>
          </div>
      );
    }

    private _renderSubmitButton = () => {
        if (this.props.placingOrder) {
            return (
                <Button className="btn btn-order" disabled={!this.props.canSubmit} onClick={this.props.onSubmit}>
                    Save & Review
                </Button>
            )
        }

        else
        {
            return (
                <Button className="btn btn-order" onClick={this._switchViews}>
                    Use for order
                </Button>
                )
        }
    }

    private _switchViews = () => {
        this.props.switchViews("order");
    }

    private _renderErrorButton = () => {
      if (this.props.hideError) {
        return null;
      }

      const errorIconStyle = {
          color: "red",
          padding: "0 10px",
      };

      return (
          <a onClick={this._handleErrors}>
          Fix Errors <i className="fa fa-exclamation" style={errorIconStyle} />
        </a>
      );
    }

    private _renderToolTipIcon = (tooltip: string) => {
      return (
        <i
          className="analysisTooltip fa fa-info-circle"
          aria-hidden="true"
          data-toggle="tooltip"
          title={tooltip}
          data-container="table"
        />
      );
    }

    private _renderTests = () => {
        const isUcClient = this.props.clientType === "uc";

        return this.props.selectedTests.map((item) => {
            const price = isUcClient ? item.internalCost : item.externalCost;
            const setupCost = isUcClient ? item.internalSetupCost : item.externalSetupCost;
            const perTest = price * this.props.quantity;
            const rowTotalDisplay = (perTest + setupCost);

            return (
                <tr key={item.id}>
                    <td>{item.analysis}</td>
                    <td><NumberFormat value={price} {...numberFormatOptions} /></td>
                    <td><NumberFormat value={perTest} {...numberFormatOptions} /></td>
                    <td><NumberFormat value={setupCost} {...numberFormatOptions} /></td>
                    <td><NumberFormat value={rowTotalDisplay} {...numberFormatOptions} /></td>
                </tr>
            );
        });
    }

    private _handleErrors = () => {
        this.props.handleErrors();
    }

    private _totalCost = () => {
        if (this.props.quantity < 1) {
            return 0;
        }

        const isUcClient = this.props.clientType === "uc";

        const total = this.props.selectedTests.reduce((prev, item) => {
            const setup = isUcClient ? item.internalSetupCost : item.externalSetupCost;
            const price = isUcClient ? item.internalCost : item.externalCost;
            const perTest = price * this.props.quantity;
            return prev + perTest + setup;
        }, 0);

        return total + this.props.processingFee;
    }
}
