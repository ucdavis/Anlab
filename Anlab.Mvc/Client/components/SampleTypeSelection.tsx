import * as React from 'react';
import { RadioGroup, RadioButton } from 'react-toolbox/lib/radio';

export interface ISampleType {
    sampleType: string;    
}

interface ISampleTypeProps {
    sampleType: ISampleType;
    onSampleSelected: Function;
}

export class SampleTypeSelection extends React.Component<ISampleTypeProps, any> {
    handleChange = (sampleType: string) => {
        var updatedSoilInfo = { ...this.props.sampleType, sampleType };

        this.props.onSampleSelected(updatedSoilInfo);
    }
    render() {
        return (            
            <RadioGroup name='comic' value={this.props.sampleType.sampleType} onChange={this.handleChange}>
                <RadioButton label='Soil' value='Soil' />
                <RadioButton label='Water' value='Water' />
                <RadioButton label='Plant Material' value='Plant' />
            </RadioGroup>
        );
    }
}