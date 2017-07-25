import * as React from 'react';
import Input from 'react-toolbox/lib/input';
import Checkbox from 'react-toolbox/lib/checkbox';

import { IPayment } from './PaymentSelection';
import NumberFormat from 'react-number-format';
import { groupBy } from '../util/arrayHelpers';

export interface ITestItem {
    id: number;
    analysis: string;
    code: string;
    internalCost: number;
    externalCost: number;
    internalSetupCost: number;
    externalSetupCost: number;
    category: string;
    notes: string;
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
        const selected = e;

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

        const grouped = groupBy(filteredItems, 'group');

        const rows = [];

        Object.keys(grouped).map(groupName => {
            // push the group header
            rows.push(<tr key={`group-${groupName}`} className="group-header"><td colSpan={4}>Group {groupName}</td></tr>);

            // now get all tests for that group
            const testRows = grouped[groupName].map(item => {
                const selected = !!this.props.selectedTests[item.id];
                const priceDisplay = (this.props.payment.clientType === 'uc' ? item.internalCost : item.externalCost);
                return (
                    <tr key={item.id} >
                        <td>
                            <Checkbox checked={selected} onChange={e => this.onSelection(item, e)} />
                        </td>
                        <td><div className="analysisTooltip" data-toggle="tooltip" title={item.notes}>{item.analysis}</div></td>
                        <td>{item.code}</td>
                        <td><NumberFormat value={priceDisplay} displayType={'text'} thousandSeparator={true} decimalPrecision={true} prefix={'$'} /></td>
                    </tr>
                );
            });

            // add those tests
            rows.push.apply(rows, testRows);
        })

        return rows;
    };
    render() {
        return (
          <div className="form_wrap">
          <h2 className="form_header margin-bottom-zero">Which tests would you like to run?</h2>
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
            </div>
        );
    }
}
