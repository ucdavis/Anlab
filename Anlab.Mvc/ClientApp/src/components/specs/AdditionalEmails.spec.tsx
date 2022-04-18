import { mount, render, shallow } from "enzyme";
import * as React from "react";
import { Badge } from "react-bootstrap";
import { AdditionalEmails, IAdditionalEmailsProps } from "../AdditionalEmails";

describe("<AdditionalEmails />", () => {
    const defaultProps = {
        addedEmails: [],
        defaultEmail: "",
        onDeleteEmail: null,
        onEmailAdded: null,
    } as IAdditionalEmailsProps;

    it("should render a default email", () => {
        const target = mount(
            <AdditionalEmails
                {...defaultProps}
                defaultEmail={"default@example.com"}
            />);

        expect(target).toContainReact(<Badge>default@example.com</Badge>);
    });

    it("should render existing emails", () => {
        const target = mount(
            <AdditionalEmails
                {...defaultProps}
                addedEmails={["test1@testy.com", "test2@testy.com", "test3@testy.com"]}
            />);

        expect(target.find('.badge').at(1)).toHaveText("test1@testy.com");
        expect(target.find('.badge').at(2)).toHaveText("test2@testy.com");
        expect(target.find('.badge').at(3)).toHaveText("test3@testy.com");
        //expect(target).toContainReact(<Badge>test1@testy.com</Badge>);
        //expect(target).toContainReact(<Badge>test2@testy.com</Badge>);
        //expect(target).toContainReact(<Badge>test3@testy.com</Badge>);
    });

    it("should set state email value on change", () => {
        const target = mount(<AdditionalEmails {...defaultProps} />);
        target.setState({ toggle: true });
        expect(target.state().email).toBe("");

        // act
        target.find("input").simulate("change", { target: { value: "xxx" } });

        expect(target.state().email).toBe("xxx");
    });

    it("should call onEmailAdded with valid state.email onBlur event", () => {
        const onEmailAdded = jasmine.createSpy("onEmailAdded");
        const target = shallow(
            <AdditionalEmails
                {...defaultProps}
                onEmailAdded={onEmailAdded}
            />);
        target.setState({ toggle: true, email: "test@testy.com" });

        // act
        target.find("input").simulate("blur");

        expect(onEmailAdded).toHaveBeenCalled();
        expect(onEmailAdded).toHaveBeenCalledWith("test@testy.com");
        expect(target.state().hasError).toBe(false);
        expect(target.state().errorText).toBe("");
    });

    it("should call onEmailAdded with valid state.email onClick event and lower it", () => {
        const onEmailAdded = jasmine.createSpy("onEmailAdded");
        const target = shallow(
            <AdditionalEmails
                {...defaultProps}
                onEmailAdded={onEmailAdded}
            />);
        target.setState({ toggle: true, email: "TEST@testy.com" });

        // act
        target.find("input").simulate("blur");

        expect(onEmailAdded).toHaveBeenCalled();
        expect(onEmailAdded).toHaveBeenCalledWith("test@testy.com");
        expect(target.state().hasError).toBe(false);
        expect(target.state().errorText).toBe("");
    });

    it("should not call onEmailAdded with invalid state.email onClick event", () => {
        const onEmailAdded = jasmine.createSpy("onEmailAdded");
        const target = shallow(
            <AdditionalEmails
                {...defaultProps}
                onEmailAdded={onEmailAdded}
            />);
        target.setState({ toggle: true, email: "test@invlid@invalid.com" });

        // act
        target.find("input").simulate("blur");

        expect(onEmailAdded).not.toHaveBeenCalled();
        expect(target.state().toggle).toBe(true);
        expect(target.state().email).toBe("test@invlid@invalid.com");
        expect(target.state().hasError).toBe(true);
        expect(target.state().errorText).toBe("Invalid email");
    });

    it("should not call onEmailAdded with duplicate state.email onClick event", () => {
        const onEmailAdded = jasmine.createSpy("onEmailAdded");
        const target = shallow(
            <AdditionalEmails
                {...defaultProps}
                addedEmails={["test1@testy.com", "test2@testy.com", "test3@testy.com"]}
                onEmailAdded={onEmailAdded}
            />);
        target.setState({ toggle: true, email: "test2@testy.com" });

        // act
        target.find("input").simulate("blur");

        expect(onEmailAdded).not.toHaveBeenCalled();
        expect(target.state().email).toBe("test2@testy.com");
        expect(target.state().hasError).toBe(true);
        expect(target.state().errorText).toBe("Email already added");
    });

    it("should not call onEmailAdded with duplicate ignoring case state.email onClick event", () => {
        const onEmailAdded = jasmine.createSpy("onEmailAdded");
        const target = shallow(
            <AdditionalEmails
                {...defaultProps}
                addedEmails={["test1@testy.com", "test2@testy.com", "test3@testy.com"]}
                onEmailAdded={onEmailAdded}
            />);
        target.setState({ toggle: true, email: "TEST1@testy.com" });

        // act
        target.find("input").simulate("blur");

        expect(onEmailAdded).not.toHaveBeenCalled();
        expect(target.state().email).toBe("TEST1@testy.com");
        expect(target.state().hasError).toBe(true);
        expect(target.state().errorText).toBe("Email already added");
    });
});
