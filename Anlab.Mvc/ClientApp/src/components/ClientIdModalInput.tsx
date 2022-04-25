import * as React from "react";
import Input from "./ui/input/input";

interface IClientIdModalInputProps {
  property: string;
  value: string;
  label: string;
  handleChange: (keys: string[], values: string[]) => void;
  required?: boolean;
}

interface IClientIdModalInputState {
  error: string;
}

export class ClientIdModalInput extends React.Component<
  IClientIdModalInputProps,
  IClientIdModalInputState
> {
  constructor(props) {
    super(props);

    this.state = {
      error: null,
    };
  }

  componentDidMount() {
    if (!!this.props.value) {
      this._validate(this.props.value);
    }
  }

  _validate = (v: string) => {
    let error = null;
    const emailRe = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

    if (this.props.required === true && (!v || v.trim() === "")) {
      error = "This field is required";
    } else if (
      (this.props.property === "email" || this.props.property === "piEmail") &&
      !emailRe.test(v)
    ) {
      error = "Invalid email";
    }
    this.setState({ error });
  };

  onChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    this._validate(value);
    this.props.handleChange([this.props.property], [value]);
  };

  onBlur = () => {
    this._validate(this.props.value);
  };

  render() {
    return (
      <Input
        required={this.props.required}
        error={this.state.error}
        value={this.props.value}
        onBlur={this.onBlur}
        onChange={this.onChange}
        label={this.props.label}
      />
    );
  }
}
