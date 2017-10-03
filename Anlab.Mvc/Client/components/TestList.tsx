import * as React from 'react';
import Input from 'react-toolbox/lib/input';
import Checkbox from 'react-toolbox/lib/checkbox';

import { IPayment } from './PaymentSelection';
import NumberFormat from 'react-number-format';
import { groupBy } from '../util/arrayHelpers';

import showdown from 'showdown';
import { Dialog } from "react-toolbox/lib/dialog";

export interface ITestItem {
    id: string;
    analysis: string;
    internalCost: number;
    externalCost: number;
    internalSetupCost: number;
    externalSetupCost: number;
    category: string;
    categories: string[];
    notes: string;
    additionalInfoPrompt?: string;
}

interface ITestListState {
    query: string;
    active: boolean;
}

export interface ITestListProps {
    items: Array<ITestItem>;
    payment: IPayment;
    selectedTests: any;
    onTestSelectionChanged: Function;
};

export class TestList extends React.Component<ITestListProps, ITestListState> {
    state = {
        query: '',
        active: false
    };

    onSelection = (test: ITestItem, e) => {
        const selected = e;

        if (selected && test.additionalInfoPrompt)
        {
            this.toggleModal();
        }

        this.props.onTestSelectionChanged(test, selected);
    }

    toggleModal = () => {
        this.setState({ ...this.state, active: !this.state.active });
    }

    actions = [
        { label: "Cancel", onClick: this.toggleModal },
        { label: "Save", onClick: this.toggleModal }
    ];

    onQueryChange = (value: string) => {
        this.setState({ ...this.state, query: value });
    }

    renderRows = () => {
        let filteredItems = this.props.items;
        const loweredQuery = this.state.query.toLowerCase();

        if (loweredQuery) {
            filteredItems = this.props.items.filter(item => {
                return item.analysis.toLowerCase().indexOf(loweredQuery) !== -1 || item.id.toLowerCase().indexOf(loweredQuery) !== -1;
            });
        }

        const grouped = groupBy(filteredItems, 'group');

        const rows = [];

        var converter = new showdown.Converter();

        Object.keys(grouped).map(groupName => {
            // push the group header
            rows.push(<tr key={`group-${groupName}`} className="group-header"><td colSpan={5}>Group {groupName}</td></tr>);

            // now get all tests for that group
            const testRows = grouped[groupName].map(item => {
                const selected = !!this.props.selectedTests[item.id];
                const priceDisplay = (this.props.payment.clientType === 'uc' ? item.internalCost : item.externalCost);
                const tooltipContent = converter.makeHtml(item.notes);
                return (
                    <tr key={item.id} >
                        <td>
                            <Checkbox checked={selected} onChange={e => this.onSelection(item, e)} />
                        </td>
                        <td>{item.analysis}</td>
                        <td>{item.id}</td>
                        <td><NumberFormat value={priceDisplay} displayType={'text'} thousandSeparator={true} decimalPrecision={2} prefix={'$'} /></td>
                        <td style={{ width: '5%' }}>
                            {item.notes ? <i className="analysisTooltip fa fa-info-circle" aria-hidden="true" data-toggle="tooltip" data-html="true" title={tooltipContent}></i> : ""}
                        </td>
                        <td hidden>
                            {item.additionalInfoPrompt &&
                                <Dialog
                                    actions={this.actions}
                                    active={this.state.active}
                                    title={item.additionalInfoPrompt}
                                >
                                <p>Testing stuff</p>
                                </Dialog>
                            }
                        </td>
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
                            <th>Notes</th>
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
