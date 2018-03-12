import * as React from "react";
import Input from "./ui/input/input";

interface ISampleDispositionProps {
    sampleDispositionRef: (element: HTMLInputElement) => void;
    handleChange: (key: string, value: any) => void;
    disposition: string;
}

export const SampleDispositionOptions = {
    dipose: "Dispose of my samples 30 days from report date.",
    pickUp: "I will pick up my samples not later than 30 days from report date.",
    return: "Return my samples to me at my cost.",
}

export class SampleDisposition extends React.Component<
    ISampleDispositionProps,
    {}
    > {

    public render() {

        return (
            <div className="input-group">
                <p>
                    <label>
                        <input
                            className="videokilledtheradiostar"
                            type="radio"
                            value={SampleDispositionOptions.dipose}
                            checked={this.props.disposition == SampleDispositionOptions.dipose}
                            onChange={this._onChange}
                        />
                        {SampleDispositionOptions.dipose}
                    </label>
                </p>
                <p>
                    <label>
                        <input
                            className="videokilledtheradiostar"
                            type="radio"
                            value={SampleDispositionOptions.pickUp}
                            checked={this.props.disposition == SampleDispositionOptions.pickUp}
                            onChange={this._onChange}
                        />
                        {SampleDispositionOptions.pickUp}
                    </label>
                </p>
                <p>
                    <label>
                        <input
                            className="videokilledtheradiostar"
                            type="radio"
                            value={SampleDispositionOptions.return}
                            checked={this.props.disposition == SampleDispositionOptions.return}
                            onChange={this._onChange}
                        />
                        {SampleDispositionOptions.return}
                    </label>
                </p>
            </div>
        );

    }

    private _onChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value;
        this.props.handleChange("sampleDisposition", value);
    }

}
