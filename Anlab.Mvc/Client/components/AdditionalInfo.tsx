
import * as React from 'react';
import Input from 'react-toolbox/lib/input';

interface IAdditionalInfoProps {
    additionalInfo: string;
    handleChange: Function;
}

export class AdditionalInfo extends React.Component<IAdditionalInfoProps, any> {
    render() {
        return (
            <Input type='text' multiline label='Additional Information' maxLength={2000} value={this.props.additionalInfo} onChange={this.props.handleChange.bind(this, 'additionalInfo')} />
        );
    }
}