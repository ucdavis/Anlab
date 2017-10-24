import { mount, render, shallow } from "enzyme";
import * as React from "react";
import { AdditionalInfo, IAdditionalInfoProps } from "../AdditionalInfo";

describe("<AdditionalInfo />", () => {

    const defaultProps = {
        handleChange: null,
        name: "",
        value: "",
    } as IAdditionalInfoProps;

    it("should render an input", () => {
        const target = mount(<AdditionalInfo {...defaultProps} />);
        expect(target).toContainReact(<input />);
    });

    it("should have a label", () => {
        const target = mount(<AdditionalInfo {...defaultProps} />);
        expect(target).toContainReact(<label>Additional Information</label>);
    });

    it("should have a value of Test1", () => {
        const handleChange = jasmine.createSpy("handleChange");
        const target = mount(<AdditionalInfo {...defaultProps} />);
        const input = target.find("Input");
        expect(input.prop("value")).toEqual("Test1");
    });

    it("should have a value of Test2", () => {
        const handleChange = jasmine.createSpy("handleChange");
        const target = mount(<AdditionalInfo {...defaultProps} />);
        const input = target.find("Input");
        expect(input.prop("value")).toEqual("Test2");
    });

    it("should call handleChange with state.internalValue on blur event", () => {
        const handleChange = jasmine.createSpy("handleChange");
        const target = shallow(<AdditionalInfo {...defaultProps} handleChange={handleChange}/>);
        const internal = target.instance();

        internal.state.internalValue = 'test';
        internal._onBlur();

        expect(handleChange).toHaveBeenCalled();
        expect(handleChange).toHaveBeenCalledWith("additionalInfo", "test");
    });
});
