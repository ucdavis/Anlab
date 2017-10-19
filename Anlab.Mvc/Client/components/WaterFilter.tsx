import * as React from "react";
import Checkbox from "react-toolbox/lib/checkbox";

interface IWaterFilterProps {
    filterWater: boolean;
    handleChange: (value: string) => void;
    sampleType: string;
}

export class WaterFilter extends React.Component<IWaterFilterProps, {}> {
    public render() {
        if (this.props.sampleType !== "Water" ) {
            return null;
        }
        return (
            <div>
                <div>
                    <Checkbox
                        checked={this.props.filterWater}
                        label="Filter"
                        onChange={() => this.props.handleChange("filterWater")}
                    />
                </div>
            </div>
        );
    }
}
