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
            <div>
                <label className="control-label">Sample Date*</label>
                <DatePicker
                    selected={this.state.internalValue}
                    onChange={this._onChange}
                    onBlur={this._onBlur}
                    dateFormat="MM/DD/YYYY"
                    className="form-control"
                />
            </div>
        );
    }

    private _onChange = (d: any) => {
        this.setState({ internalValue: d });
    }

    private _onBlur = () => {
        this.props.handleChange("dateSampled", this.state.internalValue);
    }
}
