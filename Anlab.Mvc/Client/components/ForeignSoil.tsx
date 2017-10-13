import * as React from "react";
import Checkbox from "react-toolbox/lib/checkbox";

interface IForeignSoilProps {
    foreignSoil: boolean;
    handleChange: (key: string) => void;
    sampleType: string;
}

export class ForeignSoil extends React.Component<IForeignSoilProps, {}> {
    public render() {
        if (this.props.sampleType !== "Soil" ) {
            return null;
        }

        return (
            <div>
                <Checkbox
                  checked={this.props.foreignSoil}
                  label="Foreign Soil"
                  onChange={() => this.props.handleChange("foreignSoil")}
                />
            </div>
        );
    }
}
