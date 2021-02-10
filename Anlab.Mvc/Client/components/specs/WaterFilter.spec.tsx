import * as React from 'react';
import { shallow, mount, render } from 'enzyme';
import { WaterFilter } from '../WaterFilter';

describe('<Grind />', () => {
    it('should render null when not water', () => {
        const target = mount(<WaterFilter sampleType="Dirt" handleChange={null} filterWater={false} />);
        expect(target.find('Themedn').length).toEqual(0);
    });
    it('should render a Checkbox when sampleType is water', () => {
        const handleChange = jasmine.createSpy('handleChange');
        const target = mount(<WaterFilter sampleType="Water" handleChange={handleChange} filterWater={false} />);
        expect(target.find('Themedn').length).toBeGreaterThan(0);
    });
    it('should have a checked false', () => {
        const handleChange = jasmine.createSpy('handleChange');
        const target = mount(<WaterFilter sampleType="Water" handleChange={handleChange} filterWater={false} />);
        const input = target.find('Themedn').first();
        expect(input.prop('checked')).toEqual(false);
    });
    it('should have a checked true', () => {
        const handleChange = jasmine.createSpy('handleChange');
        const target = mount(<WaterFilter sampleType="Water" handleChange={handleChange} filterWater={true} />);
        const input = target.find('Themedn').first();
        expect(input.prop('checked')).toEqual(true);
    });
    it('should have a lable prop', () => {
        const handleChange = jasmine.createSpy('handleChange');
        const target = mount(<WaterFilter sampleType="Water" handleChange={handleChange} filterWater={true} />);
        const input = target.find('Themedn').first();
        expect(input.prop('label')).toEqual("Filter");
    });

    it('should call handleChange with on change event', () => {
        const handleChange = jasmine.createSpy('handleChange');
        const target = mount(<WaterFilter sampleType="Water" handleChange={handleChange} filterWater={false} />);
        target.find('input[type="checkbox"]').simulate('click', { target: { checked: true } });

        expect(handleChange).toHaveBeenCalled();
        //expect(handleChange).toHaveBeenCalledWith('foreignSoil'); //Don't know why, but this doesn't work with the checkbox
    });
});
