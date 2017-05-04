import * as React from 'react';
import { RadioGroup, RadioButton } from 'react-toolbox/lib/radio';

interface ISampleTypeProps {
    sampleType: string;
    onSampleSelected: Function;
}

export class SampleTypeSelection extends React.Component<ISampleTypeProps, any> {
    handleChange = (sampleType: string) => {
        this.props.onSampleSelected(sampleType);
    }
    render() {
        return (            
            <RadioGroup name='comic' value={this.props.sampleType} onChange={this.handleChange}>
                <RadioButton label='Soil' value='Soil' />
                <RadioButton label='Water' value='Water' />
                <RadioButton label='Plant Material' value='Plant' />
            </RadioGroup>
        );
    }
}