
import * as React from 'react';
import Input from 'react-toolbox/lib/input';

interface IClientIdProps {
    clientId: string;
    handleChange: Function;
}

interface IClientIdInputState {
    internalValue: string;
    error: string;
}

export class ClientId extends React.Component<IClientIdProps, IClientIdInputState> {

    constructor(props) {
        super(props);

        this.state = {
            internalValue: this.props.clientId,
            error: null
        };
    }

    onChange = (v: string) => {
        this.setState({ ...this.state, internalValue: v });
    }

    onBlur = () => {
        this.props.handleChange('clientId', this.state.internalValue);
    }
    render() {
        return (
            <Input type='text' onBlur={this.onBlur} error={this.state.error} value={this.state.internalValue} onChange={this.onChange} label='Client Id' />
    );
}
}