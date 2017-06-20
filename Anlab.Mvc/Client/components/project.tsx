import * as React from 'react';
import Input from 'react-toolbox/lib/input';

interface IProjectProps {
    project: string;
    handleChange: Function;
}

interface IProjectInputState {
    internalValue: string;
    error: string;
}

export class Project extends React.Component<IProjectProps, IProjectInputState> {

    constructor(props) {
        super(props);

        this.state = {
            internalValue: this.props.project,
            error: null
        };
    }
    validate = (v: string) => {
        let error = null;
        if (v.trim() === "") {
            error = "The project id is required";
        }

        this.setState({ error } as IProjectInputState);
    }


    onChange = (v: string) => {
        this.setState({ ...this.state, internalValue: v });
        this.validate(v);
    }

    onBlur = () => {
        let internalValue = this.state.internalValue;
        this.validate(internalValue);
        this.props.handleChange('project', internalValue);
    }
    render() {
        return (
            <Input type='text' onBlur={this.onBlur} error={this.state.error} required={true} maxLength={256} value={this.state.internalValue} onChange={this.onChange} label='Project Id' />
    );
}
}