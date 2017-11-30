import * as React from "react";
import { Modal, Button } from "react-bootstrap";
import FancyCheckbox from "./ui/fancyCheckbox/fancyCheckbox";
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

        return (
            <div>
                <FancyCheckbox
                    checked={this.props.selected}
                    label={this.props.test.analysis}
                    onChange={(e) => this._onSelection(this.props.test)}
                />
                <Modal show={this.state.active}>
                    <Modal.Header>
                        <Modal.Title>{this.props.test.additionalInfoPrompt}</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <Input
                            value={this.state.internalValue}
                            onChange={this._onChange}
                            label="Additional Info Required"
                        />
                    </Modal.Body>
                    <Modal.Footer>
                        <Button onClick={this._cancelAction}>Cancel</Button>
                        <Button onClick={this._saveAction}>Save</Button>
                    </Modal.Footer>
                </Modal>
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
        this.props.updateAdditionalInfo(this.props.test.id, null);
        this.props.onSelection(this.props.test, false);
    }
}
