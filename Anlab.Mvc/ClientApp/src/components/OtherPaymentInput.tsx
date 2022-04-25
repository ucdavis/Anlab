import * as React from "react";
import Input from "./ui/input/input";

interface IOtherPaymentInputProps {
    property: string;
    value: string;
    label: string;
    required: boolean;
    handleChange: (key: string, value: string) => void;
    inputRef?: any;
}

interface IOtherPaymentInputState {
    error: string;
}

export class OtherPaymentInput extends React.Component<IOtherPaymentInputProps, IOtherPaymentInputState> {
    constructor(props) {
        super(props);

        this.state = {
            error: ""
        };
    }

    public render() {

        return (
            <Input
                required={this.props.required}
                error={this.props.required ? this.state.error : ""} //clear out error on PO if no longer required
                value={this.props.value}
                onChange={this._handleChange}
                onBlur={this._onBlur}
                label={this.props.label}
                inputRef={this.props.inputRef}
            />
        );
    }

    private _handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value;
        this.props.handleChange(this.props.property, value);
        this._validate(value);
    }

    private _onBlur = () => {
        this._validate(this.props.value);
    }

    private _validate = (v: string) => {
        let error = "";
        const emailRe = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        if (this.props.required && (!v || v.trim() === "")) {
            error = "This field is required";
        } else if (this.props.property === "acEmail" && !emailRe.test(v)) {
            error = "Invalid email";
        }
        

        this.setState({ ...this.state, error: error });
    }
}
