import * as React from 'react';
import Input from 'react-toolbox/lib/input';

interface INumberInputProps {
    name?: string;
    label?: string;
    value?: number;
    onChanged?: Function;
    min?: number;
    max?: number;
    integer?: boolean;
}

interface INumberInputState {
    internalValue: string;
    error: string;
}

export class NumberInput extends React.Component<INumberInputProps, INumberInputState> {

    public static defaultProps: Partial<INumberInputProps> = {
        integer: false
    };

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
        let value = Number(v);
        if (isNaN(value)) {
            error = "Must be a positive number greater than 0";
            this.setState({ ...this.state, internalValue: v, error });
        } else {
            value = Number(value.toFixed(0)); //Strip of any decimals
            if (value === 0) {
                error = "Must be a positive number greater than 0";
            }
            if (value < 0) {
                value = (value * -1); //Make positive if negative
            }
            this.setState({ ...this.state, internalValue: value.toString(), error });
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
            <Input
                type='text'
                label={this.props.label}
                name={this.props.name}
                error={this.state.error}
                value={this.state.internalValue}
                onChange={this.onChange}
                onBlur={this.onBlur}
            />
        );
    }
}
