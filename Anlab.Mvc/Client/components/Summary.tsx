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
}

export class Summary extends React.Component<ISummaryProps, any> {
    totalCost = () => {
        const total = this.props.testItems.reduce((prev, item) => {
            // total for current item
            const price = this.props.payment.clientType === 'uc' ? item.internalCost : item.externalCost;
            const perTest = price * this.props.quantity;
            return prev + perTest + item.setupCost;
        }, 0);

        return total;
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
    render() {
        if (this.props.testItems.length === 0) {
            return null;
        }
        return (
            <div>
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
                <div className="alert alert-info">Go ahead and place your order</div>
                <Button raised primary onClick={this.props.onSubmit}>
                    Place Order
                </Button>
            </div>

        );
    }
}