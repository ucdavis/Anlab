import * as React from 'react';
import Input from 'react-toolbox/lib/input';

interface IProjectProps {
    project: string;
    handleChange: Function;
}

export class Project extends React.Component<IProjectProps, any> {
    render() {
        return (
            <Input type='text' required={true} maxLength={256} value={this.props.project} onChange={this.props.handleChange.bind(this, 'project')} label='Project Id' />
    );
}
}