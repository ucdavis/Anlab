import { mount, render, shallow } from "enzyme";
import * as React from "react";
import { AdditionalEmails } from "../AdditionalEmails";

describe("<AdditionalEmails />", () => {
    it("should render a default email", () => {
        const target = mount(
            <AdditionalEmails
                addedEmails={[]}
                onDeleteEmail={null}
                onEmailAdded={null}
                defaultEmail={"default@example.com"}
            />);

        expect(target).toContainReact(<span>default@example.com</span>);
    });

    it("should render existing emails", () => {
        const target = mount(
            <AdditionalEmails
                addedEmails={["test1@testy.com", "test2@testy.com", "test3@testy.com"]}
                onDeleteEmail={null}
                onEmailAdded={null}
                defaultEmail={null}
            />);

        expect(target).toContainReact(<span>test1@testy.com</span>);
        expect(target).toContainReact(<span>test2@testy.com</span>);
        expect(target).toContainReact(<span>test3@testy.com</span>);
    });

    it("should set state email value on change", () => {
        const target = mount(
            <AdditionalEmails
                addedEmails={[]}
                onDeleteEmail={null}
                onEmailAdded={null}
                defaultEmail={null}
            />);
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
                addedEmails={[]}
                onDeleteEmail={null}
                onEmailAdded={onEmailAdded}
                defaultEmail={null}
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
                addedEmails={[]}
                onDeleteEmail={null}
                onEmailAdded={onEmailAdded}
                defaultEmail={null}
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
                addedEmails={[]}
                onDeleteEmail={null}
                onEmailAdded={onEmailAdded}
                defaultEmail={null}
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
                addedEmails={["test1@testy.com", "test2@testy.com", "test3@testy.com"]}
                onDeleteEmail={null}
                onEmailAdded={onEmailAdded}
                defaultEmail={null}
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
                addedEmails={["test1@testy.com", "test2@testy.com", "test3@testy.com"]}
                onDeleteEmail={null}
                onEmailAdded={onEmailAdded}
                defaultEmail={null}
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
