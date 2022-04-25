import * as React from "react";
import DatePicker from "react-datepicker";
import Input from "./ui/input/input";
import moment from "moment";

interface IDateSampledProps {
  date?: any;
  handleChange: (key: string, value: any) => void;
  dateRef: (element: HTMLInputElement) => void;
}

interface IDateSampledState {
  error: string;
}

export class DateSampled extends React.Component<
  IDateSampledProps,
  IDateSampledState
> {
  constructor(props) {
    super(props);

    this.state = {
      error: "",
    };
  }

  //use opentoDate so if prop is null calendar still works properly
  public render() {
    return (
      <DatePicker
        selected={this.props.date}
        onChange={this._onChange}
        onChangeRaw={this._onChangeRaw}
        required={true}
        dateFormat="MM/DD/YYYY"
        openToDate={moment()}
        customInput={
          <Input
            label="Date Sampled"
            value={this.props.date}
            inputRef={this.props.dateRef}
            error={this.state.error}
          />
        }
      />
    );
  }

  //will always be called on a valid date format
  private _onChange = (d: any) => {
    this.setState({ error: "" });
    this.props.handleChange("dateSampled", d);
  };

  //actual string input, validate here
  private _onChangeRaw = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    let m = moment(value, "MM/DD/YYYY", true);
    if (m.isValid()) {
      this._onChange(m);
    } else {
      this.setState({ error: "Please enter a valid date" });
      this.props.handleChange("dateSampled", null);
    }
  };
}
