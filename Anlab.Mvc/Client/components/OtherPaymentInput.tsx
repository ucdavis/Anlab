import * as React from "react";
import Input from "./ui/input/input";

interface IOtherPaymentInputProps {
    property: string;
    value: string;
    label: string;
    handleChange: (key: string, value: string) => void;
}

interface IOtherPaymentInputState {
    error: string;
}

export class OtherPaymentInput extends React.Component<IOtherPaymentInputProps, IOtherPaymentInputState> {
    constructor(props) {
        super(props);

        this.state = {
            error: null
        };
    }

    public render() {

        return (
            <Input
                required={true}
                error={this.state.error}
                value={this.props.value}
                onChange={this._handleChange}
                onBlur={this._onBlur}
                label={this.props.label}
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
        let error = null;
        const emailRe = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        const phoneRe = /^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$/;

        if (!v || v.trim() === "") {
            error = "This field is required";
        }
        else if (this.props.property == "acEmail" && !emailRe.test(v)) {
            error = "Invalid email";
        }
        else if (this.props.property == "acPhone" && !phoneRe.test(v)) {
            error = "Invalid phone number";
        }
        this.setState({ ...this.state, error: error });
    }
}
