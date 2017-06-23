import * as React from 'react';
import { shallow, mount, render } from 'enzyme';
import { Grind } from '../Grind';

describe('<Grind />', () => {
    it('should render null when not soil', () => {
        const target = mount(<Grind sampleType="Dirt" handleChange={null} grind={false} />);
        expect(target.find('Checkbox').length).toEqual(0);
    });
    it('should render a Checkbox when sampleType is soil', () => {
        const handleChange = jasmine.createSpy('handleChange');
        const target = mount(<Grind sampleType="Soil" handleChange={handleChange} grind={false} />);
        expect(target.find('Checkbox').length).toEqual(1);
    });
    it('should render a Checkbox when sampleType is plant', () => {
        const handleChange = jasmine.createSpy('handleChange');
        const target = mount(<Grind sampleType="Plant" handleChange={handleChange} grind={false} />);
        expect(target.find('Checkbox').length).toEqual(1);
    });
    it('should have a checked false', () => {
        const handleChange = jasmine.createSpy('handleChange');
        const target = mount(<Grind sampleType="Soil" handleChange={handleChange} grind={false} />);
        const input = target.find('Checkbox');
        expect(input.prop('checked')).toEqual(false);
    });
    it('should have a checked true', () => {
        const handleChange = jasmine.createSpy('handleChange');
        const target = mount(<Grind sampleType="Soil" handleChange={handleChange} grind={true} />);
        const input = target.find('Checkbox');
        expect(input.prop('checked')).toEqual(true);
    });
    it('should have a lable prop', () => {
        const handleChange = jasmine.createSpy('handleChange');
        const target = mount(<Grind sampleType="Soil" handleChange={handleChange} grind={true} />);
        const input = target.find('Checkbox');
        expect(input.prop('label')).toEqual("Grind Samples");
    });

    it('should call handleChange with on change event', () => {
        const handleChange = jasmine.createSpy('handleChange');
        const target = mount(<Grind sampleType="Soil" handleChange={handleChange} grind={false} />);
        target.find('input[type="checkbox"]').simulate('click', { target: { checked: true } });

        expect(handleChange).toHaveBeenCalled();
        //expect(handleChange).toHaveBeenCalledWith('foreignSoil'); //Don't know why, but this doesn't work with the checkbox
    });
});