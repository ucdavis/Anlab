import * as React from 'react';
import { shallow, mount, render } from 'enzyme';
import { AdditionalInfo } from '../AdditionalInfo';

describe('<AdditionalInfo />', () => {
    it('should render an input', () => {
        const handleChange = jasmine.createSpy('handleChange');
        const target = mount(<AdditionalInfo additionalInfo="Test" handleChange={handleChange} />);
        expect(target.find('Input').length).toEqual(1);
    });

    it('should have a maxLength of 2000', () => {
        const handleChange = jasmine.createSpy('handleChange');
        const target = mount(<AdditionalInfo additionalInfo="Test" handleChange={handleChange} />);
        const input = target.find('Input');
        expect(input.prop('maxLength')).toEqual(2000);
    });

    it('should have a label', () => {
        const handleChange = jasmine.createSpy('handleChange');
        const target = mount(<AdditionalInfo additionalInfo="Test" handleChange={handleChange} />);
        const input = target.find('Input');
        expect(input.prop('label')).toEqual('Additional Information');
    });

    it('should have be multiLine', () => {
        const handleChange = jasmine.createSpy('handleChange');
        const target = mount(<AdditionalInfo additionalInfo="Test" handleChange={handleChange} />);
        const input = target.find('Input');
        expect(input.prop('multiline')).toEqual(true);
    });

    it('should have a type of text', () => {
        const handleChange = jasmine.createSpy('handleChange');
        const target = mount(<AdditionalInfo additionalInfo="Test" handleChange={handleChange} />);
        const input = target.find('Input');
        expect(input.prop('type')).toEqual('text');
    });

    it('should have a value of Test1', () => {
        const handleChange = jasmine.createSpy('handleChange');
        const target = mount(<AdditionalInfo additionalInfo="Test1" handleChange={handleChange} />);
        const input = target.find('Input');
        expect(input.prop('value')).toEqual('Test1');
    });

    it('should have a value of Test2', () => {
        const handleChange = jasmine.createSpy('handleChange');
        const target = mount(<AdditionalInfo additionalInfo="Test2" handleChange={handleChange} />);
        const input = target.find('Input');
        expect(input.prop('value')).toEqual('Test2');
    });


    //it('should call handleChange with state.internalValue on blur event', () => {
    //    const handleChange = jasmine.createSpy('handleChange');
    //    const target = shallow(<AdditionalInfo additionalInfo="x" handleChange={handleChange}/>);
    //    const internal = target.instance();

    //    internal.state.internalValue = 'test';
    //    internal.onBlur();

    //    expect(handleChange).toHaveBeenCalled();
    //    expect(handleChange).toHaveBeenCalledWith('additionalInfo', 'test');
    //});
});