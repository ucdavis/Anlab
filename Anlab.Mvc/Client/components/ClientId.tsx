import "isomorphic-fetch";
import * as React from "react";
import Input from "./ui/input/input";
import { ClientIdModal, INewClientInfo } from "./ClientIdModal";

interface IClientIdProps {
    clientId: string;
    handleChange: (key: string, value: string) => void;
    clientIdRef: (element: HTMLInputElement) => void;
    newClientInfo: INewClientInfo;
    updateNewClientInfo: Function;
}

interface IClientIdInputState {
    internalValue: string;
    clientName: string;
    error: string;
    newClientInfoAdded: boolean;
}

export class ClientId extends React.Component<IClientIdProps, IClientIdInputState> {

    constructor(props) {
        super(props);

        this.state = {
            clientName: null,
            error: null,
            internalValue: this.props.clientId,
            newClientInfoAdded: false,
        };
    }

    public render() {
        return (
                <div className="row">
                    <div className="col-4">
                        <Input
                            inputRef={this.props.clientIdRef}
                            onBlur={this._onBlur}
                            error={this.state.error}
                            value={this.state.internalValue}
                            onChange={this._onChange}
                        />
                        {this.state.clientName}

                    </div>
                    <div>
                    <ClientIdModal clientInfo={this.props.newClientInfo} updateClient={this._updateNewClientInfo} />
                    {(this.state.newClientInfoAdded || (this.props.newClientInfo.name != null && !!this.props.newClientInfo.name.trim())) &&
                            <i className="fa fa-check" aria-hidden="true"></i>}
                    </div>
                </div>
        );
    }

    private _updateNewClientInfo = (info: INewClientInfo) => {
        if (info.name)
        {
            this.setState({ newClientInfoAdded: true, error: "" });
        }
        else
        {
            this.setState({ newClientInfoAdded: false });
        }
        this.props.updateNewClientInfo(info);
    }

    private _onChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value;
        this._validate(value);
        this.setState({ internalValue: value });
    }

    private _onBlur = () => {
        const internalValue = this.state.internalValue;
        this._validate(internalValue);
        this.props.handleChange("clientId", internalValue);
        this._lookupClientId();
    }

    private _validate = (v: string) => {
        if (v) {
            this.setState({ error: "" });
            return;
        }
        if (this.state.newClientInfoAdded) {
            this.setState({ error: "" });
            return;
        }

        this.setState({ error: "Either a Client ID or New Client Info is required" });
    }

    private _lookupClientId = () => {
        if (!this.state.internalValue || !this.state.internalValue.trim()) {
            this.setState({ clientName: null });
            return;
        }

        fetch(`/order/LookupClientId?id=${this.state.internalValue}`, { credentials: "same-origin" })
            .then((response) => {
                if (response === null || response.status !== 200) {
                  throw new Error("The client id you entered could not be found");
                }
                return response;
            })
            .then((response) => response.json())
            .then((response) => {
                this.setState({ clientName: response.name, error: null });
            })
            .catch((error: Error) => {
                this.setState({ clientName: null, error: error.message });
            });
    }
}
