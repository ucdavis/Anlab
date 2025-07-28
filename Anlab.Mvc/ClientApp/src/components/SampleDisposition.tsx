import * as React from "react";
import Input from "./ui/input/input";

interface ISampleDispositionProps {
  sampleDispositionRef: (element: HTMLInputElement) => void;
  handleChange: (key: string, value: any) => void;
  disposition: string;
}

export const SampleDispositionOptions = {
  dispose: "Dispose of my samples 30 days from report date.",
  pickUp: "I will pick up my samples not later than 30 days from report date.",
  return:
    "Return my samples to me at my cost. (This option is not available to UCD Clients)",
};

export class SampleDisposition extends React.Component<
  ISampleDispositionProps,
  {}
> {
  public render() {
    return (
      <div className="input-group flex-col">
        <p>
          <label>
            <input
              className="videokilledtheradiostar"
              type="radio"
              value={SampleDispositionOptions.dispose}
              checked={
                this.props.disposition === SampleDispositionOptions.dispose
              }
              onChange={this._onChange}
              ref={this.props.sampleDispositionRef}
            />
            {SampleDispositionOptions.dispose}
          </label>
        </p>
        <br />
        <p>
          <label>
            <input
              className="videokilledtheradiostar"
              type="radio"
              value={SampleDispositionOptions.pickUp}
              checked={
                this.props.disposition === SampleDispositionOptions.pickUp
              }
              onChange={this._onChange}
            />
            {SampleDispositionOptions.pickUp}
          </label>
        </p>
        <br />
        <p>
          <label>
            <input
              className="videokilledtheradiostar"
              type="radio"
              value={SampleDispositionOptions.return}
              checked={
                this.props.disposition === SampleDispositionOptions.return
              }
              onChange={this._onChange}
            />
            {SampleDispositionOptions.return}
          </label>
        </p>
        {!this.props.disposition && (
          <span className="red-text help-block">
            You must select how you would like your samples to be disposed of.
          </span>
        )}
      </div>
    );
  }

  private _onChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    this.props.handleChange("sampleDisposition", value);
  };
}
