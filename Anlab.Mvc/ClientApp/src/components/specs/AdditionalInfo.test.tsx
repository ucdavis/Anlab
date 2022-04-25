import * as React from "react";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";

import { AdditionalInfo, IAdditionalInfoProps } from "../AdditionalInfo";

describe("<AdditionalInfo />", () => {
  const defaultProps = {
    handleChange: null,
    name: "",
    value: "",
  } as IAdditionalInfoProps;

  it("should render an input", () => {
    render(<AdditionalInfo {...defaultProps} />);

    expect(screen.getByRole("textbox")).toBeInTheDocument();
  });

  it("should have a value of Test1", () => {
    render(<AdditionalInfo {...defaultProps} value="Test1" />);

    expect(screen.getByRole("textbox")).toHaveValue("Test1");
  });

  it("should call handleChange with on change event", async () => {
    const handleChange = jest.fn() as (value: string) => void;

    render(<AdditionalInfo {...defaultProps} handleChange={handleChange} />);

    const user = userEvent.setup();

    await user.type(screen.getByRole("textbox"), "Test1");

    expect(handleChange).toHaveBeenCalled();
  });
});
