import * as React from 'react';
import Checkbox from 'react-toolbox/lib/checkbox';
import { ISampleTypeQuestions } from "./SampleTypeQuestions";

interface ISampleSoilQuestions {
    handleChange: Function;
    sampleType: string;
    questions: ISampleTypeQuestions;
}

export class SampleSoilQuestions extends React.Component<ISampleSoilQuestions, {}> {

    constructor(props) {
        super(props);

    }

    onChange = () => {
        this.props.handleChange("soilImported", !this.props.questions.soilImported)
    }

    render() {
        if (this.props.sampleType !== "Soil") {
            return null;
        }
        return (
            <div>
                <table>
                    <tbody>
                        <tr>
                            <th colSpan={2}>
                                Is your soil sample imported?
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <input type="radio" checked={this.props.questions.soilImported} onChange={this.onChange} /> Yes
                            </td>
                            <td>
                                <input type="radio" checked={!this.props.questions.soilImported} onChange={this.onChange} /> No
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        );
    }
}