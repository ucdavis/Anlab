import * as React from 'react';
import { shallow, mount, render } from 'enzyme';
import { AdditionalEmails } from '../AdditionalEmails';

describe('<AdditionalEmails />', () => {
    it('should render an input', () => {
        const target = mount(<AdditionalEmails addedEmails={[]} onDeleteEmail={null} onEmailAdded={null} />);
        expect(target.find('input').length).toEqual(1);
    });
    it('should render a Button', () => {
        const target = mount(<AdditionalEmails addedEmails={[]} onDeleteEmail={null} onEmailAdded={null} />);
        expect(target.find('Button').length).toEqual(1);
    });
    it('should render a label', () => {
        const target = mount(<AdditionalEmails addedEmails={[]} onDeleteEmail={null} onEmailAdded={null} />);
        expect(target.find('label').length).toEqual(2);
    });
    it('should render a label for additional Emails', () => {
        const target = mount(<AdditionalEmails addedEmails={["1@test.com", "2@test.com"]} onDeleteEmail={null} onEmailAdded={null} />);
        expect(target.find('label').length).toEqual(4);
    });
});