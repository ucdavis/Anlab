import * as React from "react";
import Input from "./ui/input/input";

interface ICommodityProps {
  commodity: string;
  handleChange: (key: string, value: string) => void;
  commodityRef: (element: HTMLInputElement) => void;
}

interface ICommodityState {
  error: string;
}

export class Commodity extends React.Component<
  ICommodityProps,
  ICommodityState
> {
  constructor(props) {
    super(props);

    this.state = {
      error: null,
    };
  }

  public render() {
    return (
      <Input
        label="Type of Material / Commodity"
        value={this.props.commodity}
        error={this.state.error}
        required={true}
        onChange={this._onChange}
        inputRef={this.props.commodityRef}
      />
    );
  }

  private _onChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    this._validate(value);
    this.props.handleChange("commodity", value);
  };

  private _validate = (v: string) => {
    let error = null;
    if (v.trim() === "") {
      error = "The Material / Commodity is required";
    }

    this.setState({ error });
  };
}
