import * as React from 'react';
import Input from 'react-toolbox/lib/input';

interface ICommodityProps {
    commodity: string;
    handleChange: Function;
}

interface ICommodityState {
    internalValue: string;
}

export class Commodity extends React.Component<ICommodityProps, any> {
    constructor(props) {
        super(props);

        this.state = {
            internalValue: this.props.commodity
        };
    }

    onChange = (v: string) => {
        this.setState({ ...this.state, internalValue: v });
    }

    onBlur = () => {
        this.props.handleChange('commodity', this.state.internalValue);
    }

    render() {
        return (
            <Input type='text' label={"Commodity"} value={this.state.internalValue} onChange={this.onChange} onBlur={this.onBlur} />
        );
    }
}