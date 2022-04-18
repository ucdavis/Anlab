import * as React from 'react';
import { shallow, mount, render } from 'enzyme';
import { Project } from '../Project';

describe('<Project />', () => {
    it('should render an input', () => {
        const target = mount(<Project project="1" handleChange={null} projectRef={null}/>);
        expect(target.find('input').length).toEqual(1);
    });
    it('should load project into internalValue as string', () => {
        const target = shallow(<Project project={'42'} handleChange={null} projectRef={null}/>);
        const internal = target.instance();

        expect(internal.props.project).toBe('42');
    });

    it('should clear error on good value', () => {
        const handleChange = jasmine.createSpy('handleChange');
        const target = mount(<Project project={' '} handleChange={handleChange} projectRef={null}/>);
        const internal = target.instance();

        const inp = target.find('input');
        
        inp.simulate('change', { target: { value: ' ' } });  
        expect(internal.state.error).not.toBeNull();

        inp.simulate('change', { target: { value: 'x' } });
        expect(internal.state.error).toBeNull();
    });

    it('should set error empty string value', () => {
        const handleChange = jasmine.createSpy('handleChange');
        const target = mount(<Project project={' '} handleChange={handleChange} projectRef={null}/>);
        const internal = target.instance();

        const inp = target.find('input');

        inp.simulate('change', { target: { value: ' ' } }); 

        expect(internal.state.error).not.toBeNull();
        expect(internal.state.error).toBe("The project Title is required");
    });

    it('should set error spaces string value', () => {
        const handleChange = jasmine.createSpy('handleChange');
        const target = mount(<Project project={' '} handleChange={handleChange} projectRef={null}/>);
        const internal = target.instance();
        const inp = target.find('input');

        inp.simulate('change', { target: { value: '   ' } }); 

        expect(internal.state.error).not.toBeNull();
        expect(internal.state.error).toBe("The project Title is required");
    });
});
