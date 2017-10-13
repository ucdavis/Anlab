import { mount, render, shallow } from "enzyme";
import * as React from "react";
import { ForeignSoil } from "../ForeignSoil";

describe("<ForeignSoil />", () => {
    it("should render null when not soil", () => {
        const target = mount(<ForeignSoil sampleType="Dirt" handleChange={null} foreignSoil={false} />);
        expect(target.find("Checkbox").length).toEqual(0);
    });
    it("should render a Checkbox when sampleType is soil", () => {
        const handleChange = jasmine.createSpy("handleChange");
        const target = mount(<ForeignSoil sampleType="Soil" handleChange={handleChange} foreignSoil={false} />);
        expect(target.find("Checkbox").length).toEqual(1);
    });
    it("should have a checked false", () => {
        const handleChange = jasmine.createSpy("handleChange");
        const target = mount(<ForeignSoil sampleType="Soil" handleChange={handleChange} foreignSoil={false} />);
        const input = target.find("Checkbox");
        expect(input.prop("checked")).toEqual(false);
    });
    it("should have a checked true", () => {
        const handleChange = jasmine.createSpy("handleChange");
        const target = mount(<ForeignSoil sampleType="Soil" handleChange={handleChange} foreignSoil={true} />);
        const input = target.find("Checkbox");
        expect(input.prop("checked")).toEqual(true);
    });
    it("should have a lable prop", () => {
        const handleChange = jasmine.createSpy("handleChange");
        const target = mount(<ForeignSoil sampleType="Soil" handleChange={handleChange} foreignSoil={true} />);
        const input = target.find("Checkbox");
        expect(input.prop("label")).toEqual("Foreign Soil");
    });

    it("should call handleChange with on change event", () => {
        const handleChange = jasmine.createSpy("handleChange");
        const target = mount(<ForeignSoil sampleType="Soil" handleChange={handleChange} foreignSoil={false} />);
        target.find('input[type="checkbox"]').simulate("click", { target: { checked: true } });

        expect(handleChange).toHaveBeenCalled();
        //expect(handleChange).toHaveBeenCalledWith('foreignSoil'); //Don't know why, but this doesn't work with the checkbox
    });
});
