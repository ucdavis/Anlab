import * as React from 'react';
import Checkbox from 'react-toolbox/lib/checkbox';
import { Input } from "react-toolbox/lib/input";

interface IWaterQuestionProps {
    sampleType: string;
    handleChange: Function;
}

interface IWaterQuestionState {
    filterWater: boolean;
    preservativeAdded: boolean;
    preservativeText: string;
    reportedInMgL: boolean;
}

export class SampleWaterQuestions extends React.Component<IWaterQuestionProps, IWaterQuestionState> {

    constructor(props) {
        super(props);

        this.state = {
            filterWater: false,
            preservativeAdded: false,
            preservativeText: "",
            reportedInMgL: false,
        };
    }

    private _changeFilter = () => {
        this.setState({ filterWater: !this.state.filterWater });
    }

    private _changePreservative = () => {
        this.setState({ preservativeAdded: !this.state.preservativeAdded });
    }

    private _changePreservativeText = (e) => {
        this.setState({ preservativeText: e.target.value });
    }

    private _changeReporting = () => {
        this.setState({ reportedInMgL: !this.state.reportedInMgL });
    }

    render() {
        if (this.props.sampleType !== "Water") {
            return null;
        }
        return (
            <div>
                <table>
                    <tbody>
                        <tr>
                            <th colSpan={2}>
                               Are these samples filtered?
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <input type="radio" checked={this.state.filterWater} onChange={this._changeFilter} /> Yes
                            </td>
                            <td>
                                <input type="radio" checked={!this.state.filterWater} onChange={this._changeFilter} /> No
                            </td>
                        </tr>
                        <tr>
                            <th colSpan={2}>
                                Was a preservative added to your sample?
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <input type="radio" checked={this.state.preservativeAdded} onChange={this._changePreservative} /> Yes
                            </td>
                            <td>
                                <input type="radio" checked={!this.state.preservativeAdded} onChange={this._changePreservative} /> No
                            </td>
                        </tr>
                        <tr hidden={!this.state.preservativeAdded} >
                            <td colSpan={2}>
                                <input type="text" value ={this.state.preservativeText} onChange={this._changePreservativeText} />
                            </td>
                        </tr>
                        <tr>
                            <th colSpan={2}>
                                 Cl and Soluble Ca, Mg, and Na are reported in meq/L. Do you want them reported in mg/L?
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <input type="radio" checked={this.state.reportedInMgL} onChange={this._changeReporting} /> Yes
                            </td>
                            <td>
                                <input type="radio" checked={!this.state.reportedInMgL} onChange={this._changeReporting} /> No
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        );
    } 
}