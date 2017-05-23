import * as React from 'react';
import Checkbox from 'react-toolbox/lib/checkbox';

interface IGrindProps {
    grind: boolean;
    handleChange: Function;
    sampleType: string;
}


export class Grind extends React.Component<IGrindProps, any> {

    render() {
        if (!(this.props.sampleType === "Soil" || this.props.sampleType === "Plant")) {
            return null;
        }
        return (
            <div>
                <Checkbox checked={this.props.grind} label="Grind Samples" onChange={this.props.handleChange.bind(this, 'grind')}/>
            </div>
        );
    } 
}