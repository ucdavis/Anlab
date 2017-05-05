import * as React from 'react';

import Input from 'react-toolbox/lib/input';

import { IPayment } from './PaymentSelection';

export interface ITestItem {
    id: number;
    analysis: string;
    code: string;
    internalCost: number;
    externalCost: number;
    setupCost: number;
    category: string;
}

interface ITestListState {
    query: string;
}

export interface ITestListProps {
    items: Array<ITestItem>;
    payment: IPayment;
    selectedTests: any;
    onTestSelectionChanged: Function;
};

export class TestList extends React.Component<ITestListProps, ITestListState> {
    state = { query: '' };

    onSelection = (test: ITestItem, e) => {
        const selected = e.target.checked;

        this.props.onTestSelectionChanged(test, selected);
    }

    onQueryChange = (value: string) => {
        this.setState({ ...this.state, query: value });
    }

    renderRows = () => {
        let filteredItems = this.props.items;
        const loweredQuery = this.state.query.toLowerCase();

        if (loweredQuery) {
            filteredItems = this.props.items.filter(item => {
                return item.analysis.toLowerCase().indexOf(loweredQuery) !== -1 || item.code.toLowerCase().indexOf(loweredQuery) !== -1;
            });
        }

        return filteredItems.map(item => {
            const selected = !!this.props.selectedTests[item.id];
            const priceDisplay = (this.props.payment.clientType === 'uc' ? item.internalCost : item.externalCost).toFixed(2);
            return (
                <tr key={item.id}>
                    <td>
                        <input type="checkbox" checked={selected} onChange={e => this.onSelection(item, e)} />
                    </td>
                    <td>{item.analysis}</td>
                    <td>{item.code}</td>
                    <td>{priceDisplay}</td>
                </tr>
            );
        });
    };
    render() {
        return (
            <div>
                <Input type='search' label='Search' name='name' value={this.state.query} onChange={this.onQueryChange} />
                <table className="table">
                    <thead>
                        <tr>
                            <th>Select</th>
                            <th>Analysis</th>
                            <th>Code</th>
                            <th>Price</th>
                        </tr>
                    </thead>
                    <tbody>
                        {this.renderRows()}
                    </tbody>
                </table>
            </div>
        );
    }
}