import * as React from 'react';
import { NumberInput } from './numberInput/numberInput';

interface IQuantityProps {
    quantity?: number;
    onQuantityChanged: Function;
}

export class Quantity extends React.Component<IQuantityProps, any> {
    render() {
        return (
            <NumberInput
                name='quantity'
                label='Quantity'
                value={this.props.quantity}
                onChanged={this.props.onQuantityChanged}
                integer
                min={0}
                max={100}
            />
        );
    }
}
