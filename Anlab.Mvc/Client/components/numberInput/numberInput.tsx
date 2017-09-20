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
    required?: boolean;
    numberRef?: any;
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
        this.setState({ internalValue: this.transformValue(nextProps.value) } as INumberInputState);
    }

    transformValue = (value: Number) => {
        return value ? String(value) : '';
    }

    validate = (v: string) => {
        let error = null;
        // if it's not a number, return error
        let value = Number(v);

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

    onChange = (v: string) => {

        this.setState({ internalValue: v } as INumberInputState);

        this.validate(v);

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

        this.validate(this.state.internalValue);

        this.props.onChanged(value);
    }
    render() {
        return (
            <Input
                ref={this.props.numberRef}
                type='text'
                label={this.props.label}
                name={this.props.name}
                error={this.state.error}
                value={this.state.internalValue}
                onChange={this.onChange}
                onBlur={this.onBlur}
                required={this.props.required}
            />
        );
    }
}
