import * as React from 'react';
import Tooltip from 'react-toolbox/lib/tooltip';
import { RadioGroup, RadioButton } from 'react-toolbox/lib/radio';

interface ISampleTypeProps {
    sampleType: string;
    onSampleSelected: Function;
}

const TooltipDiv = Tooltip(('div') as any);

const style = {
    position: 'absolute',
    height: 20,
    width: 200,
    top: 0
}

export class SampleTypeSelection extends React.Component<ISampleTypeProps, any> {
    handleChange = (sampleType: string) => {
        this.props.onSampleSelected(sampleType);
    }
    render() {
        return (
            <RadioGroup name='comic' value={this.props.sampleType} onChange={this.handleChange}>
                <RadioButton label='Soil' value='Soil'>
                    <TooltipDiv style={style} tooltip="A Soil Tooltip" tooltipDelay={500} tooltipPosition={'left'}/>
                </RadioButton>
                <RadioButton label='Water' value='Water'>
                    <TooltipDiv style={style} tooltip="A Water Tooltip" tooltipDelay={500} tooltipPosition={'left'} />
                </RadioButton>
                <RadioButton label='Plant Material' value='Plant'/>
            </RadioGroup>
        );
    }
}