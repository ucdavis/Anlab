import * as React from "react";
import Input from "./ui/input/input";

interface ISampleDispositionProps {
    sampleDispositionRef: (element: HTMLInputElement) => void;
    handleChange: (key: string, value: any) => void;
    disposition: string;
}

interface ISampleDispositionState {
    error: string;
}

export class SampleDisposition extends React.Component<
    ISampleDispositionProps,
    ISampleDispositionState
    > {
    constructor(props) {
        super(props);

        this.state = {
            error: null
        };
    }

    public render() {
        return (
                <Input
                label="Sample Disposition"
                value={this.props.disposition}
                    error={this.state.error}
                    required={true}
                    maxLength={256}
                    onChange={this._onChange}
                    inputRef={this.props.sampleDispositionRef}
                />
        );
    }

    private _onChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value;
        this._validate(value);
        this.props.handleChange("sampleDisposition", value);
    }


    private _validate = (v: string) => {
        let error = null;
        if (v.trim() === "") {
            error = "Sample Disposition is required";
        }

        this.setState({ error });
    }
}
