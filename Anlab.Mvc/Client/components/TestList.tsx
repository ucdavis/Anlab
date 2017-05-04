import * as React from 'react';
import { IPayment } from './PaymentSelection';

export interface ITestItem {
    id: number;
    analysis: string;
    code: string;
    internalCost: number;
    externalCost: number;
    category: string;
}

export interface ITestListProps {
    items: Array<ITestItem>,
    payment: IPayment,
    sampleType: string,
};

export class TestList extends React.Component<ITestListProps, any> {
    state = { selected: [] };

    renderRows = () => {
        var sample = this.props.sampleType;
        var filtered = this.props.items.filter(i => i.category === sample);
        return filtered.map(item => {
            const price = this.props.payment.clientType === 'uc' ? item.internalCost : item.externalCost;
            return (
                <tr key={item.id}>
                    <td>{item.analysis}</td>
                    <td>{item.code}</td>
                    <td>{price}</td>
                    <td>{item.category}</td>
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
                        <th>Category</th>
                    </tr>
                </thead>
                <tbody>
                    {this.renderRows()}
                </tbody>
            </table>
        );
    }
}