import * as React from "react";
import { SampleSoilQuestions } from "./SampleSoilQuestions";
import { SampleWaterQuestions } from "./SampleWaterQuestions";
import { SamplePlantQuestions } from "./SamplePlantQuestions";
import { ITestItem } from "./TestList";

export interface ISampleTypeQuestions {
  soilImported: boolean;
  plantReportingBasis: string;
  waterFiltered: boolean;
  waterPreservativeAdded: boolean;
  waterPreservativeInfo: string;
  waterReportedInMgL: boolean;
  dryMatterTests: ITestItem[];
  isDryMatterTestSelected: boolean;
}
interface ISampleTypeQuestionsProps {
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
        <SampleSoilQuestions
          sampleType={this.props.sampleType}
          questions={this.props.questions}
          handleChange={this.props.handleChange}
        />
        <SampleWaterQuestions
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
