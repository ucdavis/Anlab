import * as React from 'react';
import { ITestItem } from './TestList';
import { IPayment } from './PaymentSelection';

interface ISummaryProps {
    testItems: Array<ITestItem>;
    quantity: number;
    payment: IPayment;
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
            return (
                <tr key={item.id}>
                    <td>{item.analysis}</td>
                    <td>{price}</td>
                    <td>{perTest}</td>
                    <td>{item.setupCost}</td>
                    <td>{perTest + item.setupCost}</td>
                </tr>
            );
        });

        return tests;
    }
    render() {
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
                            <td>{this.totalCost()}</td>
                        </tr>
                    </tfoot>
                </table>
            </div>

        );
    }
}