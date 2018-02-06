import * as React from "react";
import { Button, Modal } from "react-bootstrap";
import { ClientIdModalInput } from "./ClientIdModalInput";
import { IClientInfo } from "./ClientId";

interface IClientIdModalProps {
    clientInfo: IClientInfo;
    onClose: () => void;
    handleChange: (key: string, value: string) => void;
    disabled: boolean;
    style: string;
}

interface IClientIdModalState {
    active: boolean;
}

export class ClientIdModal extends React.Component<IClientIdModalProps, IClientIdModalState> {
    constructor(props) {
        super(props);

        this.state = {
            active: false,
        };
    }

    toggleModal = () => {
        this.setState({ ...this.state, active: !this.state.active });
    }

    //validate = () => {
    //    const emailre = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    //    const phoneRe = /^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$/;

    //    let valid = (this.state.newClientInfo.employer && !!this.state.newClientInfo.employer.trim()) && (this.state.newClientInfo.name && !!this.state.newClientInfo.name.trim())
    //        && emailre.test((this.state.newClientInfo.email)) && phoneRe.test((this.state.newClientInfo.phoneNumber));

    //    this.setState({ ...this.state, isValid: valid });
    //}

    saveAction = () => {
            this.setState({ active: false });
    }

    cancelAction = () => {
        this.props.onClose();
        this.setState({  active: false });
    }

    clearAction = () => {
        //const clearInfo = {
        //    employer: "",
        //    name: "",
        //    email: "",
        //    phoneNumber: "",
        //};
        //this.setState({
        //    ...this.state, newClientInfo: clearInfo
        //});
    }

    render() {
        let title = "Please input the following information";

        return (
            <span>
                <div>
                    <label>Need a new Client ID</label>
                </div>
                <Button className={this.props.style} onClick={this.toggleModal} disabled={this.props.disabled} > New Client</Button>

                <Modal show={this.state.active} onHide={this.cancelAction} >
                    <Modal.Header closeButton={true}>
                        <Modal.Title>{title}</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <ClientIdModalInput property="name" value={this.props.clientInfo.name} label="Name" handleChange={this.props.handleChange} />
                        <ClientIdModalInput property="employer" value={this.props.clientInfo.employer} label="Employer" handleChange={this.props.handleChange} />
                        <ClientIdModalInput property="email" value={this.props.clientInfo.email} label="Email" handleChange={this.props.handleChange} />
                        <ClientIdModalInput property="phoneNumber" value={this.props.clientInfo.phoneNumber} label="Phone Number" handleChange={this.props.handleChange} />
                    </Modal.Body>
                    <Modal.Footer>
                        <Button className="btn btn-newClient" onClick={this.clearAction} type="reset">Clear</Button>
                        <Button className="btn btn-newClient" onClick={this.cancelAction}>Close</Button>
                    </Modal.Footer>
                </Modal>
            </span>
        );
    }
}
