import * as React from "react";
import { ISampleTypeQuestions } from "./SampleTypeQuestions";
import Input from "./ui/input/input";

interface IWaterQuestionsProps {
  waterPreservativeRef: (element: HTMLInputElement) => void;
  handleChange: (key: string, value: any) => void;
  sampleType: string;
  questions: ISampleTypeQuestions;
}

interface IWaterQuestionsState {
  waterPreservativeInfo: string;
  error: string;
}

export class SampleWaterQuestions extends React.Component<
  IWaterQuestionsProps,
  IWaterQuestionsState
> {
  constructor(props) {
    super(props);

    this.state = {
      waterPreservativeInfo: this.props.questions.waterPreservativeInfo,
      error: null,
    };
  }

  private _changeFilter = () => {
    this.props.handleChange(
      "waterFiltered",
      !this.props.questions.waterFiltered
    );
  };

  private _changePreservative = () => {
    this.props.handleChange(
      "waterPreservativeAdded",
      !this.props.questions.waterPreservativeAdded
    );
  };

  private _onChangePreservativeText = (
    e: React.ChangeEvent<HTMLInputElement>
  ) => {
    const value = e.target.value;
    this.setState({ waterPreservativeInfo: value });
    this._validate(value);
  };

  private _onBlurPreservativeText = () => {
    const waterPreservativeInfo = this.state.waterPreservativeInfo;
    this._validate(waterPreservativeInfo);
    this.props.handleChange(
      "waterPreservativeInfo",
      this.state.waterPreservativeInfo
    );
  };

  private _validate = (v: string) => {
    let error = null;
    if (!v || v.trim() === "") {
      error = "This information is required";
    }

    this.setState({ error } as IWaterQuestionsState);
  };

  public render() {
    if (this.props.sampleType !== "Water") {
      return null;
    }
    return (
      <div className="input-group d-flex flex-col">
        <label className="form_header margin-bottom-zero">
          Are these samples filtered?
        </label>
        <p>
          <label>
            <input
              type="radio"
              checked={this.props.questions.waterFiltered}
              onChange={this._changeFilter}
            />{" "}
            Yes
          </label>
        </p>
        <p>
          <label>
            <input
              type="radio"
              checked={!this.props.questions.waterFiltered}
              onChange={this._changeFilter}
            />{" "}
            No
          </label>
        </p>
        <label className="form_header margin-bottom-zero">
          Was a preservative added to your sample?
        </label>
        <p>
          <label>
            <input
              type="radio"
              checked={this.props.questions.waterPreservativeAdded}
              onChange={this._changePreservative}
            />{" "}
            Yes
          </label>
        </p>
        <div>
          <label>
            <input
              type="radio"
              checked={!this.props.questions.waterPreservativeAdded}
              onChange={this._changePreservative}
            />{" "}
            No
          </label>
          {this.props.questions.waterPreservativeAdded && (
            <div className="order-form-flex-col">
              <Input
                placeholder="Preservative Information"
                inputRef={this.props.waterPreservativeRef}
                error={this.state.error}
                required={true}
                maxLength={256}
                value={this.state.waterPreservativeInfo}
                onChange={this._onChangePreservativeText}
                onBlur={this._onBlurPreservativeText}
                label="Provide more information about your preservative"
              />
            </div>
          )}
        </div>
      </div>
    );
  }
}
