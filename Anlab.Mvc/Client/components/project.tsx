import * as React from "react";
import Input from "react-toolbox/lib/input";

interface IProjectInputProps {
    project: string;
    handleChange: (key: string, value: string) => void;
    projectRef: any;
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
                  ref={this.props.projectRef}
                  type="text"
                  onBlur={this._onBlur}
                  error={this.state.error}
                  required={true}
                  maxLength={256}
                  value={this.state.internalValue}
                  onChange={this._onChange}
                  label="Project Title"
                />
            </div>
          );
    }

    private _onChange = (v: string) => {
        this.setState({ internalValue: v });
        this._validate(v);
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
