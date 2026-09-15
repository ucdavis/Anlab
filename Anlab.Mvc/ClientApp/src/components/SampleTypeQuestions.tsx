import * as React from "react";
import { SampleSoilQuestions } from "./SampleSoilQuestions";
import { SampleWaterQuestions } from "./SampleWaterQuestions";
import { SamplePlantQuestions } from "./SamplePlantQuestions";
import { ITestItem } from "./TestList";

export interface ISampleTypeQuestions {
  soilImported: boolean;
  soilAgreement: string;
  plantReportingBasis: string;
  waterFiltered: boolean;
  waterFilterInfo: string;
  waterPreservativeAdded: boolean;
  waterPreservativeInfo: string;
  waterReportedInMgL: boolean;
  dryMatterTests: ITestItem[];
  isDryMatterTestSelected: boolean;
}
interface ISampleTypeQuestionsProps {
  waterFilterRef: (element: HTMLInputElement) => void;
  waterPreservativeRef: (element: HTMLInputElement) => void;
  sampleType: string;
  questions: ISampleTypeQuestions;
  handleChange: (key: string, value: any) => void;
  plantReportingRef: (element: HTMLInputElement) => void;
  dryMatterTests: ITestItem[];
  isDryMatterTestSelected: boolean;
}

export class SampleTypeQuestions extends React.Component<
  ISampleTypeQuestionsProps,
  {}
> {
  render() {
    return (
      <div>
        {this.props.sampleType === "Miscellaneous" && (
          <div className="alert alert-warning" role="alert">
            <h4>Miscellaneous &amp; Specialty Testing</h4>
            <div>
              Please use this category for all other samples that do not fit within our standard
              categories.
            </div>
            <div>
              <strong>Examples:</strong> seawater, manure, compost, etc.
            </div>
            <hr />
            <div>
              For Miscellaneous samples, if the following questions do not
              apply, leave them as <strong>No</strong>.
            </div>
          </div>
        )}
        <SampleSoilQuestions
          sampleType={this.props.sampleType}
          questions={this.props.questions}
          handleChange={this.props.handleChange}
        />
        <SampleWaterQuestions
          waterFilterRef={this.props.waterFilterRef}
          waterPreservativeRef={this.props.waterPreservativeRef}
          sampleType={this.props.sampleType}
          questions={this.props.questions}
          handleChange={this.props.handleChange}
        />
        <SamplePlantQuestions
          plantReportingRef={this.props.plantReportingRef}
          sampleType={this.props.sampleType}
          questions={this.props.questions}
          handleChange={this.props.handleChange}
          dryMatterTests={this.props.questions.dryMatterTests}
          isDryMatterTestSelected={this.props.questions.isDryMatterTestSelected}
        />
      </div>
    );
  }
}
