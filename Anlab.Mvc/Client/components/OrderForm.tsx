import * as React from 'react';
import { ITestItem, TestList } from './TestList';

declare var window: any;

interface IOrderState {
    testItems: Array<ITestItem>;
}

export default class OrderForm extends React.Component<any, IOrderState> {
    state = { payment: {}, testItems: window.App.orderData.testItems };

    render() {
        return (
            <div>
                <TestList items={this.state.testItems} />
            </div>
        );
    }
}