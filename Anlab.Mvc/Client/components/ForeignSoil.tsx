import * as React from 'react';
import { RadioGroup, RadioButton } from 'react-toolbox/lib/radio';
import Tooltip from 'react-toolbox/lib/tooltip';

interface IForeignSoilProps {
    foreignSoil: string;
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


export class ForeignSoil extends React.Component<IForeignSoilProps, any> {
    handleChange = (foreignSoil: string) => {
        this.props.handleChange('foreignSoil', foreignSoil);
    }
    render() {
        if (this.props.sampleType !== "Soil" ) {
            return null;
        }
        return (
            <div>
                <label>Are these samples Foreign Soil?:</label>
                <RadioGroup name='comic' value={this.props.foreignSoil} onChange={this.handleChange}>
                    <RadioButton label='Yes' value='Yes'>
                        <TooltipDiv style={style} tooltip="Show the cost" tooltipDelay={500} tooltipPosition={'left'}/>
                    </RadioButton>
                    <RadioButton label='No' value='No'/>
                </RadioGroup>
            </div>
        );
    } 
}