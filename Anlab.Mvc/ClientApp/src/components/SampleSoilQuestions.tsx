import * as React from "react";
import { ISampleTypeQuestions } from "./SampleTypeQuestions";

interface ISampleSoilQuestions {
  handleChange: (key: string, value: any) => void;
  sampleType: string;
  questions: ISampleTypeQuestions;
}

export class SampleSoilQuestions extends React.Component<
  ISampleSoilQuestions,
  {}
> {
  private _changeImportedSoil = () => {
    this.props.handleChange("soilImported", !this.props.questions.soilImported);
  };

  render() {
    if (this.props.sampleType !== "Soil") {
      return null;
    }
    return (
      <div className="input-group">
        <label className="form_header margin-bottom-zero">
          Are these samples Quarantined Soil either foreign or regulated
          domestic soils?
        </label>
        <p>
          <label>
            <input
              type="radio"
              checked={this.props.questions.soilImported}
              onChange={this._changeImportedSoil}
            />{" "}
            Yes
          </label>
        </p>
        <p>
          <label>
            <input
              type="radio"
              checked={!this.props.questions.soilImported}
              onChange={this._changeImportedSoil}
            />{" "}
            No
          </label>
        </p>
        {this.props.questions.soilImported && (
          <div className="alert alert-warning" role="alert">
            <p>
              You must select{" "}
              <strong>
                Quarantined Soil Processing Fee - Required for foreign &
                regulated domestic soils
              </strong>{" "}
              in the tests below.
            </p>
          </div>
        )}
        {!this.props.questions.soilImported && (
          <div className="alert alert-info" role="alert">
            <p>
              Do not select the Quarantined Soil Processing Fee unless your soil
              is either foreign or regulated domestic soils.
            </p>
          </div>
        )}
      </div>
    );

    //Keep this here in case we need to add soil questions back in.
    //return (
    //  <div className="alert alert-warning" role="alert">
    //    We do not accept foreign soils.
    //  </div>
    //);
  }
}
