import * as React from 'react';
import Checkbox from 'react-toolbox/lib/checkbox';
import { Input } from "react-toolbox/lib/input";

interface IPlantQuestionProps {
    sampleType: string;
    handleChange: Function;
}

interface IPlantQuestionState {
    reportingBasis: string;
}

export class SamplePlantQuestions extends React.Component<IPlantQuestionProps, IPlantQuestionState> {

    constructor(props) {
        super(props);

        this.state = {
            reportingBasis: null,
        };
    }

    private _changeReporting = (e) => {
        console.log(e.target.value);
        this.setState({ reportingBasis: e.target.value });
    }

    render() {
        if (this.props.sampleType !== "Plant") {
            return null;
        }
        const option1 = "Report results on 100% dry weight basis, based on an average of 10% of the samples.";
        const option2 = "Report results on As Received basis.";
        const option3 = "Report results on 100% dry weight basis, based on individual dry matter results (charge applies).";
        return (
            <div>
                <table>
                    <tbody>
                        <tr>
                            <th>
                                How would you like your samples reported?
                            </th>
                        </tr>
                        <tr>
                            <td>
                                <input type="radio" value={option1} checked={ !this.state.reportingBasis || this.state.reportingBasis == option1 } onChange={this._changeReporting} />
                                {option1}
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <input type="radio" value={option2} checked={ this.state.reportingBasis == option2 } onChange={this._changeReporting} />
                                {option2}
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <input type="radio" value={option3} checked={ this.state.reportingBasis == option3 } onChange={this._changeReporting} />
                                {option3}
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        );
    }
}