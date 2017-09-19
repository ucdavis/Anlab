import * as React from 'react';
import { shallow, mount, render } from 'enzyme';
import { AdditionalEmails } from '../AdditionalEmails';

describe('<AdditionalEmails />', () => {
    it('should render an input', () => {
        const target = mount(<AdditionalEmails addedEmails={[]} onDeleteEmail={null} onEmailAdded={null} defaultEmail={null} />);
        expect(target.find('input').length).toEqual(1);
    });
    it('should render a label', () => {
        const target = mount(<AdditionalEmails addedEmails={[]} onDeleteEmail={null} onEmailAdded={null} defaultEmail={null} />);
        expect(target.find('label').length).toEqual(2);
    });
    it('should render existing emails', () => {
        const target = mount(<AdditionalEmails addedEmails={["test1@testy.com", "test2@testy.com", "test3@testy.com"]} onDeleteEmail={null} onEmailAdded={null} defaultEmail={null}/>);
        expect(target.find('Button').length).toEqual(3);
    });
    it('should render existing emails1', () => {
        const target = mount(<AdditionalEmails addedEmails={["test1@testy.com", "test2@testy.com", "test3@testy.com"]} onDeleteEmail={null} onEmailAdded={null} defaultEmail={null}/>);
        const target2 = target.first('Button');
        expect(target2.text()).toContain("test1@testy.com");
    });
    it('should render existing emails2', () => {
        const target = mount(<AdditionalEmails addedEmails={["test1@testy.com", "test2@testy.com", "test3@testy.com"]} onDeleteEmail={null} onEmailAdded={null} defaultEmail={null}/>);
        const target2 = target.first('Button');
        expect(target2.text()).toContain("test2@testy.com");
    });
    it('should render existing emails3', () => {
        const target = mount(<AdditionalEmails addedEmails={["test1@testy.com", "test2@testy.com", "test3@testy.com"]} onDeleteEmail={null} onEmailAdded={null} defaultEmail={null}/>);
        const target2 = target.first('Button');
        expect(target2.text()).toContain("test3@testy.com");
    });
    it('should render existing emails4', () => {
        const target = mount(<AdditionalEmails addedEmails={["test1@testy.com", "test2@testy.com", "test3@testy.com"]} onDeleteEmail={null} onEmailAdded={null} defaultEmail={null}/>);
        const target2 = target.first('Button');
        expect(target2.text()).not.toContain("test4@testy.com");
    });

    it('should set internal email value', () => {
        const target = mount(<AdditionalEmails addedEmails={[]} onDeleteEmail={null} onEmailAdded={null} defaultEmail={null}/>);
        const internal = target.instance();

        expect(internal.state.email).toBe('');

        internal.onEmailChanged('xxx');

        expect(internal.state.email).toBe('xxx');
    });

    it('should call onEmailAdded with valid state.email onClick event', () => {
        const onEmailAdded = jasmine.createSpy('onEmailAdded');
        const target = shallow(<AdditionalEmails addedEmails={[]} onDeleteEmail={null} onEmailAdded={onEmailAdded} defaultEmail={null}/>);
        const internal = target.instance();

        internal.state.email = 'test@testy.com';
        internal.onClick();

        expect(onEmailAdded).toHaveBeenCalled();
        expect(onEmailAdded).toHaveBeenCalledWith('test@testy.com');
        expect(internal.state.email).toBe('');
    });

    it('should call onEmailAdded with valid state.email onClick event and lower it', () => {
        const onEmailAdded = jasmine.createSpy('onEmailAdded');
        const target = shallow(<AdditionalEmails addedEmails={[]} onDeleteEmail={null} onEmailAdded={onEmailAdded} defaultEmail={null}/>);
        const internal = target.instance();

        internal.state.email = 'TEST@testy.COM';
        internal.onClick();

        expect(onEmailAdded).toHaveBeenCalled();
        expect(onEmailAdded).toHaveBeenCalledWith('test@testy.com');
        expect(internal.state.email).toBe('');
    });

    it('should not call onEmailAdded with invalid state.email onClick event', () => {
        const onEmailAdded = jasmine.createSpy('onEmailAdded');
        spyOn(window, 'alert');
        const target = shallow(<AdditionalEmails addedEmails={[]} onDeleteEmail={null} onEmailAdded={onEmailAdded} defaultEmail={null}/>);
        const internal = target.instance();

        internal.state.email = 'test@invlid@invalid.com';
        internal.onClick();

        expect(onEmailAdded).not.toHaveBeenCalled();
        expect(internal.state.email).toBe('test@invlid@invalid.com');
        expect(window.alert).toHaveBeenCalledWith('Invalid email');
    });

    it('should not call onEmailAdded with duplicate state.email onClick event', () => {
        const onEmailAdded = jasmine.createSpy('onEmailAdded');
        spyOn(window, 'alert');
        const target = shallow(<AdditionalEmails addedEmails={["test1@testy.com", "test2@testy.com", "test3@testy.com"]} onDeleteEmail={null} onEmailAdded={onEmailAdded} defaultEmail={null}/>);
        const internal = target.instance();

        internal.state.email = 'test2@testy.com';
        internal.onClick();

        expect(onEmailAdded).not.toHaveBeenCalled();
        expect(internal.state.email).toBe('');
        expect(window.alert).toHaveBeenCalledWith('Email already added');
    });

    it('should not call onEmailAdded with duplicate ignoring case state.email onClick event', () => {
        const onEmailAdded = jasmine.createSpy('onEmailAdded');
        spyOn(window, 'alert');
        const target = shallow(<AdditionalEmails addedEmails={["test1@testy.com", "test2@testy.com", "test3@testy.com"]} onDeleteEmail={null} onEmailAdded={onEmailAdded} defaultEmail={null}/>);
        const internal = target.instance();

        internal.state.email = 'TEST2@testy.com';
        internal.onClick();

        expect(onEmailAdded).not.toHaveBeenCalled();
        expect(internal.state.email).toBe('');
        expect(window.alert).toHaveBeenCalledWith('Email already added');
    });


    it('should call onDeleteEmail with onDelete event', () => {
        const onDeleteEmail = jasmine.createSpy('onDeleteEmail');
        const target = shallow(<AdditionalEmails addedEmails={["test1@testy.com", "test2@testy.com", "test3@testy.com"]} onDeleteEmail={onDeleteEmail} onEmailAdded={null} defaultEmail={null}/>);
        const internal = target.instance();

        internal.state.email = 'test@testy.com';
        internal.onDelete('test2@testy.com');

        expect(onDeleteEmail).toHaveBeenCalled();
        expect(onDeleteEmail).toHaveBeenCalledWith('test2@testy.com');
    });
});