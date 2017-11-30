import * as React from 'react';
import { shallow, mount, render } from 'enzyme';
import { Quantity } from '../Quantity';

describe('<Quantity />', () => {
    it('should render an IntegerInput', () => {
        const target = mount(<Quantity quantity={1} onQuantityChanged={null} quantityRef={null} />);
        expect(target.find('IntegerInput').length).toEqual(1);
    });
    describe('Props ', () => {
        it('should have a name', () => {
            const target = mount(<Quantity quantity={1} onQuantityChanged={null} quantityRef={null}/>);
            expect(target.find('IntegerInput').at(0).prop('name')).toEqual('quantity');
        });
        it('should have a label', () => {
            const target = mount(<Quantity quantity={1} onQuantityChanged={null} quantityRef={null}/>);
            expect(target.find('IntegerInput').at(0).prop('label')).toEqual('Quantity');
        });
        it('should have a value 1', () => {
            const target = mount(<Quantity quantity={1} onQuantityChanged={null} quantityRef={null}/>);
            expect(target.find('IntegerInput').at(0).prop('value')).toEqual(1);
        });
        it('should have a value 2', () => {
            const target = mount(<Quantity quantity={33} onQuantityChanged={null} quantityRef={null}/>);
            expect(target.find('IntegerInput').at(0).prop('value')).toEqual(33);
        });

        it('should have a min of 1', () => {
            const target = mount(<Quantity quantity={1} onQuantityChanged={null} quantityRef={null}/>);
            expect(target.find('IntegerInput').at(0).prop('min')).toEqual(1);
        });
        it('should have a max of 100', () => {
            const target = mount(<Quantity quantity={1} onQuantityChanged={null} quantityRef={null}/>);
            expect(target.find('IntegerInput').at(0).prop('max')).toEqual(100);
        });
        it('should have be required', () => {
            const target = mount(<Quantity quantity={1} onQuantityChanged={null} quantityRef={null} />);
            expect(target.find('IntegerInput').at(0).prop('required')).toBeTruthy();
        });
    });
});
