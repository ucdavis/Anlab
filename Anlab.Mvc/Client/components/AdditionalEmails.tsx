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
    error: boolean;
    errorText: string;
}



export class AdditionalEmails extends React.Component<IAdditionalEmailsProps, IAdditionalEmailsState> {
    constructor(props) {
        super(props);
        this.state = {
            email: "",
            toggle: false,
            error: false,
            errorText: ""
        };
    }

    onEmailChanged = (e) => {
        var newEmail = e.target.value;
        this.setState({ ...this.state, email: newEmail });
    }
    onClick = () => {
        const emailtoAdd = this.state.email.toLowerCase();
        
        const re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        if (re.test((emailtoAdd))) {
            if (this.props.addedEmails.indexOf(emailtoAdd) === -1 && emailtoAdd !== this.props.defaultEmail) {
                this.props.onEmailAdded(emailtoAdd);
                this.setState({ ...this.state, email: "", error: false, errorText: "", toggle: false});
            } else {
                this.setState({ ...this.state, error: true, errorText: "Email already added" });
            }
        } else {
            this.setState({ ...this.state, error: true, errorText: "Invalid email" });
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
                this.onClick();
        }
    }

    handleBlur = () => {
        if (this.state.email != "") {
            this.onClick();
        }
        else
            this._toggleAddEmail();
    }

    _toggleAddEmail = () => {
        this.setState(prevState => ({
            email: "", error: false, errorText: "", toggle: !prevState.toggle
        }));
        
    }

    render() {
        let inputStyle = {};
        if (this.state.error) {
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
                        <i className="emailIconStyle fa fa-plus-circle" aria-hidden="true" onClick={this.onClick} ></i>
                        </Chip>}
                </div>
                <span className="emailError">{this.state.error && this.state.errorText}</span>
            </div>
        );
    }
}
