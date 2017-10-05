import * as React from 'react';
import Input from 'react-toolbox/lib/input';
import { Dialog } from "react-toolbox/lib/dialog";
import { ITestItem } from './TestList';

interface ITestInfoProps {
    testId: string;
    prompt: string;
    updateAdditionalInfo: Function;
}

interface ITestInfoState {
    internalValue: string;
    active: boolean;
}

export class TestInfo extends React.Component<ITestInfoProps, ITestInfoState> {

    constructor(props) {
        super(props);

        this.state = {
            internalValue: '',
            active: true
        };
    }

    onChange = (v: string) => {
        this.setState({ ...this.state, internalValue: v });
    }

    //onBlur = () => {
    //    this.props.handleChange('clientId', this.state.internalValue);
    //}

    toggleModal = () => {
        this.setState({ ...this.state, active: false });
        this.props.updateAdditionalInfo(this.props.testId, this.state.internalValue);
    }

    actions = [
        { label: "Save", onClick: this.toggleModal }
    ];

    render() {
        return (
            <Dialog
                actions={this.actions}
                active={this.state.active}
                title={this.props.prompt}
            >
                <Input type='text' value={this.state.internalValue} onChange={this.onChange} label='Additional Info Required' />
            </Dialog>
        );
    }
}