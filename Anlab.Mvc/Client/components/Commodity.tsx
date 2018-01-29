import * as React from "react";
import Input from "./ui/input/input";

interface ICommodityProps {
    commodity: string;
    handleChange: (key: string, value: string) => void;
}

interface ICommodityState {
    internalValue: string;
}

export class Commodity extends React.Component<ICommodityProps, ICommodityState> {
    constructor(props) {
        super(props);

        this.state = {
            internalValue: this.props.commodity,
        };
    }

    public render() {
        return (
            <Input
                label="Type of Material / Commodity"
              value={this.state.internalValue}
              onChange={this._onChange}
              onBlur={this._onBlur}
            />
        );
    }

    private _onChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        this.setState({ internalValue: e.target.value });
    }

    private _onBlur = () => {
        this.props.handleChange("commodity", this.state.internalValue);
    }
}
