import * as React from 'react';
import { shallow, mount, render } from 'enzyme';
import { PaymentSelection } from '../PaymentSelection';

describe('<PaymentSelection/>', () => {
    it('should render', () => {
        const payment = { clientType: 'uc', account: '' };
        const target = mount(<PaymentSelection payment={payment} onPaymentSelected={null} />);
        expect(target.find('div').length).toBeGreaterThan(0);
    });
    describe('<Input />', () => {
        it('should not render when other payment method', () => {
            const payment = { clientType: 'other', account: '' };
            const target = mount(<PaymentSelection payment={payment} onPaymentSelected={null} />);
            expect(target.find('Input').length).toEqual(0);
        });
        it('should render when uc payment method', () => {
            const payment = { clientType: 'uc', account: '' };
            const target = mount(<PaymentSelection payment={payment} onPaymentSelected={null} />);
            expect(target.find('Input').length).toEqual(1);
        });
        describe('Parameters', () => {
            const payment = { clientType: 'uc', account: '123' };
            const target = mount(<PaymentSelection payment={payment} onPaymentSelected={null} />);
            const input = target.find('Input').at(0);

            it('should have a type of text', () => {
                expect(input.prop('type')).toEqual('text');
            });
            it('should have a label', () => {
                expect(input.prop('label')).toEqual('UC Account');
            });
            it('should have a maxLength of 10', () => {
                expect(input.prop('maxLength')).toEqual(10);
            });
            it('should have set a value', () => {
                expect(input.prop('value')).toEqual('123');
            });
        });
        it('should call onPaymentSelected when the account is changed', () => {
            const onPaymentSelected = jasmine.createSpy('onPaymentSelected');
            const payment = { clientType: 'uc', account: '123' };
            const target = mount(<PaymentSelection payment={payment} onPaymentSelected={onPaymentSelected} />);
            expect(target.find('Input').length).toEqual(1);

            const internal = target.instance();
            internal.handleAccountChange('xxx');

            expect(onPaymentSelected).toHaveBeenCalled();
            expect(onPaymentSelected).toHaveBeenCalledWith({ clientType: 'uc', account: 'xxx' });
        });
    });
});