import * as React from "react";
import { Checkbox } from "react-bootstrap";
import { Dialog } from "react-toolbox/lib/dialog";
import Input from "./ui/input/input";
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

export class TestInfo extends React.PureComponent<ITestInfoProps, ITestInfoState> {

    constructor(props) {
        super(props);

        this.state = {
            active: false,
            internalValue: ""
        };
    }

    public render() {
        const actions = [
            { label: "Cancel", onClick: this._cancelAction },
            { label: "Save", onClick: this._saveAction },
        ];

        return (
            <div>
                <Checkbox checked={this.props.selected} onChange={(e) => this._onSelection(this.props.test)} />

                <Dialog
                    actions={actions}
                    active={this.state.active}
                    title={this.props.test.additionalInfoPrompt}
                >
                    <Input
                        value={this.state.internalValue}
                        onChange={this._onChange}
                        label="Additional Info Required"
                    />
                </Dialog>
            </div>
        );
    }

    private _onSelection = (test: ITestItem) => {
        if (test.additionalInfoPrompt) {
            this.setState({ active: !this.props.selected });
        }

        this.props.onSelection(test, !this.props.selected);
    }

    private _onChange = (e) => {
        const v = e.target.value;
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
