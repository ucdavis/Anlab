import * as React from "react";
import { ISampleTypeQuestions } from "./SampleTypeQuestions";

interface IPlantQuestionProps {
  handleChange: Function;
  sampleType: string;
  questions: ISampleTypeQuestions;
}

export const SamplePlantQuestionsOptions = {
    average: "Report results on 100% dry weight basis, based on an average of 10% of the samples.",
    asReceived: "Report results on As Received basis.",
    individual: "Report results on 100% dry weight basis, based on individual dry matter results (charge applies)."
}

export class SamplePlantQuestions extends React.Component<IPlantQuestionProps, {}> {
  public render() {
    if (this.props.sampleType !== "Plant") {
      return null;
    }
    return (
      <div className="input-group">
        <label className="form_header margin-bottom-zero">
          How would you like your samples reported?
        </label>
        <p>
          <label>
            <input
              className="videokilledtheradiostar"
              type="radio"
              value={SamplePlantQuestionsOptions.average}
              checked={this.props.questions.plantReportingBasis == SamplePlantQuestionsOptions.average}
              onChange={this._changeReporting}
            />
            {SamplePlantQuestionsOptions.average}
          </label>
        </p>
        <p>
          <label>
            <input
              className="videokilledtheradiostar"
              type="radio"
              value={SamplePlantQuestionsOptions.asReceived}
              checked={this.props.questions.plantReportingBasis == SamplePlantQuestionsOptions.asReceived}
              onChange={this._changeReporting}
            />
            {SamplePlantQuestionsOptions.asReceived}
          </label>
        </p>
        <p>
          <label>
            <input
              className="videokilledtheradiostar"
              type="radio"
              value={SamplePlantQuestionsOptions.individual}
              checked={this.props.questions.plantReportingBasis == SamplePlantQuestionsOptions.individual}
              onChange={this._changeReporting}
            />
            {SamplePlantQuestionsOptions.individual}
          </label>
          {this.props.questions.plantReportingBasis === SamplePlantQuestionsOptions.individual &&
            (<h2 className="help-block">
               Please select the dry matter test below
            </h2>)
          }
        </p>
      </div>
    );
  }

  private _changeReporting = (e) => {
    this.props.handleChange("plantReportingBasis", e.target.value);
  }

}
