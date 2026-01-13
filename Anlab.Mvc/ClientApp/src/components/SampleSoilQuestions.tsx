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

  private _changeSoilAgreement = (e: React.ChangeEvent<HTMLInputElement>) => {
    this.props.handleChange("soilAgreement", e.target.value);
  };

  private _trimSoilAgreement = () => {
    if (this.props.questions.soilAgreement) {
      const trimmed = this.props.questions.soilAgreement.trim();
      if (trimmed !== this.props.questions.soilAgreement) {
        this.props.handleChange("soilAgreement", trimmed);
      }
    }
  };

  render() {
    if (
      this.props.sampleType !== "Soil" &&
      this.props.sampleType !== "Miscellaneous"
    ) {
      return null;
    }
    return (
      <div className="input-group flex flex-col">
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
          <div>
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
            <div className="form-group">
              <label
                className="form_header margin-bottom-zero"
                htmlFor="soilAgreement"
              >
                I agree to pay any shipping, handling, or customs fees the lab
                incurs (type yes).
              </label>
              <input
                type="text"
                id="soilAgreement"
                className={`form-control ${
                  this.props.questions.soilAgreement?.toLowerCase() !== "yes"
                    ? "is-invalid"
                    : "is-valid"
                }`}
                value={this.props.questions.soilAgreement || ""}
                onChange={this._changeSoilAgreement}
                onBlur={this._trimSoilAgreement}
                placeholder="Type 'yes' to agree"
              />
              {this.props.questions.soilAgreement?.toLowerCase() !== "yes" && (
                <div className="invalid-feedback" style={{ display: "block" }}>
                  You must type "yes" to agree to the terms.
                </div>
              )}
            </div>
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
