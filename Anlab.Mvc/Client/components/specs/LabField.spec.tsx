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

        it('should have a type of text', () => {
            const handleChange = jasmine.createSpy('handleChange');
            const target = mount(<LabFields isFromLab={true} labComments="Test" adjustmentAmount={0} handleChange={handleChange} />);
            const input = target.find('Input').at(0);
            expect(input.prop('type')).toEqual('text');
        });

        it('should have be multiline', () => {
            const handleChange = jasmine.createSpy('handleChange');
            const target = mount(<LabFields isFromLab={true} labComments="Test" adjustmentAmount={0} handleChange={handleChange} />);
            const input = target.find('Input').at(0);
            expect(input.prop('multiline')).toEqual(true);
        });

        it('should have have a maxLength of 2000', () => {
            const handleChange = jasmine.createSpy('handleChange');
            const target = mount(<LabFields isFromLab={true} labComments="Test" adjustmentAmount={0} handleChange={handleChange} />);
            const input = target.find('Input').at(0);
            expect(input.prop('maxLength')).toEqual(2000);
        }); 

        it('should have have a value of Test1', () => {
            const handleChange = jasmine.createSpy('handleChange');
            const target = mount(<LabFields isFromLab={true} labComments="Test1" adjustmentAmount={0} handleChange={handleChange} />);
            const input = target.find('Input').at(0);
            expect(input.prop('value')).toEqual("Test1");
        });

        it('should have have a value of Test2', () => {
            const handleChange = jasmine.createSpy('handleChange');
            const target = mount(<LabFields isFromLab={true} labComments="Test2" adjustmentAmount={0} handleChange={handleChange} />);
            const input = target.find('Input').at(0);
            expect(input.prop('value')).toEqual("Test2");
        });

        it('it should call handleChange', () => {
            const handleChange = jasmine.createSpy('handleChange');
            const target = mount(<LabFields isFromLab={true} labComments="Test" adjustmentAmount={0} handleChange={handleChange} />);
            target.find('textarea').at(0).first().simulate('change', 'XXX');
            expect(handleChange).toHaveBeenCalled();

        });
        //it('debug', () => {
        //    const handleChange = jasmine.createSpy('handleChange');
        //    const target = mount(<LabFields isFromLab={true} labComments="Test" adjustmentAmount={0} handleChange={handleChange} />);
        //    console.log(target.find('textarea').at(0).first().debug());

        //});
    });
    describe('<NumberInput/> ', () => {
        it('should render an NumberInput', () => {
            const handleChange = jasmine.createSpy('handleChange');
            const target = mount(<LabFields isFromLab={true} labComments="Test" adjustmentAmount={0} handleChange={handleChange} />);
            expect(target.find('NumberInput').length).toEqual(1);
        });
        it('should have a label', () => {
            const handleChange = jasmine.createSpy('handleChange');
            const target = mount(<LabFields isFromLab={true} labComments="Test" adjustmentAmount={0} handleChange={handleChange} />);
            const input = target.find('NumberInput').at(0);
            expect(input.prop('label')).toEqual('Adjustment Amount');
        });
        it('should have a name', () => {
            const handleChange = jasmine.createSpy('handleChange');
            const target = mount(<LabFields isFromLab={true} labComments="Test" adjustmentAmount={0} handleChange={handleChange} />);
            const input = target.find('NumberInput').at(0);
            expect(input.prop('name')).toEqual('adjustmentAmount');
        });

        it('should have have a value of 0.01', () => {
            const handleChange = jasmine.createSpy('handleChange');
            const target = mount(<LabFields isFromLab={true} labComments="Test1" adjustmentAmount={0.01} handleChange={handleChange} />);
            const input = target.find('NumberInput').at(0);
            expect(input.prop('value')).toEqual(0.01);
        });

        it('should have have a value of 500', () => {
            const handleChange = jasmine.createSpy('handleChange');
            const target = mount(<LabFields isFromLab={true} labComments="Test1" adjustmentAmount={500} handleChange={handleChange} />);
            const input = target.find('NumberInput').at(0);
            expect(input.prop('value')).toEqual(500);
        });

        it('should have have a value of -500', () => {
            const handleChange = jasmine.createSpy('handleChange');
            const target = mount(<LabFields isFromLab={true} labComments="Test1" adjustmentAmount={-500} handleChange={handleChange} />);
            const input = target.find('NumberInput').at(0);
            expect(input.prop('value')).toEqual(-500);
        });

        it('should have have a integer value of false', () => {
            const handleChange = jasmine.createSpy('handleChange');
            const target = mount(<LabFields isFromLab={true} labComments="Test1" adjustmentAmount={-500} handleChange={handleChange} />);
            const input = target.find('NumberInput').at(0);
            expect(input.prop('integer')).toEqual(false);
        });

    });
});