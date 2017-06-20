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
    it('should render existing emails', () => {
        const target = mount(<AdditionalEmails addedEmails={["test1@testy.com", "test2@testy.com", "test3@testy.com"]} onDeleteEmail={null} onEmailAdded={null} />);
        expect(target.find('Button').length).toEqual(4);
    });
    it('should render existing emails1', () => {
        const target = mount(<AdditionalEmails addedEmails={["test1@testy.com", "test2@testy.com", "test3@testy.com"]} onDeleteEmail={null} onEmailAdded={null} />);
        const target2 = target.first('Button');
        expect(target2.text()).toContain("test1@testy.com");
    });
    it('should render existing emails2', () => {
        const target = mount(<AdditionalEmails addedEmails={["test1@testy.com", "test2@testy.com", "test3@testy.com"]} onDeleteEmail={null} onEmailAdded={null} />);
        const target2 = target.first('Button');
        expect(target2.text()).toContain("test2@testy.com");
    });
    it('should render existing emails3', () => {
        const target = mount(<AdditionalEmails addedEmails={["test1@testy.com", "test2@testy.com", "test3@testy.com"]} onDeleteEmail={null} onEmailAdded={null} />);
        const target2 = target.first('Button');
        expect(target2.text()).toContain("test3@testy.com");
    });
    it('should render existing emails4', () => {
        const target = mount(<AdditionalEmails addedEmails={["test1@testy.com", "test2@testy.com", "test3@testy.com"]} onDeleteEmail={null} onEmailAdded={null} />);
        const target2 = target.first('Button');
        expect(target2.text()).not.toContain("test4@testy.com");
    });
});