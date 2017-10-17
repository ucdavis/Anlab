import * as React from "react";
import Input from "react-toolbox/lib/input";

interface IAdditionalInfoProps {
    additionalInfo: string;
    handleChange: (key: string, value: string) => void;
}

interface IAdditionalInfoState {
    internalValue: string;
}

export class AdditionalInfo extends React.Component<IAdditionalInfoProps, IAdditionalInfoState> {
    constructor(props) {
        super(props);

        this.state = {
            internalValue: this.props.additionalInfo,
        };
    }

    public render() {
        return (
            <Input
              type="text"
              multiline={true}
              label="Additional Information"
              maxLength={2000}
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
        this.props.handleChange("additionalInfo", this.state.internalValue);
    }
}
