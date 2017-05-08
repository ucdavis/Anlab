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
        let error = null;
        if (isNaN(Number(v))) {
            error = "Must be a positive number greater than 1";
            this.setState({ ...this.state, internalValue: v, error });
        } else {
            let value = Number(v).toFixed(0); //Strip of any decimals
            if (Number(value) === 0) {
                error = "Must be a positive number greater than 1";
            }
            if (Number(value) < 0) {
                value = (Number(value) * -1).toString(); //Make positive if negative
            }
            this.setState({ ...this.state, internalValue: value, error });
        }        
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