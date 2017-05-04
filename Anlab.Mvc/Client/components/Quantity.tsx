import * as React from 'react';
import Input from 'react-toolbox/lib/input';

interface IQuantityProps {
    quantity?: number;
    onQuantityChanged: Function;
}

export class Quantity extends React.Component<IQuantityProps, any> {
    onChanged = (value: string) => {
        const numValue = Number(value);

        if (isNaN(numValue)) {
            // do nothing
        } else {
            this.props.onQuantityChanged(Number(value));
        }
    }
    render() {
        return (
            <Input type='text' label='Number' name='quantity' required value={this.props.quantity} onChange={this.onChanged} />
        );
    }
}