import * as React from 'react';
import { shallow, mount, render } from 'enzyme';
import { Project } from '../Project';

describe('<Project />', () => {
    it('should render an input', () => {
        const target = mount(<Project />);
        expect(target.find('input').length).toEqual(1);
    });
    it('should load project into internalValue as string', () => {
        const target = shallow(<Project project={'42'} />);
        const internal = target.instance();

        expect(internal.state.internalValue).toBe('42');
    });

    it('should not load project into internalValue on new props as string', () => {
        const target = shallow(<Project project={'24'} />);
        const internal = target.instance();

        target.setProps({ project: '42' }); //Doesn't accept this

        expect(internal.state.internalValue).toBe('24');
    });

    it('should call handleChange with state.internalValue on blur event', () => {
        const handleChange = jasmine.createSpy('handleChange');
        const target = shallow(<Project handleChange={handleChange} />);
        const internal = target.instance();

        internal.state.internalValue = 'test';
        internal.onBlur();

        expect(handleChange).toHaveBeenCalled();
        expect(handleChange).toHaveBeenCalledWith('project','test');
    });
});