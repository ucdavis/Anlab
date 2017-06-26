import * as React from 'react';
import { shallow, mount, render } from 'enzyme';
import { SampleTypeSelection } from '../SampleTypeSelection';

describe('<SampleTypeSelection/>', () => {
    it('should render', () => {
        const target = mount(<SampleTypeSelection sampleType="soil" onSampleSelected={null} />);
        expect(target.find('div').length).toBeGreaterThan(0);
    });

    it('should render first div with className form_wrap', () => {
        const target = mount(<SampleTypeSelection sampleType="soil" onSampleSelected={null} />);
        expect(target.find('div').at(0).hasClass('form_wrap')).toBe(true);
    });
    it('should have an h2', () => {
        const target = mount(<SampleTypeSelection sampleType="soil" onSampleSelected={null} />);
        const expectedTag = target.find('div').at(0).find('h2');
        expect(expectedTag.length).toEqual(1);
    });
    it('should have an h2 with expected className', () => {
        const target = mount(<SampleTypeSelection sampleType="soil" onSampleSelected={null} />);
        const expectedTag = target.find('div').at(0).find('h2');
        expect(expectedTag.hasClass('form_header')).toEqual(true);
    });
    it('should have an h2 with expected text', () => {
        const target = mount(<SampleTypeSelection sampleType="soil" onSampleSelected={null} />);
        const expectedTag = target.find('div').at(0).find('h2');
        expect(expectedTag.text()).toEqual('What type of samples?');
    });

    it('should have an inner div with a className of row', () => {
        const target = mount(<SampleTypeSelection sampleType="soil" onSampleSelected={null} />);
        const expectedTag = target.find('div').at(1);
        expect(expectedTag.hasClass('row')).toEqual(true);
    });

    describe('Soil selector', () => {
        it('should have common classNames 1', () => {
            const target = mount(<SampleTypeSelection sampleType="xxx" onSampleSelected={null} />);
            const expectedTag = target.find('div').at(1).childAt(0);
            expect(expectedTag.hasClass('anlab_form_style')).toEqual(true);
            expect(expectedTag.hasClass('anlab_form_samplebtn')).toEqual(true);
            expect(expectedTag.hasClass('col')).toEqual(true);
            expect(expectedTag.hasClass('t-center')).toEqual(true);
        });
        it('should have common classNames 2', () => {
            const target = mount(<SampleTypeSelection sampleType="Soil" onSampleSelected={null} />);
            const expectedTag = target.find('div').at(1).childAt(0);
            expect(expectedTag.hasClass('anlab_form_style')).toEqual(true);
            expect(expectedTag.hasClass('anlab_form_samplebtn')).toEqual(true);
            expect(expectedTag.hasClass('col')).toEqual(true);
            expect(expectedTag.hasClass('t-center')).toEqual(true);
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
    });
});