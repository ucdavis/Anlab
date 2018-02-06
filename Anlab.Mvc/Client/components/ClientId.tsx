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
    clientName: string;
    handleChange: (key: string, value: string) => void;
    clearClientInfo: () => void;
    clientIdRef: (element: HTMLInputElement) => void;
    clientInfo: IClientInfo;
}

interface IClientIdInputState {
    error: string;
    newClientInfoAdded: boolean;
    fetchedName: string;
}

export class ClientId extends React.Component<IClientIdProps, IClientIdInputState> {

    private _modalError = "There are some errors with the information you provided";
    private _clientIdError = ""

    constructor(props) {
        super(props);

        this.state = {
            error: null,
            newClientInfoAdded: false,
            fetchedName: "",
        };
    }

    public render() {
        let style = this.state.newClientInfoAdded ? "btn" : "btn-newClient";
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
                            disabled={this.state.newClientInfoAdded}
                        />
                        {this.props.clientInfo.clientId ? this.state.fetchedName : ""}

                    </div>
                    <span className="col-2 t-center align-middle"></span>
                    <div className="col-3">
                    <ClientIdModal
                        clientInfo={this.props.clientInfo}
                        handleChange={this._handleChange}
                        onClear={this._onClear}
                        disabled={this.props.clientInfo.clientId != ""}
                        style={style}
                        onClose={this._onModalClose} />
                    {(this.state.newClientInfoAdded && this.state.error == "") &&
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
        this._lookupClientId(value);
        this._handleChange("clientId", value);

    }

    private _handleChange = (property: string, value: string) => {
        this.props.handleChange(property, value);
    }

    private _onBlur = () => {
        this.props.handleChange("name", this.state.fetchedName);
    }

    private _onModalClose = () => {
        this._validate();
    }

    private _onClear = () => {
        this.props.clearClientInfo();
    }

    private _validate = () => {
        if (this.props.clientInfo.clientId && !!this.props.clientInfo.clientId.trim() && this.props.clientInfo.name)
        {
            this.setState({ error: "", newClientInfoAdded: false });
            return;
        }
        const emailre = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        const phoneRe = /^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$/;

        let valid = (this.props.clientInfo.employer && !!this.props.clientInfo.employer.trim()) && (this.props.clientInfo.name && !!this.props.clientInfo.name.trim())
            && emailre.test((this.props.clientInfo.email)) && phoneRe.test((this.props.clientInfo.phoneNumber));
        if (!valid) {
            this.setState({ error: this._modalError, newClientInfoAdded: false });
        }
        else {
            this.setState({ error: "", newClientInfoAdded: true});
        }



    }

    private _lookupClientId = (value) => {
        if (!value || !value.trim()) {
            //this.props.handleChange("name", "");
            this.setState({ error: "Either a Client ID or New Client Info is required", fetchedName: "" });
            return;
        }
        if (value.length < 4) {
            //this.props.handleChange("name", "");
            this.setState({ error: "Client IDs must be at least 4 characters long", fetchedName: "" });
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
                this.setState({ error: null, fetchedName: response.name });
                //this.props.handleChange("name", response.name);
            })
            .catch((error: Error) => {
                this.setState({ error: error.message, fetchedName: "" });
                //this.props.handleChange("name", null);
            });
    }
}
