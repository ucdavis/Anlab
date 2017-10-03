
import * as React from 'react';
import Input from 'react-toolbox/lib/input';

interface IAdditionalInfoProps {
    additionalInfo: string;
    handleChange: Function;
}

interface IAdditionalInfoState {
    internalValue: string;
}

export class AdditionalInfo extends React.Component<IAdditionalInfoProps, IAdditionalInfoState> {
    constructor(props) {
        super(props);

        this.state = {
            internalValue: this.props.additionalInfo
        };
    }

    onChange = (v: string) => {
        this.setState({ ...this.state, internalValue: v });
    }

    onBlur = () => {
        this.props.handleChange('additionalInfo', this.state.internalValue);
    }

    render() {
        return (
            <Input type='text' multiline label='Additional Information' maxLength={2000} value={this.state.internalValue} onChange={this.onChange} onBlur={this.onBlur} />
        );
    }
}