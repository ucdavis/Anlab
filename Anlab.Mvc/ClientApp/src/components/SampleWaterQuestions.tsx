import * as React from "react";
import { ISampleTypeQuestions } from "./SampleTypeQuestions";
import Input from "./ui/input/input";

interface IWaterQuestionsProps {
  waterFilterRef: (element: HTMLInputElement) => void;
  waterPreservativeRef: (element: HTMLInputElement) => void;
  handleChange: (key: string, value: any) => void;
  sampleType: string;
  questions: ISampleTypeQuestions;
}

interface IWaterQuestionsState {
  waterFilterInfo: string;
  waterFilterError: string;
  waterPreservativeInfo: string;
  waterPreservativeError: string;
}

export class SampleWaterQuestions extends React.Component<
  IWaterQuestionsProps,
  IWaterQuestionsState
> {
  constructor(props) {
    super(props);

    this.state = {
      waterFilterInfo: this.props.questions.waterFilterInfo,
      waterFilterError: null,
      waterPreservativeInfo: this.props.questions.waterPreservativeInfo,
      waterPreservativeError: null,
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

  private _onChangeFilterText = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    this.setState({ waterFilterInfo: value });
    this._validateFilter(value);
  };

  private _onBlurFilterText = () => {
    const waterFilterInfo = this.state.waterFilterInfo;
    this._validateFilter(waterFilterInfo);
    this.props.handleChange("waterFilterInfo", this.state.waterFilterInfo);
  };

  private _validateFilter = (v: string) => {
    let waterFilterError = null;
    if (!v || v.trim() === "") {
      waterFilterError = "This information is required";
    }

    this.setState({ waterFilterError } as IWaterQuestionsState);
  };

  private _onChangePreservativeText = (
    e: React.ChangeEvent<HTMLInputElement>
  ) => {
    const value = e.target.value;
    this.setState({ waterPreservativeInfo: value });
    this._validatePreservative(value);
  };

  private _onBlurPreservativeText = () => {
    const waterPreservativeInfo = this.state.waterPreservativeInfo;
    this._validatePreservative(waterPreservativeInfo);
    this.props.handleChange(
      "waterPreservativeInfo",
      this.state.waterPreservativeInfo
    );
  };

  private _validatePreservative = (v: string) => {
    let waterPreservativeError = null;
    if (!v || v.trim() === "") {
      waterPreservativeError = "This information is required";
    }

    this.setState({ waterPreservativeError } as IWaterQuestionsState);
  };

  public render() {
    if (
      this.props.sampleType !== "Water" &&
      this.props.sampleType !== "Miscellaneous"
    ) {
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
        <div>
          <label>
            <input
              type="radio"
              checked={!this.props.questions.waterFiltered}
              onChange={this._changeFilter}
            />{" "}
            No
          </label>
          {this.props.questions.waterFiltered && (
            <div className="order-form-flex-col">
              <Input
                placeholder="Filter Information"
                inputRef={this.props.waterFilterRef}
                error={this.state.waterFilterError}
                required={true}
                maxLength={256}
                value={this.state.waterFilterInfo}
                onChange={this._onChangeFilterText}
                onBlur={this._onBlurFilterText}
                label="Provide filter information and procedure used"
              />
            </div>
          )}
        </div>
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
                error={this.state.waterPreservativeError}
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
