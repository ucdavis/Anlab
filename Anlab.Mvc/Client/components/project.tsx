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

    onChange = (v: string) => {
        let error = null;
        if (v.trim() === "") {
            error = "The project id is required";
        }
        this.setState({ ...this.state, internalValue: v, error });
        
    }

    onBlur = () => {
        let error = null;
        let internalValue = this.state.internalValue;
        if (internalValue.trim() === "") {
            error = "The project id is required";
        }
        this.setState({ ...this.state, error });
        this.props.handleChange('project', internalValue);
    }
    render() {
        return (
            <Input type='text' onBlur={this.onBlur} error={this.state.error} required={true} maxLength={256} value={this.state.internalValue} onChange={this.onChange} label='Project Id' />
    );
}
}