import * as React from "react";
import { NumberInput } from "./ui/numberInput/numberInput";

interface IQuantityProps {
    quantity?: number;
    onQuantityChanged: (value: number) => void;
    quantityRef: (element: HTMLInputElement) => void;
}

export class Quantity extends React.Component<IQuantityProps, {}> {
    public render() {
        return (
            <NumberInput
                name="quantity"
                label="Quantity"
                value={this.props.quantity}
                onChange={this.props.onQuantityChanged}
                integer={true}
                min={1}
                max={100}
                required={true}
                inputRef={this.props.quantityRef}
            />
        );
    }
}
