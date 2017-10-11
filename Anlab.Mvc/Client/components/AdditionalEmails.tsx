﻿import * as React from "react";

// ui
import { Button, IconButton } from "react-toolbox/lib/button";
import Chip from "react-toolbox/lib/chip";
import Input from "react-toolbox/lib/input";

import { validateEmail } from "../util/email";

interface IAdditionalEmailsProps {
    addedEmails: string[];
    onEmailAdded: (email: string) => void;
    onDeleteEmail: (email: string) => void;
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
            errorText: "",
            hasError: false,
            toggle: false,
        };
    }

    public render() {
        return (
            <div className="form_wrap">
                <label className="form_header">Who should be notified for this test?</label>
                <div>
                    <Chip>{this.props.defaultEmail}</Chip>
                    {this._renderEmails()}
                    {this._renderInput()}
                </div>
                {this._renderError()}
            </div>
        );
    }

    private _renderEmails = () => {
        if (this.props.addedEmails.length === 0) {
            return null;
        }

        return this.props.addedEmails.map((item) => {
            return (
                <Chip key={item} deletable={true} onDeleteClick={this._onDelete.bind(this, item)}>
                  {item}
                </Chip>
            );
        });
    }

    private _renderInput = () => {
      if (!this.state.toggle) {
        return (
          <Chip onClick={this._toggleAddEmail}>
            <i className="emailAddIconStyle fa fa-plus" aria-hidden="true" />
          </Chip>
        );
      }

      let inputStyle = {};
      if (this.state.hasError) {
          inputStyle = {
              color: "#de3226",
          };
      }

      return (
        <Chip>
          <input
            className="emailInput"
            value={this.state.email}
            style={inputStyle}
            onChange={this._onEmailChanged}
            onKeyPress={this._handleKeyPress}
            onBlur={this._handleBlur}
            autoFocus={true}
          />
          <i
            className="emailIconStyle fa fa-plus-circle"
            aria-hidden="true"
            onClick={this._addEmailAddress}
          />
        </Chip>
      );
    }

    private _renderError = () => {
      if (!this.state.hasError || !this.state.errorText) {
        return null;
      }

      return <span className="text-danger emailError">{this.state.errorText}</span>;
    }

    private _onEmailChanged = (e) => {
        const newEmail = e.target.value;
        this.setState({ ...this.state, email: newEmail });
    }

    private _addEmailAddress = () => {
        const emailtoAdd = this.state.email.toLowerCase();

        // check for valid email address
        if (!validateEmail(emailtoAdd)) {
          this.setState({
            ...this.state,
            errorText: "Invalid email",
            hasError: true,
          });
          return;
        }

        // check for duplicate email
        if (emailtoAdd === this.props.defaultEmail || this.props.addedEmails.indexOf(emailtoAdd) >= 0) {
          this.setState({ ...this.state, hasError: true, errorText: "Email already added" });
          return;
        }

        this.props.onEmailAdded(emailtoAdd);
        this.setState({
          ...this.state,
          email: "",
          errorText: "",
          hasError: false,
          toggle: false,
        });
    }

    private _onDelete = (email2Delete: any) => {
        this.props.onDeleteEmail(email2Delete);
    }

    private _handleKeyPress = (e) => {
        if (e.key === "Enter") {
            if (this.state.email === "") {
                this._toggleAddEmail();
            } else {
                this._addEmailAddress();
            }
        }
    }

    private _handleBlur = () => {
        if (this.state.email !== "") {
            this._addEmailAddress();
        } else {
            this._toggleAddEmail();
        }
    }

    private _toggleAddEmail = () => {
        this.setState((prevState) => ({
            email: "",
            errorText: "",
            hasError: false,
            toggle: !prevState.toggle,
        }));
    }
}
