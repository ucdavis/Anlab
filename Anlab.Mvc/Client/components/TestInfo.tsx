import * as React from 'react';
import Input from 'react-toolbox/lib/input';
import { Dialog } from "react-toolbox/lib/dialog";
import { ITestItem } from './TestList';
import { Checkbox } from "react-toolbox/lib/checkbox";

interface ITestInfoProps {
    test: ITestItem;
    updateAdditionalInfo: Function;
    value: string;
    onSelection: Function;
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
            internalValue: this.props.value,
            active: false
        };
    }

    onSelection = (test: ITestItem, e) => {
        const selected = e;

        this.setState({ ...this.state, active: selected });

        this.props.onSelection(test, selected);
    }

    onChange = (v: string) => {
        this.setState({ ...this.state, internalValue: v });
    }

    saveAction = () => {
        this.setState({ ...this.state, active: false });
        this.props.updateAdditionalInfo(this.props.test.id, this.state.internalValue);
    }

    cancelAction = () => {
        this.setState({ ...this.state, active: false });
        this.props.onSelection(this.props.test, false);
    }
    actions = [
        { label: "Cancel", onClick: this.cancelAction },
        { label: "Save", onClick: this.saveAction }
    ];

    render() {
        return (
            <div>
                <Checkbox checked={this.props.selected} onChange={e => this.onSelection(this.props.test, e)} />

                <Dialog
                    actions={this.actions}
                    active={this.state.active}
                    title={this.props.test.additionalInfoPrompt}
                >
                    <Input type='text' value={this.state.internalValue} onChange={this.onChange} label='Additional Info Required' />
                </Dialog>
            </div>
        );
    }
}