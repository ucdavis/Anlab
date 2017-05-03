import * as React from 'react';
import { IPayment } from './PaymentSelection';

export interface ITestItem {
    id: number;
    analysis: string;
    code: string;
    internalCost: number;
    externalCost: number;
}

export interface ITestListProps {
    items: Array<ITestItem>,
    payment: IPayment,
};

export class TestList extends React.Component<ITestListProps, any> {
    state = { selected: [] };

    renderRows = () => {
        return this.props.items.map(item => {
            const price = this.props.payment.clientType === 'uc' ? item.internalCost : item.externalCost;
            return (
                <tr key={item.id}>
                    <td>{item.analysis}</td>
                    <td>{item.code}</td>
                    <td>{price}</td>
                </tr>
            );
        });
    };

    render() {
        return (
            <table className="table">
                <thead>
                    <tr>
                        <th>Analysis</th>
                        <th>Col2</th>
                        <th>Col3</th>
                    </tr>
                </thead>
                <tbody>
                    {this.renderRows()}
                </tbody>
            </table>
        );
    }
}