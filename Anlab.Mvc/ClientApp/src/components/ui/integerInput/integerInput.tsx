import * as React from "react";
import Input from "../input/input";

interface IIntegerInputProps {
  name?: string;
  label?: string;
  value?: number;
  min?: number;
  max?: number;
  required?: boolean;
  onChange?: (value: number) => void;
  inputRef?: (element: HTMLInputElement) => void;
}

interface IIntegerInputState {
  error: string;
}

export class IntegerInput extends React.Component<
  IIntegerInputProps,
  IIntegerInputState
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
        label={this.props.label}
        name={this.props.name}
        value={this.transformValue(this.props.value)}
        error={this.state.error}
        required={this.props.required}
        onChange={this.onChange}
        inputRef={this.props.inputRef}
      />
    );
  }

  private transformValue = (value: number) => {
    return value ? String(value) : "";
  };

  private validate = (v: string) => {
    let error = null;
    // if it's not a number, return error
    const value = Number(v);

    if (isNaN(value)) {
      error = "Must be a number.";
      this.props.onChange(null);
    }
    // check min range or early return
    else if (!isNaN(this.props.min) && this.props.min > value) {
      error = `Must be a number greater than ${this.props.min}.`;
    }
    // check max range or early return
    else if (!isNaN(this.props.max) && this.props.max < value) {
      error = `Must be a number less than or equal to ${this.props.max}.`;
    }

    this.setState({ error } as IIntegerInputState);
  };

  private onChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    this.props.onChange(Number(value));
    this.validate(value);
  };
}
