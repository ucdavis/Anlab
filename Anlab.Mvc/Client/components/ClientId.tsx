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
}

export class ClientId extends React.Component<IClientIdProps, IClientIdInputState> {

    constructor(props) {
        super(props);

        this.state = {
            clientName: null,
            error: null,
            internalValue: this.props.clientId,
        };
    }

    public render() {
        return (
            <div>
                <div className="row">
                    <div className="col-4">
                <Input
                    inputRef={this.props.clientIdRef}
                    onBlur={this._onBlur}
                    error={this.state.error}
                    value={this.state.internalValue}
                    onChange={this._onChange}
                />
                </div>
                <div className="flexcol">
                    <ClientIdModal clientInfo={this.props.newClientInfo} updateClient={this.props.updateNewClientInfo} />
                </div>
            </div>
            <div className="row">
              {this.state.clientName}
            </div>
            </div>

        );
    }

    private _onChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const value = e.target.value;
        this.setState({ internalValue: value });
    }

    private _onBlur = () => {
        const internalValue = this.state.internalValue;
        this._validate(internalValue);
        this.props.handleChange("clientId", internalValue);
        this._lookupClientId();
    }

    private _validate = (v: string) => {
        let error = null;
        if (v.trim() === "") {
            error = "Either a Client ID or New Client Info is required";
        }

        this.setState({ error });
    }

    private _lookupClientId = () => {
        if (this.state.internalValue === "") {
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
