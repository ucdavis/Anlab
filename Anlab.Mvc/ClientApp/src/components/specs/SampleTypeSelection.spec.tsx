import * as React from 'react';
import { shallow, mount, render } from 'enzyme';
import { SampleTypeSelection } from '../SampleTypeSelection';

describe('<SampleTypeSelection/>', () => {
    it('should render', () => {
        const target = mount(<SampleTypeSelection sampleType="soil" onSampleSelected={null} />);
        expect(target.find('div').length).toBeGreaterThan(0);
    });

    it('should have an inner div with a className of flexrow', () => {
        const target = mount(<SampleTypeSelection sampleType="soil" onSampleSelected={null} />);
        const expectedTag = target.find('div').at(1);
        expect(expectedTag.hasClass('flexrow')).toEqual(true);
    });

    describe('Soil selector', () => {
        it('should have common classNames 1', () => {
            const target = mount(<SampleTypeSelection sampleType="xxx" onSampleSelected={null} />);
            const expectedTag = target.find('div').at(1).childAt(0);
            expect(expectedTag.hasClass('anlab_form_style')).toEqual(true);
            expect(expectedTag.hasClass('anlab_form_samplebtn')).toEqual(true);
            expect(expectedTag.hasClass('flexcol')).toEqual(true);
        });
        it('should have common classNames 2', () => {
            const target = mount(<SampleTypeSelection sampleType="Soil" onSampleSelected={null} />);
            const expectedTag = target.find('div').at(1).childAt(0);
            expect(expectedTag.hasClass('anlab_form_style')).toEqual(true);
            expect(expectedTag.hasClass('anlab_form_samplebtn')).toEqual(true);
            expect(expectedTag.hasClass('flexcol')).toEqual(true);
        });
        it('should have not have selected classNames when not soil', () => {
            const target = mount(<SampleTypeSelection sampleType="xxx" onSampleSelected={null} />);
            const expectedTag = target.find('div').at(1).childAt(0);
            expect(expectedTag.hasClass('active-bg')).toEqual(false);
            expect(expectedTag.hasClass('active-border')).toEqual(false);
            expect(expectedTag.hasClass('active-svg')).toEqual(false);
            expect(expectedTag.hasClass('active-text')).toEqual(false);
        });
        it('should have have selected classNames when soil', () => {
            const target = mount(<SampleTypeSelection sampleType="Soil" onSampleSelected={null} />);
            const expectedTag = target.find('div').at(1).childAt(0);
            expect(expectedTag.hasClass('active-bg')).toEqual(true);
            expect(expectedTag.hasClass('active-border')).toEqual(true);
            expect(expectedTag.hasClass('active-svg')).toEqual(true);
            expect(expectedTag.hasClass('active-text')).toEqual(true);
        });
        it('should have soil svg', () => {
            const target = mount(<SampleTypeSelection sampleType="Soil" onSampleSelected={null} />);
            const expectedTag = target.find('div').at(1).childAt(0).find('SVG');
            //console.log(target.find('div').at(1).childAt(0).debug());
            expect(expectedTag.length).toEqual(1);
            expect(expectedTag.find('title').at(0).text()).toEqual('soil');
        });
        it('should have h3', () => {
            const target = mount(<SampleTypeSelection sampleType="Soil" onSampleSelected={null} />);
            const expectedTag = target.find('div').at(1).childAt(0).find('h3');
            //console.log(target.find('div').at(1).childAt(0).debug());
            expect(expectedTag.length).toEqual(1);
            expect(expectedTag.text()).toEqual('Soil');
        });

        it('should call on click with soil parameter', () => {
            const onSampleSelected = jasmine.createSpy('onSampleSelected');
            const target = mount(<SampleTypeSelection sampleType="Soil" onSampleSelected={onSampleSelected} />);
            const expectedTag = target.find('div').at(1).childAt(0);
            expectedTag.simulate('click');

            expect(onSampleSelected).toHaveBeenCalled();
            expect(onSampleSelected).toHaveBeenCalledWith('Soil');
        });
    });
    describe('Plant selector', () => {
        it('should have common classNames 1', () => {
            const target = mount(<SampleTypeSelection sampleType="xxx" onSampleSelected={null} />);
            const expectedTag = target.find('div').at(1).childAt(1);
            expect(expectedTag.hasClass('anlab_form_style')).toEqual(true);
            expect(expectedTag.hasClass('anlab_form_samplebtn')).toEqual(true);
            expect(expectedTag.hasClass('flexcol')).toEqual(true);
        });
        it('should have common classNames 2', () => {
            const target = mount(<SampleTypeSelection sampleType="Plant" onSampleSelected={null} />);
            const expectedTag = target.find('div').at(1).childAt(1);
            expect(expectedTag.hasClass('anlab_form_style')).toEqual(true);
            expect(expectedTag.hasClass('anlab_form_samplebtn')).toEqual(true);
            expect(expectedTag.hasClass('flexcol')).toEqual(true);
        });
        it('should have not have selected classNames when not soil', () => {
            const target = mount(<SampleTypeSelection sampleType="xxx" onSampleSelected={null} />);
            const expectedTag = target.find('div').at(1).childAt(1);
            expect(expectedTag.hasClass('active-bg')).toEqual(false);
            expect(expectedTag.hasClass('active-border')).toEqual(false);
            expect(expectedTag.hasClass('active-svg')).toEqual(false);
            expect(expectedTag.hasClass('active-text')).toEqual(false);
        });
        it('should have have selected classNames when soil', () => {
            const target = mount(<SampleTypeSelection sampleType="Plant" onSampleSelected={null} />);
            const expectedTag = target.find('div').at(1).childAt(1);
            expect(expectedTag.hasClass('active-bg')).toEqual(true);
            expect(expectedTag.hasClass('active-border')).toEqual(true);
            expect(expectedTag.hasClass('active-svg')).toEqual(true);
            expect(expectedTag.hasClass('active-text')).toEqual(true);
        });
        it('should have soil svg', () => {
            const target = mount(<SampleTypeSelection sampleType="Plant" onSampleSelected={null} />);
            const expectedTag = target.find('div').at(1).childAt(1).find('SVG');
            //console.log(target.find('div').at(1).childAt(0).debug());
            expect(expectedTag.length).toEqual(1);
            expect(expectedTag.find('title').at(0).text()).toEqual('plant');
        });
        it('should have h3', () => {
            const target = mount(<SampleTypeSelection sampleType="Plant" onSampleSelected={null} />);
            const expectedTag = target.find('div').at(1).childAt(1).find('h3');
            //console.log(target.find('div').at(1).childAt(0).debug());
            expect(expectedTag.length).toEqual(1);
            expect(expectedTag.text()).toEqual('Plant Material');
        });

        it('should call on click with soil parameter', () => {
            const onSampleSelected = jasmine.createSpy('onSampleSelected');
            const target = mount(<SampleTypeSelection sampleType="Plant" onSampleSelected={onSampleSelected} />);
            const expectedTag = target.find('div').at(1).childAt(1);
            expectedTag.simulate('click');

            expect(onSampleSelected).toHaveBeenCalled();
            expect(onSampleSelected).toHaveBeenCalledWith('Plant');
        });
    });
    describe('Water selector', () => {
        it('should have common classNames 1', () => {
            const target = mount(<SampleTypeSelection sampleType="xxx" onSampleSelected={null} />);
            const expectedTag = target.find('div').at(1).childAt(2);
            expect(expectedTag.hasClass('anlab_form_style')).toEqual(true);
            expect(expectedTag.hasClass('anlab_form_samplebtn')).toEqual(true);
            expect(expectedTag.hasClass('flexcol')).toEqual(true);
        });
        it('should have common classNames 2', () => {
            const target = mount(<SampleTypeSelection sampleType="Water" onSampleSelected={null} />);
            const expectedTag = target.find('div').at(1).childAt(2);
            expect(expectedTag.hasClass('anlab_form_style')).toEqual(true);
            expect(expectedTag.hasClass('anlab_form_samplebtn')).toEqual(true);
            expect(expectedTag.hasClass('flexcol')).toEqual(true);
        });
        it('should have not have selected classNames when not soil', () => {
            const target = mount(<SampleTypeSelection sampleType="xxx" onSampleSelected={null} />);
            const expectedTag = target.find('div').at(1).childAt(2);
            expect(expectedTag.hasClass('active-bg')).toEqual(false);
            expect(expectedTag.hasClass('active-border')).toEqual(false);
            expect(expectedTag.hasClass('active-svg')).toEqual(false);
            expect(expectedTag.hasClass('active-text')).toEqual(false);
        });
        it('should have have selected classNames when soil', () => {
            const target = mount(<SampleTypeSelection sampleType="Water" onSampleSelected={null} />);
            const expectedTag = target.find('div').at(1).childAt(2);
            expect(expectedTag.hasClass('active-bg')).toEqual(true);
            expect(expectedTag.hasClass('active-border')).toEqual(true);
            expect(expectedTag.hasClass('active-svg')).toEqual(true);
            expect(expectedTag.hasClass('active-text')).toEqual(true);
        });
        it('should have soil svg', () => {
            const target = mount(<SampleTypeSelection sampleType="Water" onSampleSelected={null} />);
            const expectedTag = target.find('div').at(1).childAt(2).find('SVG');
            //console.log(target.find('div').at(1).childAt(0).debug());
            expect(expectedTag.length).toEqual(1);
            expect(expectedTag.find('title').at(0).text()).toEqual('water');
        });
        it('should have h3', () => {
            const target = mount(<SampleTypeSelection sampleType="Water" onSampleSelected={null} />);
            const expectedTag = target.find('div').at(1).childAt(2).find('h3');
            //console.log(target.find('div').at(1).childAt(0).debug());
            expect(expectedTag.length).toEqual(1);
            expect(expectedTag.text()).toEqual('Water');
        });

        it('should call on click with soil parameter', () => {
            const onSampleSelected = jasmine.createSpy('onSampleSelected');
            const target = mount(<SampleTypeSelection sampleType="Water" onSampleSelected={onSampleSelected} />);
            const expectedTag = target.find('div').at(1).childAt(2);
            expectedTag.simulate('click');

            expect(onSampleSelected).toHaveBeenCalled();
            expect(onSampleSelected).toHaveBeenCalledWith('Water');
        });

    });
});
