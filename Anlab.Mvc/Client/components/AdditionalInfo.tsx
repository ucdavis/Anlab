import * as React from "react";
import Input from "react-toolbox/lib/input";

export interface IAdditionalInfoProps {
    handleChange: (key: string, value: string) => void;
    name: string;
    value: string;
}

export class AdditionalInfo extends React.Component<IAdditionalInfoProps, {}> {
    public render() {
        return (
            <div className="form-control">
                <label htmlFor="additionalInfo">Additional Info</label>
                <textarea
                  label="Additional Information"
                  name={this.props.name}
                  value={this.props.value}
                  onChange={this._onChange}
                  maxLength={2000}
                  rows={4}
                />
            </div>
        );
    }

    private _onChange = (e) => {
        this.props.handleChange("additionalInfo", e.target.value);
    }
}
