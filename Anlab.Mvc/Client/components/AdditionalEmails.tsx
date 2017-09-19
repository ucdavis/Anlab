import * as React from 'react';
import Input from 'react-toolbox/lib/input';
import { Button, IconButton } from 'react-toolbox/lib/button';
import Chip from 'react-toolbox/lib/chip';

interface IAdditionalEmailsProps {
    addedEmails: Array<string>;
    onEmailAdded: Function;
    onDeleteEmail: Function;
}

interface IAdditionalEmailsState {
    email: string;
}



export class AdditionalEmails extends React.Component<IAdditionalEmailsProps, IAdditionalEmailsState> {
    constructor(props) {
        super(props);
        this.state = {
            email: ""
        };
    }

    onEmailChanged = (email: string) => {
        this.setState({ ...this.state, email });
    }
    onClick = () => {
        const emailtoAdd = this.state.email.toLowerCase();
        
        const re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        if (re.test((emailtoAdd))) {            
            if (this.props.addedEmails.indexOf(emailtoAdd) === -1) {
                this.props.onEmailAdded(emailtoAdd);
            } else {
                alert("Email already added");
            }
            this.setState({ ...this.state, email: "" });
        } else {
            alert("Invalid email");
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
            this.onClick();
        }
    }

    handleBlur = () => {
        if (this.state.email != "")
            this.onClick();
    }

    _renderAddButton = () => {
        if (this.state.email.length > 0) {
            return (<Button label='Add Email' flat primary onClick={this.onClick} />);
        } else {
            return null;
        }
        
    }

    render() {
        return (
            <div className="form_wrap">
                <label className="form_header">Should anyone else be notified for this test?</label>
                <div>
                    {this._renderEmails()}
                </div>
                <Input type='text' value={this.state.email} label='Email To Add' maxLength={50} onChange={this.onEmailChanged} onKeyPress={this.handleKeyPress} onBlur={this.handleBlur} />
                {this._renderAddButton()}
            </div>
        );
    }
}
