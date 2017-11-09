import { mount, render, shallow } from "enzyme";
import * as React from "react";

import { IntegerInput } from "../ui/integerInput/integerInput";

describe("<IntegerInput />", () => {
    it("should render an input", () => {
        const target = mount(<IntegerInput />);
        expect(target.find("input").length).toEqual(1);
    });

    it("should clear error on good value", () => {
        const target = mount(<IntegerInput min={10} max={20} />);
        const internal = target.instance();

        const inp = target.find('input');

        inp.simulate('change', { target: { value: 15 } });  

        expect(internal.state.error).toBeNull();
    });

    it("should set error on non-number value", () => {
        const target = mount(<IntegerInput />);
        const internal = target.instance();

        const inp = target.find('input');

        inp.simulate('change', { target: { value: 'ABC' } });  

        expect(internal.state.error).not.toBeNull();
        expect(internal.state.error).toBe("Must be a number.");
    });

    it("should set error on less than min value", () => {
        const target = mount(<IntegerInput min={10} />);
        const internal = target.instance();

        const inp = target.find('input');

        inp.simulate('change', { target: { value: 5 } });  

        expect(internal.state.error).not.toBeNull();
        expect(internal.state.error).toBe("Must be a number greater than 10.");
    });

    it("should set error on more than max value", () => {
        const target = mount(<IntegerInput max={10} />);
        const internal = target.instance();

        const inp = target.find('input');

        inp.simulate('change', { target: { value: 15 } });  

        expect(internal.state.error).not.toBeNull();
        expect(internal.state.error).toBe("Must be a number less than or equal to 10.");
    });
});
