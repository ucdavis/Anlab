import * as React from 'react';
import Input from 'react-toolbox/lib/input';
import { Dialog } from "react-toolbox/lib/dialog";
import { Button } from "react-toolbox/lib/button";
import { ClientIdModalInput } from './ClientIdModalInput';

export interface INewClientInfo {
    employer: string;
    name: string;
    email: string;
    phoneNumber: string;
}

interface IClientIdModalProps {
    clientId: string;
    updateClient: Function;
}

interface IClientIdModalState {
    newClientInfo: INewClientInfo;
    active: boolean;
}

export class ClientIdModal extends React.Component<IClientIdModalProps, IClientIdModalState> {
    constructor(props) {
        super(props);

        this.state = {
            newClientInfo: {
                employer: '',
                name: '',
                email: '',
                phoneNumber: ''
            },
            active: false
        };
    }

    toggleModal = () => {
        this.setState({ ...this.state, active: !this.state.active });
    }

    handleChange = (property: string, value: string) => {
        this.setState({
            ...this.state, newClientInfo: {
                ...this.state.newClientInfo,
                [property]: value
                }
            });
    }

    saveAction = () => {
        this.setState({ ...this.state, active: false });
        this.props.updateClient();
    }

    cancelAction = () => {
        this.setState({ ...this.state, active: false });
    }
    actions = [
        { label: "Cancel", onClick: this.cancelAction },
        { label: "Save", onClick: this.saveAction }
    ];

    render() {
        let title = "Please input the following information";
        return (
            <div>
                <Button className="btn btn-order" onClick={this.toggleModal} >New Client</Button>
                <Dialog
                    actions={this.actions}
                    active={this.state.active}
                    title={title}
                >
                    <label>Name:</label>
                    <ClientIdModalInput property="name" handleChange={this.handleChange} />
                    <label>Employer:</label>
                    <ClientIdModalInput property="employer" handleChange={this.handleChange} />
                    <label>Email:</label>
                    <ClientIdModalInput property="email" handleChange={this.handleChange} />
                    <label>Phone Number:</label>
                    <ClientIdModalInput property="phoneNumber" handleChange={this.handleChange} />
                </Dialog>
            </div>
        );
    }
}