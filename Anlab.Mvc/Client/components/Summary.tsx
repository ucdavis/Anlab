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
    grind: boolean;
    foreignSoil: boolean;
    filterWater: boolean;
}

export class Summary extends React.Component<ISummaryProps, any> {
    totalCost = () => {
        const total = this.props.testItems.reduce((prev, item) => {
            // total for current item
            const price = this.props.payment.clientType === 'uc' ? item.internalCost : item.externalCost;
            const perTest = price * this.props.quantity;
            const grindTotal = this.grindCost() * this.props.quantity;
            const foreignSoilTotal = this.foreignSoilCost() * this.props.quantity;
            const filterWaterTotal = this.waterFilterCost() * this.props.quantity;
            return prev + perTest + item.setupCost + grindTotal + foreignSoilTotal + filterWaterTotal;
        }, 0);

        return total;
    }

    grindCost = () => {
        if (this.props.grind) {
            return this.props.payment.clientType === 'uc' ? 6 : 9;
        } else {
            return 0;
        }
    }

    foreignSoilCost = () => {
        if (this.props.foreignSoil) {
            return this.props.payment.clientType === 'uc' ? 9 : 14;
        } else {
            return 0;
        }
    }
    waterFilterCost = () => {
        if (this.props.filterWater) {
            return this.props.payment.clientType === 'uc' ? 11 : 17;
        } else {
            return 0;
        }
    }

    _renderTests = () => {

        const tests = this.props.testItems.map(item => {
            const price = this.props.payment.clientType === 'uc' ? item.internalCost : item.externalCost;
            const perTest = price * this.props.quantity;            
            const rowTotalDisplay = (perTest + item.setupCost);
            return (
                <tr key={item.id}>
                    <td>{item.analysis}</td>
                    <td><NumberFormat value={price} displayType={'text'} thousandSeparator={true} decimalPrecision={true} prefix={'$'} /></td>
                    <td><NumberFormat value={perTest} displayType={'text'} thousandSeparator={true} decimalPrecision={true} prefix={'$'} /></td>
                    <td><NumberFormat value={item.setupCost} displayType={'text'} thousandSeparator={true} decimalPrecision={true} prefix={'$'} /></td>
                    <td><NumberFormat value={rowTotalDisplay} displayType={'text'} thousandSeparator={true} decimalPrecision={true} prefix={'$'} /></td>
                </tr>
            );
        });

        return tests;
    }

    _renderAdditionalFees = () => {
        if (!(this.props.grind || this.props.foreignSoil || this.props.filterWater)) {
            return null;
        }


        return (
            <table className="table">
                <thead>
                <tr>
                    <th>Fee Type</th>
                    <th>Fee</th>
                    <th>Total</th>
                </tr>
                </thead>
                <tbody>
                    {this._renderGrindFee()}
                    {this._renderForeignSoilFee()}
                    {this._renderFilterWaterFee()}
                </tbody>
            </table>
        );
    }

    _renderGrindFee = () => {
        if (!this.props.grind) {
            return null;
        }
        const Price = this.grindCost();
        const Total = Price * this.props.quantity;

        return (
            <tr>
                <td>Grind</td>
                <td><NumberFormat value={Price} displayType={'text'} thousandSeparator={true} decimalPrecision={true} prefix={'$'} /></td>
                <td><NumberFormat value={Total} displayType={'text'} thousandSeparator={true} decimalPrecision={true} prefix={'$'} /></td>
            </tr>
            );
    }

    _renderForeignSoilFee = () => {
        if (!this.props.foreignSoil) {
            return null;
        }
        const Price = this.foreignSoilCost();
        const Total = Price * this.props.quantity;

        return (
            <tr>
                <td>Foreign Soil</td>
                <td><NumberFormat value={Price} displayType={'text'} thousandSeparator={true} decimalPrecision={true} prefix={'$'} /></td>
                <td><NumberFormat value={Total} displayType={'text'} thousandSeparator={true} decimalPrecision={true} prefix={'$'} /></td>
            </tr>
        );
    }

    _renderFilterWaterFee = () => {
        if (!this.props.filterWater) {
            return null;
        }
        const Price = this.waterFilterCost();
        const Total = Price * this.props.quantity;

        return (
            <tr>
                <td>Filter</td>
                <td><NumberFormat value={Price} displayType={'text'} thousandSeparator={true} decimalPrecision={true} prefix={'$'} /></td>
                <td><NumberFormat value={Total} displayType={'text'} thousandSeparator={true} decimalPrecision={true} prefix={'$'} /></td>
            </tr>
        );
    }

    render() {
        if (this.props.testItems.length === 0) {
            return null;
        }
        const saveText = this.props.isCreate ? "Place Order" : "Update Order";
        const infoText = this.props.isCreate ? "Go ahead and place your order" : "Go ahead and update your order";
        return (
            <div>
                {this._renderAdditionalFees()}
                <table className="table">
                    <thead>
                        <tr>
                            <th>Analysis</th>
                            <th>Fee</th>
                            <th>Price</th>
                            <th>Setup</th>
                            <th>Total</th>
                        </tr>
                    </thead>
                    <tbody>
                        {this._renderTests()}
                    </tbody>
                    <tfoot>
                        <tr>
                            <td colSpan={4}></td>
                            <td><NumberFormat value={this.totalCost()} displayType={'text'} thousandSeparator={true} decimalPrecision={true} prefix={'$'} /></td>
                        </tr>
                    </tfoot>
                </table>
                {this.props.canSubmit ? <div className="alert alert-info">{infoText}</div> : null }
                <Button raised primary disabled={!this.props.canSubmit} onClick={this.props.onSubmit}>
                    {saveText}
                </Button>
            </div>

        );
    }
}