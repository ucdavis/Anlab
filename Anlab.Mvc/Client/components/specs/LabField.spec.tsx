import * as React from 'react';
import { shallow, mount, render } from 'enzyme';
import { LabFields } from '../LabFields';

describe('<LabFields />', () => {
    it('should not render an input when is not from lab', () => {
        const handleChange = jasmine.createSpy('handleChange');
        const target = mount(<LabFields isFromLab={false} labComments="Test" adjustmentAmount={0} handleChange={handleChange} />);
        expect(target.find('Input').length).toEqual(0);
    });
    it('should render an input', () => {
        const handleChange = jasmine.createSpy('handleChange');
        const target = mount(<LabFields isFromLab={true} labComments="Test" adjustmentAmount={0} handleChange={handleChange} />);
        expect(target.find('Input').length).toEqual(2);
    });
    describe('<Input/> ', () => {
        it('should have a label', () => {
            const handleChange = jasmine.createSpy('handleChange');
            const target = mount(<LabFields isFromLab={true} labComments="Test" adjustmentAmount={0} handleChange={handleChange} />);
            const input = target.find('Input').at(0);
            expect(input.prop('label')).toEqual('Lab Comments');
        });
    });
    describe('<NumberInput/> ', () => {
        it('should have a label', () => {
            const handleChange = jasmine.createSpy('handleChange');
            const target = mount(<LabFields isFromLab={true} labComments="Test" adjustmentAmount={0} handleChange={handleChange} />);
            const input = target.find('Input').at(1);
            expect(input.prop('label')).toEqual('Adjustment Amount');
        });
    });
});