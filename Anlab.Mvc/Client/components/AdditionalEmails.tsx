import * as React from 'react';
import Input from 'react-toolbox/lib/input';
import { Button, IconButton } from 'react-toolbox/lib/button';
import Chip from 'react-toolbox/lib/chip';

interface IAdditionalEmailsProps {
    addedEmails: Array<string>;
    onEmailAdded: Function;
    onDeleteEmail: Function;
    defaultEmail: string;
}

interface IAdditionalEmailsState {
    email: string;
    toggle: boolean;
    hasError: boolean;
    errorText: string;
}



export class AdditionalEmails extends React.Component<IAdditionalEmailsProps, IAdditionalEmailsState> {
    constructor(props) {
        super(props);
        this.state = {
            email: "",
            toggle: false,
            hasError: false,
            errorText: ""
        };
    }

    onEmailChanged = (e) => {
        var newEmail = e.target.value;
        this.setState({ ...this.state, email: newEmail });
    }
    _addEmailAddress = () => {
        const emailtoAdd = this.state.email.toLowerCase();
        
        const re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        if (re.test((emailtoAdd))) {
            if (this.props.addedEmails.indexOf(emailtoAdd) === -1 && emailtoAdd !== this.props.defaultEmail) {
                this.props.onEmailAdded(emailtoAdd);
                this.setState({ ...this.state, email: "", hasError: false, errorText: "", toggle: false});
            } else {
                this.setState({ ...this.state, hasError: true, errorText: "Email already added" });
            }
        } else {
            this.setState({ ...this.state, hasError: true, errorText: "Invalid email" });
        }
    }

    onDelete = (email2Delete: any) => {
        this.props.onDeleteEmail(email2Delete);
    }

    _renderEmails = () => {
        if (this.props.addedEmails.length === 0) {
            return null;
        }

        const emails = this.props.addedEmails.map(item => {
            return (
                <Chip key={item} deletable onDeleteClick={() => this.onDelete(item)} > { item }</Chip>
            );
        });


        return emails;
    }

    handleKeyPress = (e) => {
        if (e.key === 'Enter') {
            if (this.state.email === "")
                this._toggleAddEmail();
            else
                this._addEmailAddress();
        }
    }

    handleBlur = () => {
        if (this.state.email != "") {
            this._addEmailAddress();
        }
        else
            this._toggleAddEmail();
    }

    _toggleAddEmail = () => {
        this.setState(prevState => ({
            email: "", hasError: false, errorText: "", toggle: !prevState.toggle
        }));
        
    }

    render() {
        let inputStyle = {};
        if (this.state.hasError) {
            inputStyle = {
                color: '#de3226'
            }
        }
        return (
            <div className="form_wrap">
                <label className="form_header">Who should be notified for this test?</label>
                <div>
                    <Chip>{this.props.defaultEmail}</Chip>
                    {this._renderEmails()}
                    {!this.state.toggle && <Chip onClick={this._toggleAddEmail} > <i className="emailAddIconStyle fa fa-plus" aria-hidden="true"></i></Chip>}
                    {this.state.toggle &&
                        <Chip>
                        <input className="emailInput" value={this.state.email} style={inputStyle} onChange={this.onEmailChanged} onKeyPress={this.handleKeyPress} onBlur={this.handleBlur} autoFocus={true} />
                        <i className="emailIconStyle fa fa-plus-circle" aria-hidden="true" onClick={this._addEmailAddress} ></i>
                        </Chip>}
                </div>
                <span className="emailError">{this.state.hasError && this.state.errorText}</span>
            </div>
        );
    }
}
