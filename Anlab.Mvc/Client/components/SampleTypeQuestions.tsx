import * as React from 'react';
import { ForeignSoil } from './ForeignSoil';
import { SampleWaterQuestions } from './SampleWaterQuestions';
import { SamplePlantQuestions } from "./SamplePlantQuestions";
import Input from 'react-toolbox/lib/input';
import 'isomorphic-fetch';

interface ISampleTypeQuestionsProps {
    sampleType: string;
    handleChange: Function;
}

interface ISampleTypeQuestionsState {
    soilIsImported: boolean;
}

export class SampleTypeQuestions extends React.Component<ISampleTypeQuestionsProps, ISampleTypeQuestionsState> {

    constructor(props) {
        super(props);

        this.state = {
            soilIsImported: false
        };
    }

    handleChange = (name, value) => {
        this.setState({ ...this.state, [name]: value });
    };

    render() {
        return (
            <div>
                <ForeignSoil sampleType={this.props.sampleType} handleChange={this.handleChange} />
                <SampleWaterQuestions sampleType={this.props.sampleType} handleChange={this.handleChange} />
                <SamplePlantQuestions sampleType={this.props.sampleType} handleChange={this.handleChange} />
            </div>

        );
    }
}
