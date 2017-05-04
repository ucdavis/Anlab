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
    items: Array<ITestItem>;
    payment: IPayment;
    selectedTests: any;
    onTestSelectionChanged: Function;
};

export class TestList extends React.Component<ITestListProps, any> {
    state = { selected: [] };

    onSelection = (test: ITestItem, e) => {
        const selected = e.target.checked;

        this.props.onTestSelectionChanged(test, selected);  
    }

    renderRows = () => {
        return this.props.items.map(item => {
            const selected = !!this.props.selectedTests[item.id];
            const price = this.props.payment.clientType === 'uc' ? item.internalCost : item.externalCost;
            return (
                <tr key={item.id}>
                    <td>
                        <input type="checkbox" checked={selected} onChange={e => this.onSelection(item, e)} />
                    </td>
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
                        <th>Select</th>
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