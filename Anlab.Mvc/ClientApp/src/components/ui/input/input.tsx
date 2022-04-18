import * as React from "react";

export interface IInputProps {
    name?: string;
    value: string;
    label?: string;
    error?: string;
    feedback?: string;
    multiline?: boolean;
    maxLength?: number;
    required?: boolean;
    placeholder?: string;
    disabled?: boolean;
    onBlur?: (event: React.FocusEvent<HTMLInputElement>) => void;
    onChange?: (event: React.ChangeEvent<HTMLInputElement>) => void;
    inputRef?: (element: HTMLInputElement) => void;
    onClick?: (element: React.MouseEvent<HTMLInputElement>) => void;
}

export default class Input extends React.PureComponent<IInputProps, {}> {

    public static defaultProps: Partial<IInputProps> = {
        multiline: false,
        required: false,
    };

    public render() {
        let classN = "form-group";
        const { error } = this.props;
        if (error) {            
            classN = "form-group has-error";
        }

        return (
            <div className={classN} >
                {this.renderLabel()}
                <input
                    type="text"
                    className="form-control"
                    name={this.props.name}
                    value={this.props.value}
                    maxLength={this.props.maxLength}
                    required={this.props.required}
                    disabled={this.props.disabled}
                    placeholder={this.props.placeholder}
                    onBlur={this.props.onBlur}
                    onChange={this.props.onChange}
                    onClick={this.props.onClick}
                    ref={this.props.inputRef}
                />
                {this.renderError()}
                {this.renderFeedback()}
            </div>

        );
    }

    private renderLabel() {
        let { label } = this.props;
        if (!label) {
            return null;
        }
        if (this.props.required)
            label += "*";

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
            <span className="form-text text-error help-block">{error}</span>
        );
    }

    private renderFeedback() {
        const { feedback } = this.props;
        if (!feedback) {
            return null;
        }

        return (
            <span className="form-text help-block">{feedback}</span>
        );
    }
}
