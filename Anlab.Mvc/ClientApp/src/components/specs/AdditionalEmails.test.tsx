import * as React from "react";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";

import { AdditionalEmails, IAdditionalEmailsProps } from "../AdditionalEmails";

describe("<AdditionalEmails />", () => {
  const defaultProps = {
    addedEmails: [],
    defaultEmail: "",
    onDeleteEmail: null,
    onEmailAdded: null,
  } as IAdditionalEmailsProps;

  it("should render a default email", () => {
    render(
      <AdditionalEmails
        {...defaultProps}
        defaultEmail={"default@example.com"}
      />
    );

    expect(screen.getByText("default@example.com")).toBeInTheDocument();
  });

  it("should render existing emails", () => {
    render(
      <AdditionalEmails
        {...defaultProps}
        addedEmails={["test1@testy.com", "test2@testy.com", "test3@testy.com"]}
      />
    );

    expect(screen.getByText("test1@testy.com")).toBeInTheDocument();
    expect(screen.getByText("test2@testy.com")).toBeInTheDocument();
    expect(screen.getByText("test3@testy.com")).toBeInTheDocument();
  });

  it("should call onEmailAdded with valid state.email onBlur event", async () => {
    const handleAddEmail = jest.fn() as (value: string) => void;

    render(
      <AdditionalEmails {...defaultProps} onEmailAdded={handleAddEmail} />
    );

    const user = userEvent.setup();

    // click on plus sign, then type in textbox to add email
    await user.click(screen.getByTestId("add-email"));
    await user.type(screen.getByRole("textbox"), "test3@testy.com");
    await user.tab();

    expect(handleAddEmail).toHaveBeenCalledWith("test3@testy.com");
  });

  it("should not call onEmailAdded with invalid state.email onClick event", async () => {
    const handleAddEmail = jest.fn() as (value: string) => void;

    render(
      <AdditionalEmails {...defaultProps} onEmailAdded={handleAddEmail} />
    );

    const user = userEvent.setup();

    // click on plus sign, then type in textbox to add email
    await user.click(screen.getByTestId("add-email"));
    await user.type(screen.getByRole("textbox"), "test3@invalid@testy.com");
    await user.tab();

    expect(handleAddEmail).not.toHaveBeenCalled();
  });

  it("should not call onEmailAdded with duplicate state.email onClick event", async () => {
    const handleAddEmail = jest.fn() as (value: string) => void;

    render(
      <AdditionalEmails
        {...defaultProps}
        addedEmails={["test1@testy.com", "test2@testy.com", "test3@testy.com"]}
        onEmailAdded={handleAddEmail}
      />
    );

    const user = userEvent.setup();

    // click on plus sign, then type in textbox to add email
    await user.click(screen.getByTestId("add-email"));
    await user.type(screen.getByRole("textbox"), "test3@testy.com");
    await user.tab();

    // add email should not be called since it is a duplicate
    expect(handleAddEmail).not.toHaveBeenCalled();
  });

  it("should not call onEmailAdded with duplicate ignoring case state.email onClick event", async () => {
    const handleAddEmail = jest.fn() as (value: string) => void;

    render(
      <AdditionalEmails
        {...defaultProps}
        addedEmails={["test1@testy.com", "test2@testy.com", "test3@testy.com"]}
        onEmailAdded={handleAddEmail}
      />
    );

    const user = userEvent.setup();

    // click on plus sign, then type in textbox to add email
    await user.click(screen.getByTestId("add-email"));
    await user.type(screen.getByRole("textbox"), "TEST3@testy.com");
    await user.tab();

    // add email should not be called since it is a duplicate
    expect(handleAddEmail).not.toHaveBeenCalled();
  });
});
