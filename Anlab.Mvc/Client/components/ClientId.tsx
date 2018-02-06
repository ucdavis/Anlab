import "isomorphic-fetch";
import * as React from "react";
import Input from "./ui/input/input";
import { ClientIdModal } from "./ClientIdModal";

export interface IClientInfo {
    clientId?: string;
    employer: string;
    name: string;
    email: string;
    phoneNumber: string;
}

interface IClientIdProps {
    //change one property of parent ClientInfo state
    handleClientInfoChange: (key: string, value: string) => void;
    //change if parent thinks ClientInfo is valid
    updateClientInfoValid: (key: string, value: any) => void;
    //clear all 
    clearClientInfo: () => void;
    clientIdRef: (element: HTMLInputElement) => void;
    clientInfo: IClientInfo;
}

interface IClientIdInputState {
    error: string;
    modalValid: boolean;
    fetchedName: string;
}

export class ClientId extends React.Component<IClientIdProps, IClientIdInputState> {

    private _modalError = "There are some errors with the new client information you provided";

    constructor(props) {
        super(props);

        this.state = {
            error: null,
            modalValid: false,
            fetchedName: "",
        };
    }

    public render() {
        //if modal filled properly, set button style to filled it
        let style = this.state.modalValid ? "btn" : "btn-newClient";
        //set border red if modal has error
        if (this.state.error == this._modalError)
            style += " btn-error";
        return (
            <div className="row">
                <div className="col-3">
                        <Input
                            inputRef={this.props.clientIdRef}
                            error={this.state.error}
                            value={this.props.clientInfo.clientId}
                            onChange={this._onClientIdChange}
                            onBlur={this._onBlur}
                            placeholder={"Client ID"}
                            label={"Already have Client ID"}
                            disabled={this.state.modalValid}
                        />
                        {this.props.clientInfo.clientId ? this.state.fetchedName : ""}

                    </div>
                    <span className="col-2 t-center align-middle"></span>
                    <div className="col-3">
                    <ClientIdModal
                        clientInfo={this.props.clientInfo}
                        handleChange={this.props.handleClientInfoChange}
                        onClear={this.props.clearClientInfo}
                        disabled={this.props.clientInfo.clientId != ""}
                        style={style}
                        onClose={this._onModalClose} />
                    {(this.state.modalValid && this.state.error == "") &&
                        <i className="fa fa-check" aria-hidden="true"></i>}
                    {(this.state.error == this._modalError) &&
                        <i className="fa fa-times" aria-hidden="true"></i>}

                    </div>
                </div>
        );
    }

    private _onClientIdChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        let value = e.target.value;
        if (value) {
            value = value.toUpperCase();
        }
        this.props.handleClientInfoChange("clientId", value);
        this._lookupAndValidateClientId(value);
    }

    private _onBlur = () => {
        this.props.handleClientInfoChange("name", this.state.fetchedName);
    }

    private _onModalClose = () => {
        this._validateModal();
    }

    //validate modal contents via props, not client id
    private _validateModal = () => {
        const emailre = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        const phoneRe = /^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$/;

        let valid = (this.props.clientInfo.employer && !!this.props.clientInfo.employer.trim()) && (this.props.clientInfo.name && !!this.props.clientInfo.name.trim())
            && emailre.test((this.props.clientInfo.email)) && phoneRe.test((this.props.clientInfo.phoneNumber));
        if (!valid) {
            this.setState({ error: this._modalError, modalValid: false });
            this.props.updateClientInfoValid("clientInfoValid", false);
        }
        else {
            this.setState({ error: "", modalValid: true });
            this.props.updateClientInfoValid("clientInfoValid", true);
        }
    }

    //validate client id, not modal. this is because we are validating the string itself, not the props
    private _lookupAndValidateClientId = (value) => {
        if (!value || !value.trim()) {
            this.setState({ error: "Either a Client ID or New Client Info is required", fetchedName: "" });
            this.props.updateClientInfoValid("clientInfoValid", false);
            return;
        }
        if (value.length < 4) {
            this.setState({ error: "Client IDs must be at least 4 characters long", fetchedName: "" });
            this.props.updateClientInfoValid("clientInfoValid", false);
            return;
        }
        fetch(`/order/LookupClientId?id=${value.trim()}`, { credentials: "same-origin" })
            .then((response) => {
                if (response === null || response.status !== 200) {
                    throw new Error("The client id you entered could not be found");
                }
                return response;
            })
            .then((response) => response.json())
            .then((response) => {
                this.setState({ error: "", fetchedName: response.name });
                this.props.updateClientInfoValid("clientInfoValid", true);
            })
            .catch((error: Error) => {
                this.setState({ error: error.message, fetchedName: "" });
                this.props.updateClientInfoValid("clientInfoValid", false);
            });
    }
}
