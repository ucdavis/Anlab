import "isomorphic-fetch";
import * as React from "react";
import Input from "./ui/input/input";
import { ClientIdModal, INewClientInfo } from "./ClientIdModal";

interface IClientIdProps {
    clientId: string;
    clientName: string;
    handleChange: (key: string, value: string) => void;
    clientIdRef: (element: HTMLInputElement) => void;
    newClientInfo: INewClientInfo;
    updateNewClientInfo: Function;
}

interface IClientIdInputState {
    error: string;
    newClientInfoAdded: boolean;
}

export class ClientId extends React.Component<IClientIdProps, IClientIdInputState> {

    constructor(props) {
        super(props);

        this.state = {
            error: null,
            newClientInfoAdded: false,
        };
    }

    public render() {
        return (
            <div className="row">
                <div className="col-3">
                        <Input
                            inputRef={this.props.clientIdRef}
                            error={this.state.error}
                            value={this.props.clientId}
                            onChange={this._onChange}
                            placeholder={"Client ID"}
                            label={"Already have Client ID"}
                        />
                        {this.props.clientName}

                    </div>
                    <span className="col-2 t-center align-middle"></span>
                    <div className="col-3">
                    <ClientIdModal clientInfo={this.props.newClientInfo} updateClient={this._updateNewClientInfo} />
                    {(this.state.newClientInfoAdded || (this.props.newClientInfo.name != null && !!this.props.newClientInfo.name.trim())) &&
                            <i className="fa fa-check" aria-hidden="true"></i>}
                    </div>
                </div>
        );
    }

    private _updateNewClientInfo = (info: INewClientInfo) => {
        let error = "";
        let valid = false;
        if (info.name)
        {
            valid = true;
        }
        else if (!this.props.clientId)
        {
            error = "Either a Client ID or New Client Info is required";
        }
        this.setState({ newClientInfoAdded: valid, error: error});
        this.props.updateNewClientInfo(info);
    }

    private _onChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value;
        this._validate(value);
        this.props.handleChange("clientId", value);
        this._lookupClientId(value);
    }

    private _validate = (v: string) => {
        if (v && !!v.trim()) {
            if (v.length < 4)
                this.setState({ error: "Client IDs must be at least 4 characters long" });
            else
                this.setState({ error: "" });
            return;
        }
        if (this.state.newClientInfoAdded) {
            this.setState({ error: "" });
            return;
        }

        this.setState({ error: "Either a Client ID or New Client Info is required" });
    }

    private _lookupClientId = (value) => {
        if (!value || !value.trim() || value.length < 4) {
            this.props.handleChange("clientName", null);
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
                this.setState({ error: null });
                this.props.handleChange("clientName", response.name);
            })
            .catch((error: Error) => {
                this.setState({ error: error.message });
                this.props.handleChange("clientName", null);
            });
    }
}
