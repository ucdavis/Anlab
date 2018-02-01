import * as React from "react";
import { ISampleTypeQuestions } from "./SampleTypeQuestions";

interface ISampleSoilQuestions {
    handleChange: Function;
    sampleType: string;
    questions: ISampleTypeQuestions;
}

export class SampleSoilQuestions extends React.Component<ISampleSoilQuestions, {}> {

    onChange = () => {
        this.props.handleChange("soilImported", !this.props.questions.soilImported);
    }

    render() {
        if (this.props.sampleType !== "Soil") {
            return null;
        }
        return (
            <div className="input-group">
                <label className="form_header margin-bottom-zero">
                  Is your soil sample imported?
                </label>
                <p>
                    <label>
                        <input type="radio" checked={this.props.questions.soilImported} onChange={this.onChange}/> Yes
                    </label>                    
                </p>
                <p>
                    <label>
                        <input type="radio" checked={!this.props.questions.soilImported} onChange={this.onChange}/> No
                    </label>
                </p>
                {this.props.questions.soilImported && (<p>Please remember to choose "Imported Soil" in the tests below to have an accurate order price.</p>)}
            </div>
        );
    }
}
