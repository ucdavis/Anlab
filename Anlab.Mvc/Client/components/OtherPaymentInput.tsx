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
                label={this.props.label}
            />
        );
    }

    private _handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value;
        this.props.handleChange(this.props.property, value);
    }

}
