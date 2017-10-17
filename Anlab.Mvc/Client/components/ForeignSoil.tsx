import * as React from 'react';
import Checkbox from 'react-toolbox/lib/checkbox';

interface IForeignSoilProps {
    handleChange: Function;
    sampleType: string;
}

interface IForeignSoilState {
    soilIsImported: boolean;
}

export class ForeignSoil extends React.Component<IForeignSoilProps, IForeignSoilState> {

    constructor(props) {
        super(props);

        this.state = {
            soilIsImported: false
        };
    }

    onChange = () => {
        this.setState({ soilIsImported: !this.state.soilIsImported });
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
                                <input type="radio" checked={this.state.soilIsImported} onChange={this.onChange} /> Yes
                            </td>
                            <td>
                                <input type="radio" checked={!this.state.soilIsImported} onChange={this.onChange} /> No
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        );
    }
}