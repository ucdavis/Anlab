import * as React from "react";

export interface IInputProps {
    name?: string;
    value: string;
    label?: string;
    error?: string;
    multiline?: boolean;
    maxLength?: number;
    required?: boolean;
    placeholder?: string;
    onBlur?: (event: React.FocusEvent<HTMLInputElement>) => void;
    onChange?: (event: React.ChangeEvent<HTMLInputElement>) => void;
    inputRef?: (element: HTMLInputElement) => void;
}

export default class Input extends React.PureComponent<IInputProps, {}> {

    public static defaultProps: Partial<IInputProps> = {
        multiline: false,
        required: false,
    };

    public render() {
        return (
            <div className="form-group">
                {this.renderLabel()}
                <input
                    type="text"
                    className="form-control"
                    name={this.props.name}
                    value={this.props.value}
                    maxLength={this.props.maxLength}
                    required={this.props.required}
                    placeholder={this.props.placeholder}
                    onBlur={this.props.onBlur}
                    onChange={this.props.onChange}
                    ref={this.props.inputRef}
                />
                {this.renderError()}
            </div>

        );
    }

    private renderLabel() {
        const { label } = this.props;
        if (!label) {
            return null;
        }

        return (
            <label className="control-label">{label}</label>
        );
    }

    private renderError() {
        const { error } = this.props;
        if (!error) {
            return null;
        }

        return (
            <small className="form-text text-error">{error}</small>
        );
    }
}
