import * as React from "react";
import { ISampleTypeQuestions } from "./SampleTypeQuestions";

interface IPlantQuestionProps {
    handleChange: Function;
    sampleType: string;
    questions: ISampleTypeQuestions;
}
export class SamplePlantQuestions extends React.Component<IPlantQuestionProps, {}> {

    private _changeReporting = (e) => {
        this.props.handleChange("plantReportingBasis",e.target.value);
    }

    public render() {
        if (this.props.sampleType !== "Plant") {
            return null;
        }
        const option1 = "Report results on 100% dry weight basis, based on an average of 10% of the samples.";
        const option2 = "Report results on As Received basis.";
        const option3 = "Report results on 100% dry weight basis, based on individual dry matter results (charge applies).";
        return (
            <div className="input-group">
                <p>
                                How would you like your samples reported?</p>
                <p>
                    <label>
                        <input className="videokilledtheradiostar" type="radio" value={option1} checked={this.props.questions.plantReportingBasis == option1} onChange={this._changeReporting} />
                        {option1}
                    </label>
                </p>
                <p>
                    <label>
                        <input className="videokilledtheradiostar" type="radio" value={option2} checked={this.props.questions.plantReportingBasis == option2 } onChange={this._changeReporting} />
                        {option2}
                    </label>
                </p>
                <p>
                    <label>
                        <input className="videokilledtheradiostar" type="radio" value={option3} checked={this.props.questions.plantReportingBasis == option3 } onChange={this._changeReporting} />
                        {option3}
                    </label>
              </p>
            </div>
        );
    }
}
