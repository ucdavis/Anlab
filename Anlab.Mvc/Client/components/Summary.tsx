import * as React from 'react';
import { ITestItem } from './TestList';
import { IPayment } from './PaymentSelection';
import NumberFormat from 'react-number-format';
import {Button} from 'react-toolbox/lib/button';

interface ISummaryProps {
    testItems: Array<ITestItem>;
    quantity: number;
    payment: IPayment;
    onSubmit: Function;
    canSubmit: boolean;
    isCreate: boolean;
    hideError: boolean;
    status: string;
    processingFee: number;
    project: string;
    focusInput: Function;
    quantityRef: any;
    projectRef: any;
    accountRef: any;
}

export class Summary extends React.Component<ISummaryProps, any> {

    totalCost = () => {

        const total = this.props.testItems.reduce((prev, item) => {
            // total for current item
            const price = this.props.payment.clientType === 'uc' ? item.internalCost : item.externalCost;
            const perTest = price * this.props.quantity;

            return prev + perTest + (this.props.payment.clientType === 'uc' ? item.internalSetupCost : item.externalSetupCost);
        }, 0);

        return total + this.props.processingFee;
    }


    _renderTests = () => {

        const tests = this.props.testItems.map(item => {
            const price = this.props.payment.clientType === 'uc' ? item.internalCost : item.externalCost;
            const setupCost = this.props.payment.clientType === 'uc' ? item.internalSetupCost : item.externalSetupCost;
            const perTest = price * this.props.quantity;            
            const rowTotalDisplay = (perTest + setupCost);
            return (
                <tr key={item.id}>
                    <td>{item.analysis}</td>
                    <td><NumberFormat value={price} displayType={'text'} thousandSeparator={true} decimalPrecision={true} prefix={'$'} /></td>
                    <td><NumberFormat value={perTest} displayType={'text'} thousandSeparator={true} decimalPrecision={true} prefix={'$'} /></td>
                    <td><NumberFormat value={setupCost} displayType={'text'} thousandSeparator={true} decimalPrecision={true} prefix={'$'} /></td>
                    <td><NumberFormat value={rowTotalDisplay} displayType={'text'} thousandSeparator={true} decimalPrecision={true} prefix={'$'} /></td>
                </tr>
            );
        });

        return tests;
    }

    handleErrors = () => {
        if (!this.props.hideError)
        {
            if (this.props.payment.clientType === 'uc' && this.props.payment.account === '')
            {
                this.props.focusInput(this.props.accountRef);
            }
            else if (this.props.quantity < 1)
            {
                this.props.focusInput(this.props.quantityRef);
            }
            else if (!this.props.project)
            {
                this.props.focusInput(this.props.projectRef);
            }
        }
    }

    render() {
        if (this.props.testItems.length === 0) {
            return null;
        }
        let errorIconStyle = {
            color: 'red',
            padding: '0 10px'
        }
        let saveText = this.props.isCreate ? "Place Order" : "Update Order";
        let infoText = this.props.isCreate ? "Go ahead and place your order" : "Go ahead and update your order";        
        const errorText = "Please correct any errors and complete any required fields before you " + saveText.toLowerCase();
        return (
            <div>
                <div className="ordersum">
                    <div>
                        <h3>Order Total:<NumberFormat value={this.totalCost()} displayType={'text'} thousandSeparator={true} decimalPrecision={true} prefix={'$'} /></h3>
                        <a role="button" data-toggle="collapse" data-target="#collapseExample" aria-expanded="false" aria-controls="collapseExample">
                            Order Details
                        </a>
                    </div>
                    <div>
                        {!this.props.hideError &&
                            <a onClick={this.handleErrors}>Fix Errors<i className="fa fa-exclamation" style={errorIconStyle} ></i></a>}
                        <Button className="btn btn-order" disabled={!this.props.canSubmit} onClick={this.props.onSubmit} >{saveText}</Button>
                    </div>
                </div>

                <div className="collapse" id="collapseExample">
                    <div className="well">

                            <table className="table">
                                <thead>
                                    <tr>
                                        <th>Analysis</th>
                                        <th>Fee <i className="analysisTooltip fa fa-info-circle" aria-hidden="true" data-toggle="tooltip" title="Price per sample" data-container="table"></i></th>
                                        <th>Price <i className="analysisTooltip fa fa-info-circle" aria-hidden="true" data-toggle="tooltip" title="Fee*Quantity" data-container="table"></i></th>
                                        <th>Setup</th>
                                        <th>Total <i className="analysisTooltip fa fa-info-circle" aria-hidden="true" data-toggle="tooltip" title="Price+Setup" data-container="table"></i></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {this._renderTests()}
                                </tbody>
                                <tfoot>
                                    <tr>
                                        <th>Processing Fee</th>
                                        <td colSpan={3}></td>
                                        <td><NumberFormat value={this.props.processingFee} displayType={'text'} thousandSeparator={true} decimalPrecision={true} prefix={'$'} /></td>
                                    </tr>
                                    <tr>
                                        <td colSpan={4}></td>
                                        <td><NumberFormat value={this.totalCost()} displayType={'text'} thousandSeparator={true} decimalPrecision={true} prefix={'$'} /></td>
                                    </tr>
                                </tfoot>
                            </table>
                            {this.props.canSubmit ? <div className="alert alert-info">{infoText}</div> : null}
                            {!this.props.hideError ? <div className="alert alert-danger">{errorText}</div> : null}
                            <Button raised primary disabled={!this.props.canSubmit} onClick={this.props.onSubmit}>
                                {saveText}
                            </Button>
                    </div>
                </div>
                </div>

        );
    }
}