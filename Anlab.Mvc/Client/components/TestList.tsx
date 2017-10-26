import * as React from "react";
import Input from "./ui/input/input";

import NumberFormat from "react-number-format";
import { groupBy } from "../util/arrayHelpers";
import { IPayment } from "./PaymentSelection";

import { Dialog } from "react-toolbox/lib/dialog";
import showdown from "showdown";
import { TestInfo } from "./TestInfo";

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
    sop: string;
}

interface ITestListState {
    query: string;
}

export interface ITestListProps {
    items: ITestItem[];
    additionalInfoList: object;

    clientType: string;
    selectedCodes: object;
    onTestSelectionChanged: (test: ITestItem, selected: boolean) => void;
    updateAdditionalInfo: (key: string, value: string) => void;
}

const showdownConverter = new showdown.Converter();

export class TestList extends React.PureComponent<ITestListProps, ITestListState> {
    constructor(props) {
        super(props);
        this.state = {
            query: "",
        };
    }

    public render() {
        return (
          <div className="form_wrap">
          <h2 className="form_header margin-bottom-zero">Which tests would you like to run?</h2>
            <div>
                <Input
                    label="Search"
                    name="name"
                    value={this.state.query}
                    onChange={this._onQueryChange}
                />
                <table className="table">
                    <thead>
                        <tr>
                            <th>Select</th>
                            <th>Analysis</th>
                            <th>Method Ref</th>
                            <th>Price</th>
                            <th>Notes</th>
                        </tr>
                    </thead>
                    <tbody>
                        {this._renderRows()}
                    </tbody>
                </table>
            </div>
            </div>
        );
    }

    private _renderRows = () => {
        let filteredItems = this.props.items;
        const loweredQuery = this.state.query.toLowerCase();

        if (loweredQuery) {
            filteredItems = this.props.items.filter((item) => {
                return item.analysis.toLowerCase().indexOf(loweredQuery) !== -1
                    || item.id.toLowerCase().indexOf(loweredQuery) !== -1;
            });
        }

        const grouped = groupBy(filteredItems, "group");

        const rows = [];

        Object.keys(grouped).forEach((groupName) => {
            // push the group header
            rows.push((
                <tr key={`group-${groupName}`} className="group-header">
                    <td colSpan={5}>Group {groupName}</td>
                </tr>
            ));

            // now get all tests for that group
            const testRows = grouped[groupName].map((item) => this._renderRow(item));

            // add those tests
            rows.push.apply(rows, testRows);
        });

        return rows;
    }

    private _renderRow = (item) => {
        const selected = !!this.props.selectedCodes[item.id];
        const priceDisplay = (this.props.clientType === "uc" ? item.internalCost : item.externalCost);
        const url = `/analysis/${item.category}/${item.sop}`;
        return (
            <tr key={item.id} >
                <td>
                    <TestInfo
                        test={item}
                        selected={selected}
                        updateAdditionalInfo={this.props.updateAdditionalInfo}
                        value={this.props.additionalInfoList[item.id] as string}
                        onSelection={this.props.onTestSelectionChanged}
                    />
                </td>
                <td>{item.analysis}</td>
                <td>
                    {item.sop === "0" ? "---" : <a href={url} target="_blank" >{item.sop}</a>} </td>
                <td>
                    <NumberFormat
                        value={priceDisplay}
                        displayType={"text"}
                        thousandSeparator={true}
                        decimalPrecision={2}
                        prefix={"$"}
                    />
                </td>
                <td style={{ width: "5%" }}>
                    {this._renderNotes(item.notes)}
                </td>
            </tr>
        );
    }

    private _renderNotes(notes) {
        if (!notes) {
            return null;
        }

        const tooltipContent = showdownConverter.makeHtml(notes);
        return (
            <i
                className="analysisTooltip fa fa-info-circle"
                aria-hidden="true"
                data-toggle="tooltip"
                data-html="true"
                title={tooltipContent}
            />
        );
    }

    private _onQueryChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        this.setState({ query: e.target.value });
    }
}
