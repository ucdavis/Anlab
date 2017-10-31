import * as React from "react";
import Input from "react-toolbox/lib/input";

interface IClientIdModalInputProps {
    property: string;
    value: string;
    label: string;
    handleChange: Function;
}

interface IClientIdModalInputState {
    internalValue: string;
    error: string;
}

export class ClientIdModalInput extends React.Component<IClientIdModalInputProps, IClientIdModalInputState> {
    constructor(props) {
        super(props);

        this.state = {
            internalValue: this.props.value,
            error: null
        };
    }

    validate = (v: string) => {
        let error = null;
        const emailRe = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        const phoneRe = /^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$/;

        if (v.trim() === "") {
            error = "This field is required";
        }
        else if (this.props.property == "email" && !emailRe.test(v))
        {
            error = "Invalid email";
        }
        else if (this.props.property == "phoneNumber" && !phoneRe.test(v))
        {
            error = "Invalid phone number";
        }
        this.setState({ ...this.state, error: error });
    }

    onChange = (v: string) => {
        this.setState({ internalValue: v });
    }

    onBlur = () => {
        let internalValue = this.state.internalValue;
        this.validate(internalValue);
        this.props.handleChange(this.props.property, internalValue);
    }

    render() {
        return (
            <Input type='text' required error={this.state.error} value={this.state.internalValue} onChange={this.onChange} onBlur={this.onBlur} label={this.props.label} />
        );
    }
}
