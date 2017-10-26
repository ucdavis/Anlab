import * as React from "react";
import Input from "./ui/input/input";

interface IProjectInputProps {
    project: string;
    handleChange: (key: string, value: string) => void;
    projectRef: (element: HTMLInputElement) => void;
}

interface IProjectInputState {
    internalValue: string;
    error: string;
}

export class Project extends React.Component<IProjectInputProps, IProjectInputState> {
    constructor(props) {
        super(props);

        this.state = {
            error: null,
            internalValue: this.props.project,
        };
    }

    public render() {
        return (
            <div>
                <Input
                    label="Project Title"
                    value={this.state.internalValue}
                    error={this.state.error}
                    required={true}
                    maxLength={256}
                    onBlur={this._onBlur}
                    onChange={this._onChange}
                    inputRef={this.props.projectRef}
                />
            </div>
          );
    }

    private _onChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value;
        this.setState({ internalValue: value });
        this._validate(value);
    }

    private _onBlur = () => {
        const internalValue = this.state.internalValue;
        this._validate(internalValue);
        this.props.handleChange("project", internalValue);
    }

    private _validate = (v: string) => {
        let error = null;
        if (v.trim() === "") {
            error = "The project Title is required";
        }

        this.setState({ error });
    }
}
