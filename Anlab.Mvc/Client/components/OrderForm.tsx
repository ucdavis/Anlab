import * as React from 'react';
import TestList from './TestList';

export default class OrderForm extends React.Component<any, any> {
    state = { payment: {} };

    render() {
        return (
            <div>
                <TestList />
            </div>
        );
    }
}