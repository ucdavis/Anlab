import * as React from 'react';
import Tooltip from 'react-toolbox/lib/tooltip';
import { RadioGroup, RadioButton } from 'react-toolbox/lib/radio';
import PlantSvg from '../media/plant.svg';
import SoilSvg from '../media/soil.svg';
import WaterSvg from '../media/water.svg';

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
          <div className="form_wrap">
          <RadioGroup name='comic' value={this.props.sampleType} onChange={this.handleChange}>
              <RadioButton label='Soil' value='Soil'>
                  <TooltipDiv style={style} tooltip="A Soil Tooltip" tooltipDelay={500} tooltipPosition={'left'}/>
              </RadioButton>
              <RadioButton label='Water' value='Water'>
                  <TooltipDiv style={style} tooltip="A Water Tooltip" tooltipDelay={500} tooltipPosition={'left'} />
              </RadioButton>
              <RadioButton label='Plant Material' value='Plant'/>
          </RadioGroup>
          <h2 className="form_header">What type of samples?</h2>
          <div className="row">
          <div className="anlab_form_style anlab_form_samplebtn col t-center">
          <SoilSvg /><h3>Soil</h3></div>
          <div className="anlab_form_style anlab_form_samplebtn col t-center">
          <PlantSvg />
          <h3>Plant Material</h3></div>
          <div className="anlab_form_style anlab_form_samplebtn col t-center active-border active-svg active-text">
          <WaterSvg /><h3>Water</h3></div>
          </div>
          </div>


        );
    }
}
