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

        expect(target.find("textarea").length).toEqual(1);

        target.unmount();
    });

    it("should have a label", () => {
        const target = mount(<AdditionalInfo {...defaultProps} />);

        expect(target.find("label").length).toEqual(1);

        target.unmount();
    });

    it("should have a value of Test1", () => {
        const target = mount(<AdditionalInfo {...defaultProps} value="Test1" />);

        expect(target.find("textarea").prop("value")).toEqual("Test1");

        target.unmount();
    });

    it("should have a value of Test2", () => {
        const target = mount(<AdditionalInfo {...defaultProps} value="Test2" />);

        expect(target.find("textarea").prop("value")).toEqual("Test2");

        target.unmount();
    });

    it("should call handleChange with state.internalValue on change event", () => {
        const handleChange = jasmine.createSpy("handleChange");
        const target = mount(<AdditionalInfo {...defaultProps} handleChange={handleChange}/>);
        target.setState({ internalValue: "tes" });

        target.find("textarea").simulate("change", { target: { value: "test" } });

        expect(handleChange).toHaveBeenCalled();
        expect(handleChange).toHaveBeenCalledWith("additionalInfo", "test");

        target.unmount();
    });
});
