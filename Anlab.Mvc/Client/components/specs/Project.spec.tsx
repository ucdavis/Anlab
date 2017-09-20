import * as React from 'react';
import { shallow, mount, render } from 'enzyme';
import { Project } from '../Project';

//describe('<Project />', () => {
//    it('should render an input', () => {
//        const target = mount(<Project project="1" handleChange={null}/>);
//        expect(target.find('input').length).toEqual(1);
//    });
//    it('should load project into internalValue as string', () => {
//        const target = shallow(<Project project={'42'} handleChange={null} />);
//        const internal = target.instance();

//        expect(internal.state.internalValue).toBe('42');
//    });

//    it('should not load project into internalValue on new props as string', () => {
//        const target = shallow(<Project project={'24'} handleChange={null} />);
//        const internal = target.instance();

//        target.setProps({ project: '42' }); //Doesn't accept this

//        expect(internal.state.internalValue).toBe('24');
//    });

//    it('should call handleChange with state.internalValue on blur event', () => {
//        const handleChange = jasmine.createSpy('handleChange');
//        const target = shallow(<Project project="x" handleChange={handleChange} />);
//        const internal = target.instance();

//        internal.state.internalValue = 'test';
//        internal.onBlur();

//        expect(handleChange).toHaveBeenCalled();
//        expect(handleChange).toHaveBeenCalledWith('project','test');
//    });

//    it('should clear error on good value', () => {
//        const target = shallow(<Project project={' '} handleChange={null}  />);
//        const internal = target.instance();

//        internal.onChange(' ');
//        expect(internal.state.error).not.toBeNull();

//        internal.onChange('x');
//        expect(internal.state.error).toBeNull();
//    });

//    it('should set error empty string value', () => {
//        const target = shallow(<Project project={' '} handleChange={null} />);
//        const internal = target.instance();

//        internal.onChange('');

//        expect(internal.state.error).not.toBeNull();
//        expect(internal.state.error).toBe("The project id is required");
//    });

//    it('should set error spaces string value', () => {
//        const target = shallow(<Project project={' '} handleChange={null} />);
//        const internal = target.instance();

//        internal.onChange('   ');

//        expect(internal.state.error).not.toBeNull();
//        expect(internal.state.error).toBe("The project id is required");
//    });
//});