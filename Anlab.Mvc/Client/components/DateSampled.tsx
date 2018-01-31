import * as React from "react";
import DatePicker from "react-datepicker";

interface IDateSampledProps {
    date?: any;
    handleChange: (key: string, value: any) => void;
    //dateRef: (element: HTMLInputElement) => void;
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
                dateFormat="DD-MM-YYYY"
                className="form-control"
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
