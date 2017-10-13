import * as React from "react";
import Input from "react-toolbox/lib/input";

interface ICommodityProps {
    commodity: string;
    handleChange: (key: string, value: string) => void;
}

interface ICommodityState {
    internalValue: string;
}

export class Commodity extends React.Component<ICommodityProps, any> {
    constructor(props) {
        super(props);

        this.state = {
            internalValue: this.props.commodity,
        };
    }

    public render() {
        return (
            <Input
              type="text"
              label="Commodity"
              value={this.state.internalValue}
              onChange={this._onChange}
              onBlur={this._onBlur}
            />
        );
    }

    private _onChange = (v: string) => {
        this.setState({ internalValue: v });
    }

    private _onBlur = () => {
        this.props.handleChange("commodity", this.state.internalValue);
    }
}
