import * as React from "react";
import Checkbox from "react-toolbox/lib/checkbox";

interface IGrindProps {
    grind: boolean;
    handleChange: (key: string) => void;
    sampleType: string;
}

export class Grind extends React.Component<IGrindProps, {}> {

    public render() {
        if (!(this.props.sampleType === "Soil" || this.props.sampleType === "Plant")) {
            return null;
        }

        return (
            <div>
                <Checkbox
                  checked={this.props.grind}
                  label="Grind Samples"
                  onChange={() => this.props.handleChange("grind")}
                />
            </div>
        );
    }
}
