import * as React from 'react';
import { SampleSoilQuestions } from './SampleSoilQuestions';
import { SampleWaterQuestions } from './SampleWaterQuestions';
import { SamplePlantQuestions } from "./SamplePlantQuestions";

export interface ISampleTypeQuestions {
    soilImported: boolean;
    plantReportingBasis: string;
    waterFiltered: boolean;
    waterPreservativeAdded: boolean;
    waterPreservativeInfo: string;
    waterReportedInMgL: boolean;
}
interface ISampleTypeQuestionsProps {
    sampleType: string;
    questions: ISampleTypeQuestions;
    handleChange: Function;
}

export class SampleTypeQuestions extends React.Component<ISampleTypeQuestionsProps, {}> {

    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div>
                <SampleSoilQuestions sampleType={this.props.sampleType} questions={this.props.questions} handleChange={this.props.handleChange} />
                <SampleWaterQuestions sampleType={this.props.sampleType} questions={this.props.questions} handleChange={this.props.handleChange} />
                <SamplePlantQuestions sampleType={this.props.sampleType} questions={this.props.questions} handleChange={this.props.handleChange} />
            </div>

        );
    }
}
