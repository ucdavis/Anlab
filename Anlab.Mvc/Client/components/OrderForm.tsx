import * as React from 'react';
import TestList from './TestList';

declare var window: any;

export default class OrderForm extends React.Component<any, any> {
// ReSharper disable once TsNotResolved
    state = { payment: {}, testItems: window.App.orderData.testItems };

    render() {
        return (
            <div>
                <TestList items={this.state.testItems} />
            </div>
        );
    }
}