import * as React from 'react';
import { shallow, mount, render } from 'enzyme';
import { PaymentSelection } from '../PaymentSelection';

describe('<PaymentSelection/>', () => {
    it('should render', () => {
        const payment = { clientType: 'uc', account: '' };
        const target = mount(<PaymentSelection payment={payment} onPaymentSelected={null} ucAccountRef={null} />);
        expect(target.find('div').length).toBeGreaterThan(0);
    });
    describe('<Input /> (Uc Account Entry)', () => {
        it('should not render when other payment method', () => {
            const payment = { clientType: 'other', account: '' };
            const target = mount(<PaymentSelection payment={payment} onPaymentSelected={null} ucAccountRef={null} />);
            expect(target.find('Input').length).toEqual(0);
        });
        it('should render when uc payment method', () => {
            const payment = { clientType: 'uc', account: '' };
            const target = mount(<PaymentSelection payment={payment} onPaymentSelected={null} ucAccountRef={null} />);
            expect(target.find('Input').length).toEqual(1);
        });
        describe('Parameters', () => {
            const payment = { clientType: 'uc', account: '123' };
            const target = mount(<PaymentSelection payment={payment} onPaymentSelected={null} ucAccountRef={null} />);
            const input = target.find('Input').at(0);

            it('should have a type of error', () => {
                expect(input.prop('error')).toEqual('');
            });

            it('should have a label', () => {
                expect(input.prop('label')).toEqual('UC Account');
            });
            it('should have a maxLength of 50', () => {
                expect(input.prop('maxLength')).toEqual(50);
            });
            it('should have set a value', () => {
                expect(input.prop('value')).toEqual('123');
            });
        });
        it('should call onPaymentSelected when the account is changed', () => {
            const onPaymentSelected = jasmine.createSpy('onPaymentSelected');
            const payment = { clientType: 'uc', account: '123' };
            const target = mount(<PaymentSelection payment={payment} onPaymentSelected={onPaymentSelected} ucAccountRef={null} />);
            expect(target.find('Input').length).toEqual(1);

            const inp = target.find('input');
            inp.simulate('change', { target: { value: 'xxx'} });

            expect(onPaymentSelected).toHaveBeenCalled();
            expect(onPaymentSelected).toHaveBeenCalledWith({ clientType: 'uc', account: 'xxx' });
        });
    });
    describe('<div/> ', () => {
        const payment = { clientType: 'uc', account: '' };
        const target = mount(<PaymentSelection payment={payment} onPaymentSelected={null} ucAccountRef={null} />);

        it('second div should render with className row', () => {
            expect(target.find('div').length).toBeGreaterThan(0);
            var div = target.find('div').at(1);
            expect(div.hasClass('row')).toBe(true);
        });

    });
    describe('Other Selection div', () => {
        it('should render with basic classes 1', () => {
            const payment = { clientType: 'creditcard', account: '' };
            const target = mount(<PaymentSelection payment={payment} onPaymentSelected={null} ucAccountRef={null} />);

            var div = target.find('div').at(2);
            expect(div.hasClass('anlab_form_style')).toBe(true);
            expect(div.hasClass('col-5')).toBe(true);
        });
        it('should render with basic classes 2', () => {
            const payment = { clientType: 'uc', account: '' };
            const target = mount(<PaymentSelection payment={payment} onPaymentSelected={null} ucAccountRef={null} />);

            var div = target.find('div').at(2);
            expect(div.hasClass('anlab_form_style')).toBe(true);
            expect(div.hasClass('col-5')).toBe(true);
        });
        it('should render with actice classes when clientType is not uc', () => {
            const payment = { clientType: 'creditcard', account: '' };
            const target = mount(<PaymentSelection payment={payment} onPaymentSelected={null} ucAccountRef={null} />);

            var div = target.find('div').at(2);
            expect(div.hasClass('active-border')).toBe(true);
            expect(div.hasClass('active-text')).toBe(true);
            expect(div.hasClass('active-bg')).toBe(true);
        });
        it('should render without actice classes when clientType is uc', () => {
            const payment = { clientType: 'uc', account: '' };
            const target = mount(<PaymentSelection payment={payment} onPaymentSelected={null} ucAccountRef={null} />);

            var div = target.find('div').at(2);
            expect(div.hasClass('active-border')).toBe(false);
            expect(div.hasClass('active-text')).toBe(false);
            expect(div.hasClass('active-bg')).toBe(false);
        });

        it('should render with children', () => {
            const payment = { clientType: 'uc', account: '' };
            const target = mount(<PaymentSelection payment={payment} onPaymentSelected={null} ucAccountRef={null} />);

            var div = target.find('div').at(2);
            expect(div.children().length).toEqual(2);
        });
        it('should render with h3', () => {
            const payment = { clientType: 'uc', account: '' };
            const target = mount(<PaymentSelection payment={payment} onPaymentSelected={null} ucAccountRef={null} />);

            var h3 = target.find('div').at(2).find('h3');
            expect(h3.length).toEqual(1);
            expect(h3.text()).toEqual('Credit Card');
        });
        it('should render with p tag', () => {
            const payment = { clientType: 'uc', account: '' };
            const target = mount(<PaymentSelection payment={payment} onPaymentSelected={null} ucAccountRef={null} />);

            var p = target.find('div').at(2).find('p');
            expect(p.length).toEqual(1);
        });
    });

    it('Second div should have a span', () => {
        const payment = { clientType: 'uc', account: '' };
        const target = mount(<PaymentSelection payment={payment} onPaymentSelected={null} ucAccountRef={null} />);
        expect(target.find('div').length).toBeGreaterThan(0);
        var div = target.find('div').at(1);
        expect(div.children().length).toBe(3);
        expect(div.children().at(1).hasClass('dividing_span')).toBe(true);
        expect(div.children().at(1).hasClass('col-2')).toBe(true);
        expect(div.children().at(1).hasClass('t-center')).toBe(true);
        expect(div.children().at(1).hasClass('align-middle')).toBe(true);
        expect(div.children().at(1).text()).toEqual('or');
    });

    describe('Uc Selection div', () => {
        it('should render with basic classes 1', () => {
            const payment = { clientType: 'uc', account: '' };
            const target = mount(<PaymentSelection payment={payment} onPaymentSelected={null} ucAccountRef={null} />);

            var div = target.find('div').at(3);
            expect(div.hasClass('anlab_form_style')).toBe(true);
            expect(div.hasClass('col-5')).toBe(true);
        });
        it('should render with basic classes 2', () => {
            const payment = { clientType: 'creditcard', account: '' };
            const target = mount(<PaymentSelection payment={payment} onPaymentSelected={null} ucAccountRef={null} />);

            var div = target.find('div').at(3);
            expect(div.hasClass('anlab_form_style')).toBe(true);
            expect(div.hasClass('col-5')).toBe(true);
        });
        it('should render with actice classes when clientType is uc', () => {
            const payment = { clientType: 'uc', account: '' };
            const target = mount(<PaymentSelection payment={payment} onPaymentSelected={null} ucAccountRef={null} />);

            var div = target.find('div').at(3);
            expect(div.hasClass('active-border')).toBe(true);
            expect(div.hasClass('active-text')).toBe(true);
            expect(div.hasClass('active-bg')).toBe(true);
        });
        it('should render without actice classes when clientType is not uc', () => {
            const payment = { clientType: 'creditcard', account: '' };
            const target = mount(<PaymentSelection payment={payment} onPaymentSelected={null} ucAccountRef={null} />);

            var div = target.find('div').at(3);
            expect(div.hasClass('active-border')).toBe(false);
            expect(div.hasClass('active-text')).toBe(false);
            expect(div.hasClass('active-bg')).toBe(false);
        });

        it('should render with children', () => {
            const payment = { clientType: 'uc', account: '' };
            const target = mount(<PaymentSelection payment={payment} onPaymentSelected={null} ucAccountRef={null} />);

            var div = target.find('div').at(3);
            expect(div.children().length).toEqual(2);
        });
        it('should render with h3', () => {
            const payment = { clientType: 'uc', account: '' };
            const target = mount(<PaymentSelection payment={payment} onPaymentSelected={null} ucAccountRef={null} />);

            var h3 = target.find('div').at(3).find('h3');
            expect(h3.length).toEqual(1);
            expect(h3.text()).toEqual('UC Funds');
        });
        it('should render with p tag', () => {
            const payment = { clientType: 'uc', account: '' };
            const target = mount(<PaymentSelection payment={payment} onPaymentSelected={null} ucAccountRef={null} />);

            var p = target.find('div').at(3).find('p');
            expect(p.length).toEqual(1);
        });
    });


    it('should call onPaymentSelected when the clientType is changed', () => {
        const onPaymentSelected = jasmine.createSpy('onPaymentSelected');
        const payment = { clientType: 'uc', account: '123' };
        const target = mount(<PaymentSelection payment={payment} onPaymentSelected={onPaymentSelected} ucAccountRef={null} />);
        expect(target.find('Input').length).toEqual(1);

        const internal = target.instance();
        internal._handleChange('creditcard');

        expect(onPaymentSelected).toHaveBeenCalled();
        expect(onPaymentSelected).toHaveBeenCalledWith({ clientType: 'creditcard', account: '123' });
    });

});
