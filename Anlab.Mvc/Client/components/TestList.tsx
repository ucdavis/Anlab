import * as React from 'react';

import Input from 'react-toolbox/lib/input';
import { Table, TableHead, TableRow, TableCell } from 'react-toolbox/lib/table';

import { IPayment } from './PaymentSelection';
import NumberFormat from 'react-number-format';

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
    selectedTests: Array<ITestItem>;
    onTestSelectionChanged: Function;
};

export class TestList extends React.Component<ITestListProps, ITestListState> {
    state = { query: '' };

    onSelection = (selectedIndexes: Array<number>) => {
        console.log(selectedIndexes);
        const selected = selectedIndexes.map(i => this.props.items[i]);
        
        this.props.onTestSelectionChanged(selected);
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
            const selected = this.props.selectedTests.indexOf(item) !== -1;
            const priceDisplay = (this.props.payment.clientType === 'uc' ? item.internalCost : item.externalCost);
            return (
                <TableRow key={item.id} selected={selected}>
                    <TableCell>{item.analysis}</TableCell>
                    <TableCell>{item.code}</TableCell>
                    <TableCell numeric><NumberFormat value={priceDisplay} displayType={'text'} thousandSeparator={true} decimalPrecision={true} prefix={'$'} /></TableCell>
                </TableRow>
            );
        });
    };
    render() {
        return (
            <div>
                <Input type='search' label='Search' name='name' value={this.state.query} onChange={this.onQueryChange} />
                <Table multiSelectable onRowSelect={this.onSelection} style={{ marginTop: 10 }}>
                    <TableHead>
                        <TableCell>Analysis</TableCell>
                        <TableCell>Code</TableCell>
                        <TableCell numeric>Price</TableCell>
                    </TableHead>
                    {this.renderRows()}
                </Table>
            </div>
        );
    }
}