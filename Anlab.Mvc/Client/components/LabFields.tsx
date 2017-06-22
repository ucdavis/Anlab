import * as React from 'react';
import Input from 'react-toolbox/lib/input';
import { NumberInput } from './numberInput/numberInput';

interface ILabFieldsProps {
    isFromLab: boolean;
    labComments: string;
    adjustmentAmount: number;
    handleChange: Function;
}

export class LabFields extends React.Component<ILabFieldsProps, any> {
    render() {
        if (!this.props.isFromLab) {
            return null;
        }
        return (
            <div>
                <Input type='text' multiline label='Lab Comments' maxLength={2000} value={this.props.labComments} onChange={this.props.handleChange.bind(this, 'labComments')} />
                <NumberInput
                    name='adjustmentAmount'
                    label='Adjustment Amount'
                    value={this.props.adjustmentAmount}
                    onChanged={this.props.handleChange.bind(this, 'adjustmentAmount')}                    
                />
            </div>
        );
    }
}