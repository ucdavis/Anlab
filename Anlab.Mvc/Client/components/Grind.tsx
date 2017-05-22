import * as React from 'react';
import { RadioGroup, RadioButton } from 'react-toolbox/lib/radio';
import Tooltip from 'react-toolbox/lib/tooltip';

interface IGrindProps {
    grind: string;
    handleChange: Function;
    sampleType: string;
}

const TooltipDiv = Tooltip(('div') as any);

const style = {
    position: 'absolute',
    height: 20,
    width: 200,
    top: 0
}


export class Grind extends React.Component<IGrindProps, any> {
    handleChange = (grind: string) => {
        this.props.handleChange('grind', grind);
    }
    render() {
        if (!(this.props.sampleType === "Soil" || this.props.sampleType === "Plant")) {
            return null;
        }
        return (
            <div>
                <label>Grind Samples?:</label>
                <RadioGroup name='comic' value={this.props.grind} onChange={this.handleChange}>
                    <RadioButton label='Yes' value='Yes'>
                        <TooltipDiv style={style} tooltip="Show the cost" tooltipDelay={500} tooltipPosition={'left'}/>
                    </RadioButton>
                    <RadioButton label='No' value='No'/>
                </RadioGroup>
            </div>
        );
    } 
}