import * as React from 'react';
import Input from 'react-toolbox/lib/input';
import { Button, IconButton } from 'react-toolbox/lib/button';

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
                <div key={item}>{item} <Button label="Delete" primary onClick={() => this.onDelete(item)}/></div>
            );
        });


        return emails;
    }

    render() {
        return (
            <div className="form_wrap">
                <label className="form_header">Should anyone else be notified for this test?</label>
                {this._renderEmails()}
                <Input type='text' value={this.state.email} label='Email To Add' maxLength={50} onChange={this.onEmailChanged}/>
                <Button label='Add this' flat primary onClick={this.onClick} />
            </div>
        );
    }
}
