import * as React from "react";
import Checkbox from "react-toolbox/lib/checkbox";
import { ISampleTypeQuestions } from "./SampleTypeQuestions";
import Input from "./ui/input/input";

interface IWaterQuestionsProps {
    waterPreservativeRef: (element: HTMLInputElement) => void;
    handleChange: (key: string, value: any) => void;
    sampleType: string;
    questions: ISampleTypeQuestions;
}

interface IWaterQuestionsState {
    waterPreservativeInfo: string;
    error: string;
}

export class SampleWaterQuestions extends React.Component<IWaterQuestionsProps, IWaterQuestionsState > {

    constructor(props) {
        super(props);

        this.state = {
            waterPreservativeInfo: this.props.questions.waterPreservativeInfo,
            error: null,
        };
    }

    private _changeFilter = () => {
        this.props.handleChange("waterFiltered", !this.props.questions.waterFiltered);
    }

    private _changePreservative = () => {
        this.props.handleChange("waterPreservativeAdded", !this.props.questions.waterPreservativeAdded);
    }

    private _onChangePreservativeText = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value;
        this.setState({ waterPreservativeInfo: value });
        this._validate(value);
    }

    private _onBlurPreservativeText = () => {
        const waterPreservativeInfo = this.state.waterPreservativeInfo;
        this._validate(waterPreservativeInfo);
        this.props.handleChange("waterPreservativeInfo", this.state.waterPreservativeInfo);
    }

    private _changeReporting = () => {
        this.props.handleChange("waterReportedInMgL", !this.props.questions.waterReportedInMgL)
    }

    private _validate = (v: string) => {
        let error = null;
        if (v.trim() == "") {
            error = "This information is required";
        }

        this.setState({ error } as IWaterQuestionsState);
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
                                <Input
                                    inputRef={this.props.waterPreservativeRef}
                                    error={this.state.error}
                                    required={true}
                                    maxLength={256}
                                    value={this.state.waterPreservativeInfo}
                                    onChange={this._onChangePreservativeText}
                                    onBlur={this._onBlurPreservativeText}
                                    label="Provide more information about your preservative"
                                />
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
