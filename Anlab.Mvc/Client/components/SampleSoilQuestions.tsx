import * as React from "react";
import { ISampleTypeQuestions } from "./SampleTypeQuestions";

interface ISampleSoilQuestions {
    handleChange: Function;
    sampleType: string;
    questions: ISampleTypeQuestions;
}

export class SampleSoilQuestions extends React.Component<ISampleSoilQuestions, {}> {


    render() {
        if (this.props.sampleType !== "Soil") {
            return null;
        }
        return (
            <div className="alert alert-warning" role="alert">If your soil is imported, please remember to select that in the Special sections of the tests below.</div>
        );
    }
}
