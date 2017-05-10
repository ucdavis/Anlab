import * as React from 'react';
import { shallow, mount, render } from 'enzyme';

import { NumberInput } from './numberInput';

describe('<NumberInput />', () => {
    it('should render an input', () => {
        const target = mount(<NumberInput />);
        expect(target.find('input').length).toEqual(1);
    });

    it('should load value into internalValue as string', () => {
        const onChanged = jest.fn();
        const target = shallow(<NumberInput value={42} />);
        const internal = target.instance();

        expect(internal.state.internalValue).toBe('42');
    });

    it('should load value into internalValue on new props as string', () => {
        const onChanged = jest.fn();
        const target = shallow(<NumberInput value={24} />);
        const internal = target.instance();

        target.setProps({value:42});

        expect(internal.state.internalValue).toBe('42');
    });

    it('should call onChanged with state.internalValue on blur event', () => {
        const onChanged = jest.fn();
        const target = shallow(<NumberInput onChanged={onChanged} />);
        const internal = target.instance();

        internal.state.internalValue = '42.5';
        internal.onBlur();

        expect(onChanged).toHaveBeenCalled();
        expect(onChanged).toHaveBeenLastCalledWith(42.5);
    });

    it('should call onChanged with truncated state.internalValue', () => {
        const onChanged = jest.fn();
        const target = shallow(<NumberInput onChanged={onChanged} integer />);
        const internal = target.instance();

        internal.state.internalValue = '42.5';
        internal.onBlur();

        expect(onChanged).toHaveBeenLastCalledWith(42);
    });

    it('should clear error on good value', () => {
        const target = shallow(<NumberInput min={10} max={20} />);
        const internal = target.instance();

        internal.onChange('15');

        expect(internal.state.error).toBeNull();
    });

    it('should set error on non-number value', () => {
        const target = shallow(<NumberInput />);
        const internal = target.instance();

        internal.onChange('abc');

        expect(internal.state.error).not.toBeNull();
    });

    it('should set error on less than min value', () => {
        const target = shallow(<NumberInput min={10} />);
        const internal = target.instance();

        internal.onChange('5');

        expect(internal.state.error).not.toBeNull();
    });

    it('should set error on more than max value', () => {
        const target = shallow(<NumberInput max={10} />);
        const internal = target.instance();

        internal.onChange('15');

        expect(internal.state.error).not.toBeNull();
    });
});
