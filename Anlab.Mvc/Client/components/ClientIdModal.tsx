import * as React from "react";
import { Button, Modal } from "react-bootstrap";
import { ClientIdModalInput } from "./ClientIdModalInput";
import { IClientInfo } from "./ClientId";

interface IClientIdModalProps {
    clientInfo: IClientInfo;
    onClose: () => void;
    onClear: () => void;
    handleChange: (keys: string[], values: string[]) => void;
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

    closeAction = () => {
        this.props.onClose();
        this.setState({  active: false });
    }

    clearAction = () => {
        this.props.onClear();
    }

    render() {
        let title = "Please input the following information";

        return (
            <span>
                <div>
                    <label>Need a new Client ID</label>
                </div>
                <Button className={this.props.style} onClick={this.toggleModal} disabled={this.props.disabled} > New Client</Button>

                <Modal show={this.state.active} onHide={this.closeAction} >
                    <Modal.Header closeButton={true}>
                        <Modal.Title>{title}</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <ClientIdModalInput property="name" value={this.props.clientInfo.name} label="Name" handleChange={this.props.handleChange} />
                        <ClientIdModalInput property="employer" value={this.props.clientInfo.employer} label="Employer" handleChange={this.props.handleChange} />
                        <ClientIdModalInput property="email" value={this.props.clientInfo.email} label="Client Email" handleChange={this.props.handleChange} />
                        <ClientIdModalInput property="phoneNumber" value={this.props.clientInfo.phoneNumber} label="Phone Number" handleChange={this.props.handleChange} />
                    </Modal.Body>
                    <Modal.Footer>
                        <Button className="btn btn-newClient" onClick={this.clearAction} type="reset">Clear</Button>
                        <Button className="btn" onClick={this.closeAction}>Save</Button>
                    </Modal.Footer>
                </Modal>
            </span>
        );
    }
}
