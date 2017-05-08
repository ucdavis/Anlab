import * as React from 'react';
import Input from 'react-toolbox/lib/input';

interface INumberInputProps {
    value?: number;
    onChanged?: Function;
}

interface INumberInputState {
    internalValue: string;
    error: string;
}

export class NumberInput extends React.Component<INumberInputProps, INumberInputState> {
    constructor(props) {
        super(props);

        this.state = {
            internalValue: this.transformValue(this.props.value),
            error: null
        };
    }

    componentWillReceiveProps(nextProps) {
        this.setState({ ...this.state, internalValue: this.transformValue(nextProps.value) });
    }

    transformValue = (value: Number) => {
        return value ? String(value) : '';
    }
    onChange = (v: string) => {
        const error = isNaN(Number(v)) ? 'Must be a positive number' : null;
        this.setState({ ...this.state, internalValue: v, error  });
    }
    onBlur = () => {
        let internalValue = Number(this.state.internalValue);

        if (isNaN(internalValue)) internalValue = null;

        this.setState({ ...this.state, internalValue: this.transformValue(internalValue), error: null });

        this.props.onChanged(internalValue);
    }
    render() {
        return (
            <Input type='text' label='Number' name='quantity' error={this.state.error} value={this.state.internalValue} onChange={this.onChange} onBlur={this.onBlur} />
        );
    }
}