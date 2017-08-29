
import * as React from 'react';
import Input from 'react-toolbox/lib/input';

interface IClientIdProps {
    clientId: string;
    handleChange: Function;
    isFromLab: boolean;
}

interface IClientIdInputState {
    internalValue: string;
    error: string;
}

export class ClientId extends React.Component<IClientIdProps, IClientIdInputState> {

    constructor(props) {
        super(props);
        if (this.props.clientId)
        {
            this.state = {
                internalValue: this.props.clientId,
                error: null
            };
        }
        else
        {
            this.state = {
                internalValue: null,
                error: null
            };
        }

    }
    validate = (v: string) => {
        let error = null;
        if (v.trim() === "") {
            error = "The client id is required";
        }

        this.setState({ error } as IClientIdInputState);
    }


    onChange = (v: string) => {
        this.setState({ ...this.state, internalValue: v });
        this.validate(v);
    }

    onBlur = () => {
        let internalValue = this.state.internalValue;
        this.validate(internalValue);
        this.props.handleChange('clientId', internalValue);
    }
    render() {
        return (
            <Input type='text' onBlur={this.onBlur} required={this.props.isFromLab} error={this.state.error} value={this.state.internalValue} onChange={this.onChange} label='Client Id' />
    );
}
}