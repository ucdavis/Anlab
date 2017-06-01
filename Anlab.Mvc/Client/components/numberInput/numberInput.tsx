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

        this.setState({ internalValue: v } as INumberInputState);

        // if it's not a number, return error
        if (isNaN(value)) {
            error = "Must be a number.";
        }
        // check min range or early return
        else if (!isNaN(this.props.min) && this.props.min > value) {
            error = `Must be a number greater than ${this.props.min}.`;
        }
        // check max range or early return
        else if (!isNaN(this.props.max) && this.props.max < value) {
            error = `Must be a number less than or equal to ${this.props.max}.`;
        }

        this.setState({ error } as INumberInputState);
    }
    onBlur = () => {
        let value = Number(this.state.internalValue);

        if (isNaN(value)) value = null;

        // force integer
        if (this.props.integer && !isNaN(value)) {
          value = Math.floor(value);
        }

        // push possible changes, clear error
        this.setState({
          internalValue: this.transformValue(value)
        } as INumberInputState);

        this.props.onChanged(value);
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
