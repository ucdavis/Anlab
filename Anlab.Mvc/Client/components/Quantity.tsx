import * as React from "react";
import { IntegerInput } from "./ui/integerInput/integerInput";

interface IQuantityProps {
    quantity?: number;
    onQuantityChanged: (value: number) => void;
    quantityRef: (element: HTMLInputElement) => void;
}

export class Quantity extends React.Component<IQuantityProps, {}> {
    public render() {
        return (
            <IntegerInput
                name="quantity"
                label="Quantity"
                value={this.props.quantity}
                onChange={this.props.onQuantityChanged}
                min={1}
                max={100}
                required={true}
                inputRef={this.props.quantityRef}
            />
        );
    }
}
