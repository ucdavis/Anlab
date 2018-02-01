import * as React from "react";
import DatePicker from "react-datepicker";
import Input from "./ui/input/input";

interface IDateSampledProps {
    date?: any;
    handleChange: (key: string, value: any) => void;
    dateRef: (element: HTMLInputElement) => void;
}

interface IDateSampledState {
    internalValue: any;
}

export class DateSampled extends React.Component<IDateSampledProps, IDateSampledState> {
    constructor(props) {
        super(props);

        this.state = {
            internalValue: this.props.date,
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
                />}
            />
        );
    }

    private _onChange = (d: any) => {
        this.setState({ internalValue: d });
    }

    private _onBlur = () => {
        this.props.handleChange("dateSampled", this.state.internalValue);
    }
}
