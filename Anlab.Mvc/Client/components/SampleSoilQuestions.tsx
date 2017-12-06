import * as React from "react";
import { ISampleTypeQuestions } from "./SampleTypeQuestions";

interface ISampleSoilQuestions {
    handleChange: Function;
    sampleType: string;
    questions: ISampleTypeQuestions;
}

export class SampleSoilQuestions extends React.Component<ISampleSoilQuestions, {}> {

    onChange = () => {
        this.props.handleChange("soilImported", !this.props.questions.soilImported)
    }

    render() {
        if (this.props.sampleType !== "Soil") {
            return null;
        }
        return (
            <div className="input-group">

                                <p>Is your soil sample imported?</p>
                                <p>
                                <input type="radio" checked={this.props.questions.soilImported} onChange={this.onChange} /> Yes
                                </p>
                                <p>
  <input type="radio" checked={!this.props.questions.soilImported} onChange={this.onChange} /> No
                                </p>




            </div>
        );
    }
}
