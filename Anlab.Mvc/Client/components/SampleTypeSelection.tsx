import * as React from 'react';
import PlantSvg from '../media/plant.svg';
import SoilSvg from '../media/soil.svg';
import WaterSvg from '../media/water.svg';

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
            <div>
                <h2 className="form_header">What type of samples?</h2>
                <div className="row">
                    <div className={this.props.sampleType === "Soil" ? "anlab_form_style anlab_form_samplebtn col t-center active-bg active-border active-svg active-text" : "anlab_form_style anlab_form_samplebtn col t-center"} onClick={() => this.handleChange("Soil")}>
                        <SoilSvg />
                        <h3>Soil</h3>
                    </div>
                    <div className={this.props.sampleType === "Plant" ? "anlab_form_style anlab_form_samplebtn col t-center active-bg active-border active-svg active-text" : "anlab_form_style anlab_form_samplebtn col t-center"} onClick={() => this.handleChange("Plant")}>
                        <PlantSvg />
                        <h3>Plant Material</h3>
                    </div>
                    <div className={this.props.sampleType === "Water" ? "anlab_form_style anlab_form_samplebtn col t-center active-bg active-border active-svg active-text" : "anlab_form_style anlab_form_samplebtn col t-center"} onClick={() => this.handleChange("Water")}>
                        <WaterSvg />
                        <h3>Water</h3>
                    </div>
                </div>
            </div>
        );
    }
}
