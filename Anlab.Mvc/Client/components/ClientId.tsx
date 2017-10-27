import "isomorphic-fetch";
import * as React from "react";
import Input from "react-toolbox/lib/input";

interface IClientIdProps {
    clientId: string;
    handleChange: (key: string, value: string) => void;
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
                <Input
                    type="text"
                    onBlur={this._onBlur}
                    error={this.state.error}
                    value={this.state.internalValue}
                    onChange={this._onChange}
                    label="Client Id"
                />
                {this.state.clientName}
            </div>

        );
    }

    private _onChange = (v: string) => {
        this.setState({ internalValue: v });
    }

    private _onBlur = () => {
        this.props.handleChange("clientId", this.state.internalValue);
        this._lookupClientId();
    }

    private _lookupClientId = () => {
        if (this.state.internalValue === "") {
            this.setState({ clientName: null, error: null });
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
                this.setState({ clientName: response.name,  error: null });
            })
            .catch((error: Error) => {
                this.setState({ clientName: null, error: error.message });
            });
    }
}
