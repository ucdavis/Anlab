import * as React from "react";
import { Checkbox } from "react-toolbox/lib/checkbox";
import { Dialog } from "react-toolbox/lib/dialog";
import Input from "react-toolbox/lib/input";
import { ITestItem } from "./TestList";

interface ITestInfoProps {
    test: ITestItem;
    updateAdditionalInfo: (key: string, value: string) => void;
    value: string;
    onSelection: (test: ITestItem, selected: boolean) => void;
    selected: boolean;
}

interface ITestInfoState {
    internalValue: string;
    active: boolean;
}

export class TestInfo extends React.Component<ITestInfoProps, ITestInfoState> {

    constructor(props) {
        super(props);

        this.state = {
            active: false,
            internalValue: this.props.value,
        };
    }

    public render() {
        const actions = [
            { label: "Cancel", onClick: this._cancelAction },
            { label: "Save", onClick: this._saveAction },
        ];

        return (
            <div>
                <Checkbox checked={this.props.selected} onChange={(e) => this._onSelection(this.props.test, e)} />

                <Dialog
                    actions={actions}
                    active={this.state.active}
                    title={this.props.test.additionalInfoPrompt}
                >
                    <Input
                        type="text"
                        value={this.state.internalValue}
                        onChange={this._onChange}
                        label="Additional Info Required"
                    />
                </Dialog>
            </div>
        );
    }

    private _onSelection = (test: ITestItem, selected: boolean) => {
        if (test.additionalInfoPrompt) {
            this.setState({ active: selected });
        }

        this.props.onSelection(test, selected);
    }

    private _onChange = (v: string) => {
        this.setState({ internalValue: v });
    }

    private _saveAction = () => {
        this.setState({ active: false });
        this.props.updateAdditionalInfo(this.props.test.id, this.state.internalValue);
    }

    private _cancelAction = () => {
        this.setState({ active: false });
        this.props.onSelection(this.props.test, false);
    }
}
