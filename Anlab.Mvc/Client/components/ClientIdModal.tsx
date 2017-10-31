import * as React from "react";
import Input from "react-toolbox/lib/input";
import { Dialog } from "react-toolbox/lib/dialog";
import { Button } from "react-toolbox/lib/button";
import { ClientIdModalInput } from "./ClientIdModalInput";

export interface INewClientInfo {
    employer: string;
    name: string;
    email: string;
    phoneNumber: string;
}

interface IClientIdModalProps {
    clientInfo: INewClientInfo;
    updateClient: Function;
}

interface IClientIdModalState {
    newClientInfo: INewClientInfo;
    active: boolean;
    isValid: boolean;
}

export class ClientIdModal extends React.Component<IClientIdModalProps, IClientIdModalState> {
    constructor(props) {
        super(props);

        this.state = {
            newClientInfo: { ...this.props.clientInfo },
            active: false,
            isValid: false
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
        }, this.validate);
    }

    validate = () => {
        const emailre = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        const phoneRe = /^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$/;

        let valid = (this.state.newClientInfo.employer && !!this.state.newClientInfo.employer.trim()) && (this.state.newClientInfo.name && !!this.state.newClientInfo.name.trim())
            && emailre.test((this.state.newClientInfo.email)) && phoneRe.test((this.state.newClientInfo.phoneNumber));

        this.setState({ ...this.state, isValid: valid });
    }

    saveAction = () => {
        if (this.state.isValid)
        {
            this.setState({ ...this.state, active: false });
            this.props.updateClient(this.state.newClientInfo);
        }
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
        const errorText = "Please correct any errors and complete any required fields before you proceed";

        return (
            <div>
                <Button className="btn btn-order" onClick={this.toggleModal} >New Client</Button>
                <Dialog
                    actions={this.actions}
                    active={this.state.active}
                    title={title}
                >
                    <ClientIdModalInput property="name" value={this.state.newClientInfo.name} label="Name" handleChange={this.handleChange} />
                    <ClientIdModalInput property="employer" value={this.state.newClientInfo.employer} label="Employer" handleChange={this.handleChange} />
                    <ClientIdModalInput property="email" value={this.state.newClientInfo.email} label="Email" handleChange={this.handleChange} />
                    <ClientIdModalInput property="phoneNumber" value={this.state.newClientInfo.phoneNumber} label="Phone Number" handleChange={this.handleChange} />
                    {!this.state.isValid ? <div className="alert alert-danger">{errorText}</div> : null}
                </Dialog>
            </div>
        );
    }
}
