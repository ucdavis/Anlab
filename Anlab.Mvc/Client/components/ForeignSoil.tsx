import * as React from 'react';
import Checkbox from 'react-toolbox/lib/checkbox';

interface IForeignSoilProps {
    foreignSoil: boolean;
    handleChange: Function;
    sampleType: string;
}



export class ForeignSoil extends React.Component<IForeignSoilProps, any> {
    render() {
        if (this.props.sampleType !== "Soil" ) {
            return null;
        }
        return (
            <div>
                <Checkbox checked={this.props.foreignSoil} label="Foreign Soil" onChange={this.props.handleChange.bind(this, 'foreignSoil')} />
            </div>
        );
    } 
}