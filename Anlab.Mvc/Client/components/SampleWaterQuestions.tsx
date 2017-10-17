import * as React from 'react';
import Checkbox from 'react-toolbox/lib/checkbox';
import { ISampleTypeQuestions } from "./SampleTypeQuestions";

interface IWaterQuestionsProps {
    handleChange: Function;
    sampleType: string;
    questions: ISampleTypeQuestions;
}

interface IWaterQuestionsState {
    waterPreservativeInfo: string;
}

export class SampleWaterQuestions extends React.Component<IWaterQuestionsProps, IWaterQuestionsState > {

    constructor(props) {
        super(props);

        this.state = {
            waterPreservativeInfo: "",
        };
    }

    private _changeFilter = () => {
        this.props.handleChange("waterFiltered", !this.props.questions.waterFiltered);
    }

    private _changePreservative = () => {
        this.props.handleChange("waterPreservativeAdded", !this.props.questions.waterPreservativeAdded);
    }

    private _onChangePreservativeText = (e) => {
        this.setState({ waterPreservativeInfo: e.target.value });
    }

    private _onBlurPreservativeText = () => {
        this.props.handleChange("waterPreservativeInfo", this.state.waterPreservativeInfo);
    }

    private _changeReporting = () => {
        this.props.handleChange("waterReportedInMgL", !this.props.questions.waterReportedInMgL)
    }

    public render() {
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
                                <input type="radio" checked={this.props.questions.waterFiltered} onChange={this._changeFilter} /> Yes
                            </td>
                            <td>
                                <input type="radio" checked={!this.props.questions.waterFiltered} onChange={this._changeFilter} /> No
                            </td>
                        </tr>
                        <tr>
                            <th colSpan={2}>
                                Was a preservative added to your sample?
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <input type="radio" checked={this.props.questions.waterPreservativeAdded} onChange={this._changePreservative} /> Yes
                            </td>
                            <td>
                                <input type="radio" checked={!this.props.questions.waterPreservativeAdded} onChange={this._changePreservative} /> No
                            </td>
                        </tr>
                        {this.props.questions.waterPreservativeAdded &&
                            <tr>
                            <td colSpan={2}>
                                <input type="text" value={this.state.waterPreservativeInfo} onChange={this._onChangePreservativeText} onBlur={this._onBlurPreservativeText} />
                                </td>
                            </tr>
                        }
                        <tr>
                            <th colSpan={2}>
                                 Cl and Soluble Ca, Mg, and Na are reported in meq/L. Do you want them reported in mg/L?
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <input type="radio" checked={this.props.questions.waterReportedInMgL} onChange={this._changeReporting} /> Yes
                            </td>
                            <td>
                                <input type="radio" checked={!this.props.questions.waterReportedInMgL} onChange={this._changeReporting} /> No
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        );
    } 
}