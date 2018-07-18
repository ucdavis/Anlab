import * as React from "react";

export interface IAdditionalInfoProps {
  handleChange: (key: string, value: string) => void;
  name: string;
  value: string;
}

export class AdditionalInfo extends React.Component<IAdditionalInfoProps, {}> {
  public render() {
    return (
      <div className="form-group">
        <label htmlFor="additionalInfo">
          Comments, sampled date range, special test requests, and missing samples
        </label>
        <textarea
          className="form-control"
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

  private _onChange = e => {
    this.props.handleChange("additionalInfo", e.target.value);
  };
}
