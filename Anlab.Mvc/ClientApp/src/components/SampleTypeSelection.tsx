import * as React from "react";
import { ReactComponent as PlantSvg } from "../media/plant.svg";
import { ReactComponent as SoilSvg } from "../media/soil.svg";
import { ReactComponent as WaterSvg } from "../media/water.svg";

interface ISampleTypeProps {
    sampleType: string;
    onSampleSelected: (sample: string) => void;
}

export class SampleTypeSelection extends React.Component<ISampleTypeProps, {}> {

    public render() {
        const activeDiv = "anlab_form_style anlab_form_samplebtn active-bg flexcol active-border active-svg active-text";
        const inactiveDiv = "anlab_form_style anlab_form_samplebtn flexcol";
        return (
            <div>
                <div className="flexrow">
                    <div
                        className={this.props.sampleType === "Soil" ? activeDiv : inactiveDiv}
                        onClick={() => this._handleChange("Soil")}
                    >
                        <SoilSvg />
                        <h3>Soil</h3>
                    </div>
                    <div
                        className={this.props.sampleType === "Plant" ? activeDiv : inactiveDiv}
                        onClick={() => this._handleChange("Plant")}
                    >
                        <PlantSvg />
                        <h3>Plant Material</h3>
                    </div>
                    <div
                        className={this.props.sampleType === "Water" ? activeDiv : inactiveDiv}
                        onClick={() => this._handleChange("Water")}
                    >
                        <WaterSvg />
                        <h3>Water</h3>
                    </div>
                </div>
            </div>
        );
    }

    private _handleChange = (sampleType: string) => {
        this.props.onSampleSelected(sampleType);
    }
}
