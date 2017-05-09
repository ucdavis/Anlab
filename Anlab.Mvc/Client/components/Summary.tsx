import * as React from 'react';
import { Table, TableHead, TableRow, TableCell } from 'react-toolbox/lib/table';
import { ITestItem } from './TestList';
import { IPayment } from './PaymentSelection';
import NumberFormat from 'react-number-format';

interface ISummaryProps {
    testItems: Array<ITestItem>;
    quantity: number;
    payment: IPayment;
}

export class Summary extends React.Component<ISummaryProps, any> {
    totalCost = () => {
        const total = this.props.testItems.reduce((prev, item) => {
            // total for current item
            const price = this.props.payment.clientType === 'uc' ? item.internalCost : item.externalCost;
            const perTest = price * this.props.quantity;
            return prev + perTest + item.setupCost;
        }, 0);

        return total;
    }
    _renderTests = () => {
        const tests = this.props.testItems.map(item => {
            const price = this.props.payment.clientType === 'uc' ? item.internalCost : item.externalCost;
            const perTest = price * this.props.quantity;
            const rowTotalDisplay = (perTest + item.setupCost);
            return (
                <TableRow key={item.id}>
                    <TableCell>{item.analysis}</TableCell>
                    <TableCell><NumberFormat value={price} displayType={'text'} thousandSeparator={true} decimalPrecision={true} prefix={'$'} /></TableCell>
                    <TableCell><NumberFormat value={perTest} displayType={'text'} thousandSeparator={true} decimalPrecision={true} prefix={'$'} /></TableCell>
                    <TableCell><NumberFormat value={item.setupCost} displayType={'text'} thousandSeparator={true} decimalPrecision={true} prefix={'$'} /></TableCell>
                    <TableCell><NumberFormat value={rowTotalDisplay} displayType={'text'} thousandSeparator={true} decimalPrecision={true} prefix={'$'} /></TableCell>
                </TableRow>
            );
        });

        return tests;
    }
    render() {
        if (this.props.testItems.length > 0) {
            return (
                <div style={{ marginTop: 15 }}>
                    <h3>Selected Tests Total: <NumberFormat value={this.totalCost()} displayType={'text'} thousandSeparator={true} decimalPrecision={true} prefix={'$'} /></h3>
                    <Table multiSelectable={false} selectable={false}>
                        <TableHead>

                            <TableCell>Analysis</TableCell>
                            <TableCell>Fee</TableCell>
                            <TableCell>Price</TableCell>
                            <TableCell>Setup</TableCell>
                            <TableCell>Total</TableCell>

                        </TableHead>

                        {this._renderTests()}
                    </Table>
                </div>
            );
        } else {
            return null;
        }
    }
}