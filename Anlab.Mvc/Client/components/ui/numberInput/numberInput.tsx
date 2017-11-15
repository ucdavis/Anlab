import * as React from "react";
import Input from "../input/input";

interface INumberInputProps {
    name?: string;
    label?: string;
    value?: number;
    min?: number;
    max?: number;
    integer?: boolean;
    required?: boolean;
    onBlur?: () => void;
    onChange?: (value: number) => void;
    inputRef?: (element: HTMLInputElement) => void;
}

interface INumberInputState {
    internalValue: string;
    error: string;
}

export class NumberInput extends React.Component<INumberInputProps, INumberInputState> {

    public static defaultProps: Partial<INumberInputProps> = {
        integer: false,
    };

    constructor(props) {
        super(props);

        this.state = {
            error: null,
            internalValue: this.transformValue(this.props.value),
        };
    }

    public componentWillReceiveProps(nextProps) {
        this.setState({ internalValue: this.transformValue(nextProps.value) });
    }

    public render() {
        return (
            <Input
                label={this.props.label}
                name={this.props.name}
                value={this.state.internalValue}
                error={this.state.error}
                required={this.props.required}
                onChange={this.onChange}
                onBlur={this.onBlur}
                inputRef={this.props.inputRef}
            />
        );
    }

    private transformValue = (value: number) => {
        return value ? String(value) : "";
    }

    private validate = (v: string) => {
        let error = null;
        // if it's not a number, return error
        const value = Number(v);

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

    private onChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value;
        this.setState({ internalValue: value } as INumberInputState);
        this.validate(value);
    }

    private onBlur = (event: React.FocusEvent<HTMLInputElement>) => {
        let value = Number(this.state.internalValue);

        if (isNaN(value)) {
            value = null;
        }

        // force integer
        if (this.props.integer && !isNaN(value)) {
          value = Math.floor(value);
        }

        // push possible changes, clear error
        this.setState({
          internalValue: this.transformValue(value),
        });

        this.validate(this.state.internalValue);

        this.props.onChange(value);
    }
}
