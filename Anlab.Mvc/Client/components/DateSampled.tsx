import * as React from "react";
import DatePicker from "react-datepicker";
import Input from "./ui/input/input";
import * as moment from "moment";

interface IDateSampledProps {
    date?: any;
    handleChange: (key: string, value: any) => void;
    dateRef: (element: HTMLInputElement) => void;
}

interface IDateSampledState {
    internalValue: any;
    error: string;
}

export class DateSampled extends React.Component<IDateSampledProps, IDateSampledState> {
    constructor(props) {
        super(props);

        this.state = {
            internalValue: this.props.date,
            error: "",
        };
    }

    public render() {
        return (
            <DatePicker
                selected={this.state.internalValue}
                onChange={this._onChange}
                onBlur={this._onBlur}
                dateFormat="MM/DD/YYYY"
                customInput={<Input
                    label="Date Sampled"
                    value={this.state.internalValue}
                    required={true}
                    inputRef={this.props.dateRef}
                    error={this.state.error}
                />}
            />
        );
    }

    private _onChange = (d: any) => {
        this.setState({ internalValue: d }, this._validate);
    }

    private _onBlur = () => {
        this.props.handleChange("dateSampled", this.state.internalValue);
    }

    private _validate = () => {
        if (!this.state.internalValue) {
            this.setState({ error: "Date sampled is required" });
        } else if (!moment.isMoment(this.state.internalValue)) {
            this.setState({ error: "Please enter a valid date" });
        } else {
            this.setState({ error: "" });
        }
    }
}
