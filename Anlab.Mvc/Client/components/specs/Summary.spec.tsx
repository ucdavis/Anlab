import { mount, render, shallow } from "enzyme";
import * as React from "react";
import Summary, { ISummaryProps } from "../Summary";
import { ITestItem } from "../TestList";

describe("<Summary />", () => {
    const selectedTests: ITestItem[] = [{
        analysis: "1ABC",
        categories: ["Cat1"],
        category: "Cat1",
        externalCost: 3.03,
        externalSetupCost: 7,
        id: "1C-ABC",
        internalCost: 2.02,
        internalSetupCost: 5,
        notes: "",
        sop: "123",
    }, {
        analysis: "2ABC",
        categories: ["Cat2"],
        category: "Cat2",
        externalCost: 4.03,
        externalSetupCost: 7,
        id: "2C-ABC",
        internalCost: 1.01,
        internalSetupCost: 6,
        notes: "",
        sop: "123",
    }];

    const defaultProps: ISummaryProps = {
        canSubmit: false,
        clientType: "",
        focusInput: null,
        hideError: false,
        isCreate: true,
        onSubmit: null,
        processingFee: null,
        project: null,
        projectRef: null,
        quantity: 0,
        quantityRef: null,
        sampleType: null,
        selectedTests,
        status: "",
        waterPreservativeAdded: null,
        waterPreservativeInfo: null,
        waterPreservativeRef: null,
    };

    describe("Rendering", () => {
        it("should render nothing if no tests are selected", () => {
            const target = mount(<Summary {...defaultProps} quantity={1} clientType="other" selectedTests={[]} />);

            expect(target.find("div").length).toEqual(0);
        });
        it("should render something if tests are selected", () => {
            const target = mount(<Summary {...defaultProps} quantity={1} clientType="other" />);

            expect(target.find("div").length).toBeGreaterThan(0);
        });
    });

    describe("Internal functions", () => {
        describe("totalCost internal function", () => {
            it("should add up the cost with internal, quantity 1, no grind/filter/foreign", () => {
                const target = shallow(<Summary {...defaultProps} quantity={1} clientType="uc" />);

                const internal = target.instance();
                expect(internal._totalCost()).toEqual(14.03);
            });
            it("should add up the cost with internal, quantity 3, no grind/filter/foreign", () => {
                const target = shallow(<Summary {...defaultProps} quantity={3} clientType="uc" />);

                const internal = target.instance();
                expect(internal._totalCost()).toEqual(20.09); // (3 * 3.03) + 11
            });
            it("should add up the cost with external, quantity 1, no grind/filter/foreign", () => {
                const target = shallow(<Summary {...defaultProps} quantity={1} clientType="other" />);

                const internal = target.instance();
                expect(internal.totalCost()).toEqual(18.06); // (7.06 + 11 )
            });
            it("should add up the cost with external, quantity 3, no grind/filter/foreign", () => {
                const target = shallow(<Summary {...defaultProps} quantity={3} clientType="other" />);

                const internal = target.instance();
                expect(internal.totalCost()).toEqual(32.18); // (3 * 7.06) + 11
            });
        });
    });
});
