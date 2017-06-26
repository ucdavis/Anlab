import * as React from 'react';
import { shallow, mount, render } from 'enzyme';
import { Quantity } from '../Quantity';

describe('<Quantity />', () => {
    it('should render an NumberInput', () => {
        const target = mount(<Quantity quantity={1} onQuantityChanged={null}/>);
        expect(target.find('NumberInput').length).toEqual(1);
    });
    describe('Props ', () => {
        it('should have a name', () => {
            const target = mount(<Quantity quantity={1} onQuantityChanged={null} />);
            expect(target.find('NumberInput').at(0).prop('name')).toEqual('quantity');
        });
        it('should have a label', () => {
            const target = mount(<Quantity quantity={1} onQuantityChanged={null} />);
            expect(target.find('NumberInput').at(0).prop('label')).toEqual('Quantity');
        });
        it('should have a value 1', () => {
            const target = mount(<Quantity quantity={1} onQuantityChanged={null} />);
            expect(target.find('NumberInput').at(0).prop('value')).toEqual(1);
        });
        it('should have a value 2', () => {
            const target = mount(<Quantity quantity={33} onQuantityChanged={null} />);
            expect(target.find('NumberInput').at(0).prop('value')).toEqual(33);
        });
        it('should have a integer', () => {
            const target = mount(<Quantity quantity={1} onQuantityChanged={null} />);
            expect(target.find('NumberInput').at(0).prop('integer')).toEqual(true);
        });
        it('should have a min of 1', () => {
            const target = mount(<Quantity quantity={1} onQuantityChanged={null} />);
            expect(target.find('NumberInput').at(0).prop('min')).toEqual(1);
        });
        it('should have a max of 100', () => {
            const target = mount(<Quantity quantity={1} onQuantityChanged={null} />);
            expect(target.find('NumberInput').at(0).prop('max')).toEqual(100);
        });
    });
});