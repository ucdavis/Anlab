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
    date: any;
    error: string;
}

export class DateSampled extends React.Component<IDateSampledProps, IDateSampledState> {
    constructor(props) {
        super(props);

        this.state = {
            date: this.props.date,
            error: "",
        };
    }

    //use opentoDate so if prop is null calendar still works properly
    public render() {
        return (
            <DatePicker
                selected={this.state.date}
                onChange={this._onChange}
                onChangeRaw={this._onChangeRaw}
                onBlur={this._onBlur}
                required={true}
                dateFormat="MM/DD/YYYY"
                openToDate={moment()}
                customInput={<Input
                    label="Date Sampled"
                    value={this.state.date}
                    inputRef={this.props.dateRef}
                    error={this.state.error}
                />}
            />
        );
    }

    //will always be called on a valid date format
    private _onChange = (d: any) => {
        this.setState({ date: d, error: "" });
    }

    //actual string input, validate here
    private _onChangeRaw = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value;
        if (!value) {
            this.setState({ error: "Sample Date is required", date: null });
            return;
        }
        let m = moment(value, "MM/DD/YYYY", true);
        if (m.isValid()) {
            this._onChange(m);
        }
        else {
            this.setState({ error: "Please enter a valid date", date: null });
        }

    }

    //sets null if raw has error
    private _onBlur = () => {
        this.props.handleChange("dateSampled", this.state.date);
    }

}
