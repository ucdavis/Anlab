import * as React from 'react';
import { RadioGroup, RadioButton } from 'react-toolbox/lib/radio';
import Tooltip from 'react-toolbox/lib/tooltip';

interface IWaterFilterProps {
    filterWater: string;
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


export class WaterFilter extends React.Component<IWaterFilterProps, any> {
    handleChange = (filterWater: string) => {
        this.props.handleChange('filterWater', filterWater);
    }
    render() {
        if (this.props.sampleType !== "Water" ) {
            return null;
        }
        return (
            <div>
                <label>Filter?:</label>
                <RadioGroup name='comic' value={this.props.filterWater} onChange={this.handleChange}>
                    <RadioButton label='Yes' value='Yes'>
                        <TooltipDiv style={style} tooltip="Show the cost" tooltipDelay={500} tooltipPosition={'left'}/>
                    </RadioButton>
                    <RadioButton label='No' value='No'/>
                </RadioGroup>
            </div>
        );
    } 
}