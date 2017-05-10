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
        const target = mount(<NumberInput value={42} />);
        const internal = target.instance();

        expect(internal.state.internalValue).toBe('42');
    });

    it('should load value into internalValue on new props as string', () => {
        const onChanged = jest.fn();
        const target = mount(<NumberInput value={24} />);
        const internal = target.instance();

        target.setProps({value:42});

        expect(internal.state.internalValue).toBe('42');
    });

    it('should call onChanged after blur event', () => {
        const onChanged = jest.fn();
        const target = mount(<NumberInput onChanged={onChanged} />);

        target.find('input').simulate('blur');

        expect(onChanged).toHaveBeenCalled();
    });

    it('should call onChanged with state.internalValue on blur event', () => {
        const onChanged = jest.fn();
        const target = mount(<NumberInput onChanged={onChanged} />);
        const internal = target.instance()
        internal.state.internalValue = 42

        target.find('input').simulate('blur');
        expect(onChanged).toHaveBeenLastCalledWith(42);
    });

    it('should call onChanged with truncated state.internalValue on blur event', () => {
        const onChanged = jest.fn();
        const target = mount(<NumberInput onChanged={onChanged} integer value={42.5} />);
        const internal = target.instance();

        target.find('input').simulate('blur');
        expect(onChanged).toHaveBeenLastCalledWith(42);
    })
});
