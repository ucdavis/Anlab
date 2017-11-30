import * as React from "react";
import Input from "./ui/input/input";

interface IProjectInputProps {
    project: string;
    handleChange: (key: string, value: string) => void;
    projectRef: (element: HTMLInputElement) => void;
}

interface IProjectInputState {
    error: string;
}

export class Project extends React.Component<IProjectInputProps, IProjectInputState> {
    constructor(props) {
        super(props);

        this.state = {
            error: null,
        };
    }

    public render() {
        return (
            <div>
                <Input
                    label="Project Title"
                    value={this.props.project}
                    error={this.state.error}
                    required={true}
                    maxLength={256}
                    onChange={this._onChange}
                    inputRef={this.props.projectRef}
                />
            </div>
          );
    }

    private _onChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value;
        this._validate(value);
        this.props.handleChange("project", value);
    }


    private _validate = (v: string) => {
        let error = null;
        if (v.trim() === "") {
            error = "The project Title is required";
        }

        this.setState({ error });
    }
}
