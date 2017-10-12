import * as React from 'react';
import Input from 'react-toolbox/lib/input';

interface IClientIdModalInputProps {
    property: string;
    handleChange: Function;
}

interface IClientIdModalInputState {
    internalValue: string;
}

export class ClientIdModalInput extends React.Component<IClientIdModalInputProps, IClientIdModalInputState> {
    constructor(props) {
        super(props);

        this.state = {
            internalValue: ''
        };
    }

    onChange = (v: string) => {
        this.setState({ internalValue: v });
    }

    onBlur = () => {
        this.props.handleChange(this.props.property, this.state.internalValue);
    }

    render() {
        return (
            <Input type='text' value={this.state.internalValue} onChange={this.onChange} onBlur={this.onBlur} label='Additional Info Required' />
        );
    }
}