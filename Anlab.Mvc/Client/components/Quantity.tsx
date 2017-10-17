import * as React from "react";
import { NumberInput } from "./numberInput/numberInput";

interface IQuantityProps {
    quantity?: number;
    onQuantityChanged: () => void;
    quantityRef: any;
}

export class Quantity extends React.Component<IQuantityProps, {}> {
    public render() {
        return (
            <NumberInput
                numberRef={this.props.quantityRef}
                name="quantity"
                label="Quantity"
                value={this.props.quantity}
                onChanged={this.props.onQuantityChanged}
                integer={true}
                min={1}
                max={100}
                required={true}
            />
        );
    }
}
