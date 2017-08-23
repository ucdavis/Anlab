import * as React from 'react';
import PlantSvg from '../media/plant.svg';
import SoilSvg from '../media/soil.svg';
import WaterSvg from '../media/water.svg';
import OtherSvg from '../media/compost.svg';

interface ISampleTypeProps {
    sampleType: string;
    onSampleSelected: Function;
}


export class SampleTypeSelection extends React.Component<ISampleTypeProps, any> {
    handleChange = (sampleType: string) => {
        this.props.onSampleSelected(sampleType);
    }
    render() {
        const activeDiv = "anlab_form_style anlab_form_samplebtn col t-center active-bg active-border active-svg active-text";
        const inactiveDiv = "anlab_form_style anlab_form_samplebtn col t-center";
        return (
            <div className="form_wrap">
                <h2 className="form_header">What type of samples?</h2>
                <div className="row">
                    <div className={this.props.sampleType === "Soil" ? activeDiv : inactiveDiv} onClick={() => this.handleChange("Soil")}>
                        <SoilSvg />
                        <h3>Soil</h3>
                    </div>
                    <div className={this.props.sampleType === "Plant" ? activeDiv : inactiveDiv} onClick={() => this.handleChange("Plant")}>
                        <PlantSvg />
                        <h3>Plant Material</h3>
                    </div>
                    <div className={this.props.sampleType === "Water" ? activeDiv : inactiveDiv} onClick={() => this.handleChange("Water")}>
                        <WaterSvg />
                        <h3>Water</h3>
                    </div>
                    <div className={this.props.sampleType === "Other" ? activeDiv : inactiveDiv} onClick={() => this.handleChange("Other")}>
                        <OtherSvg />
                        <h3>Other</h3>
                    </div>
                </div>
            </div>
        );
    }
}
