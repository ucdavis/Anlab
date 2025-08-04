import * as React from "react";
import { ISampleTypeQuestions } from "./SampleTypeQuestions";
import { ITestItem } from "./TestList";

interface IPlantQuestionProps {
  handleChange: Function;
  sampleType: string;
  questions: ISampleTypeQuestions;
  dryMatterTests: ITestItem[];
  isDryMatterTestSelected: boolean;
  plantReportingRef: (element: HTMLInputElement) => void;
}

export const SamplePlantQuestionsOptions = {
  average:
    "Report results on AVERAGE 100% dry weight basis, based on an average of 10% of the samples.",
  asReceived: "Report results on As Received basis.",
  individual:
    "Report results on 100% dry weight basis, based on individual dry matter results (Charges Apply).",
};

export class SamplePlantQuestions extends React.Component<
  IPlantQuestionProps,
  {}
> {
  public render() {
    if (this.props.sampleType !== "Plant") {
      return null;
    }
    return (
      <div className="input-group flex flex-col">
        <label className="form_header margin-bottom-zero">
          How would you like your samples reported?
        </label>
        <p>
          <label>
            <input
              className="videokilledtheradiostar"
              type="radio"
              value={SamplePlantQuestionsOptions.average}
              checked={
                this.props.questions.plantReportingBasis ===
                SamplePlantQuestionsOptions.average
              }
              onChange={this._changeReporting}
              ref={this.props.plantReportingRef}
            />
            {SamplePlantQuestionsOptions.average}
          </label>
          {this.props.questions.plantReportingBasis ===
            SamplePlantQuestionsOptions.average &&
            this.props.isDryMatterTestSelected && (
              <span className="red-text help-block">
                <span className="red-text">
                  You have selected Dry Matter. This reporting basis is not
                  available. Please choose reporting basis of "As Received" or
                  "100% Dry Weight Basis, on individual dry matter."
                </span>
              </span>
            )}
        </p>
        <p>
          <label>
            <input
              className="videokilledtheradiostar"
              type="radio"
              value={SamplePlantQuestionsOptions.asReceived}
              checked={
                this.props.questions.plantReportingBasis ===
                SamplePlantQuestionsOptions.asReceived
              }
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
              checked={
                this.props.questions.plantReportingBasis ===
                SamplePlantQuestionsOptions.individual
              }
              onChange={this._changeReporting}
            />
            {SamplePlantQuestionsOptions.individual}
          </label>
          {this.props.questions.plantReportingBasis ===
            SamplePlantQuestionsOptions.individual &&
            this.props.isDryMatterTestSelected && (
              <div>
                <h2 className="help-block inline-block">
                  Dry matter test selected.
                </h2>
                <span className="red-text inline-block">(Charges Apply)</span>
              </div>
            )}
          {this.props.questions.plantReportingBasis ===
            SamplePlantQuestionsOptions.individual &&
            !this.props.isDryMatterTestSelected && (
              <span className="red-text help-block">
                You must select Dry Matter test or a group that contains Dry
                Matter from below to select this option. (Charges Apply)
              </span>
            )}
        </p>
        {!this.props.questions.plantReportingBasis && (
          <span className="red-text help-block">
            You must select how you would like your samples reported.
          </span>
        )}
        {((this.props.questions.plantReportingBasis ===
          SamplePlantQuestionsOptions.average &&
          this.props.isDryMatterTestSelected) ||
          (this.props.questions.plantReportingBasis ===
            SamplePlantQuestionsOptions.individual &&
            !this.props.isDryMatterTestSelected)) && (
          <div>
            Dry Matter and Groups that contain DM are:
            {this.props.dryMatterTests.length > 0 && (
              <ul className="red-text">
                {this.props.dryMatterTests.map((test, index) => (
                  <li key={index}>{test.analysis}</li>
                ))}
              </ul>
            )}
          </div>
        )}
      </div>
    );
  }

  private _changeReporting = (e) => {
    this.props.handleChange("plantReportingBasis", e.target.value);
  };
}
