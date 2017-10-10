
import * as React from 'react';
import Input from 'react-toolbox/lib/input';
import 'isomorphic-fetch';

interface IClientIdProps {
    clientId: string;
    handleChange: Function;
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
            internalValue: this.props.clientId,
            clientName: null,
            error: null
        };
    }

    onChange = (v: string) => {
        this.setState({ ...this.state, internalValue: v });
    }

    onBlur = () => {
        this.props.handleChange('clientId', this.state.internalValue);
        this.lookupClientId();
    }

    lookupClientId = () => {
        if (this.state.internalValue === '') {
            this.setState({ clientName: null, error: null });
            return;
        }
        fetch(`/order/LookupClientId?id=${this.state.internalValue}`, { credentials: 'same-origin' })
            .then(response => {
                if (response === null) {
                    this.setState({ clientName: null,error: "The client id you entered could not be found" });
                    return response;
                }
                return response;
            })
            .then(response => response.json())
            .then(response => {
                this.setState({ clientName: response.name,  error: null });
            })
            .catch(error => {
                this.setState({ clientName: null, error: "The client id you entered could not be found" });
            });

    }
    
    render() {
        return (
            <div>
                <Input type='text' onBlur={this.onBlur} error={this.state.error} value={this.state.internalValue} onChange={this.onChange} label='Client Id' />
                {this.state.clientName}
            </div>

    );
}
}