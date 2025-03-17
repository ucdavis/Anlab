import * as React from "react";
import { render, screen } from "@testing-library/react";

import Summary, { ISummaryProps } from "../Summary";
import { ITestItem } from "../TestList";

describe("<Summary />", () => {
  const selectedTests: ITestItem[] = [
    {
      analysis: "1ABC",
      categories: ["Cat1"],
      category: "Cat1",
      externalCost: 3.03,
      externalSetupCost: 7,
      id: "1C-ABC",
      internalCost: 2.02,
      internalSetupCost: 5,
      notes: "",
      dryMatter: false,
      sop: "123",
    },
    {
      analysis: "2ABC",
      categories: ["Cat2"],
      category: "Cat2",
      externalCost: 4.03,
      externalSetupCost: 7,
      id: "2C-ABC",
      internalCost: 1.01,
      internalSetupCost: 6,
      notes: "",
      dryMatter: false,
      sop: "123",
    },
  ];

  const defaultProps: ISummaryProps = {
    canSubmit: false,
    clientType: "",
    handleErrors: null,
    hideError: false,
    isCreate: true,
    onSubmit: null,
    isSubmitting: false,
    processingFee: null,
    quantity: 0,
    selectedTests,
    status: "",
    placingOrder: true,
    switchViews: null,
  };

  describe("Rendering", () => {
    it("should render nothing if no tests are selected", () => {
      render(
        <Summary
          {...defaultProps}
          quantity={1}
          clientType="other"
          selectedTests={[]}
        />
      );

      expect(screen.queryByText("Order Details")).not.toBeInTheDocument();
    });
    it("should render something if tests are selected", () => {
      render(<Summary {...defaultProps} quantity={1} clientType="other" />);

      expect(screen.queryByText("Order Details")).toBeInTheDocument();
    });
  });

  describe("Internal functions", () => {
    describe("totalCost internal function", () => {
      it("should add up the cost with internal, quantity 1, no grind/filter/foreign", () => {
        render(<Summary {...defaultProps} quantity={1} clientType="uc" />);

        // (1 * (1.01 + 2.02)) + (5 + 6)
        expect(screen.getByRole("heading")).toHaveTextContent(
          "Order Total: $14.03"
        );
      });
      it("should add up the cost with internal, quantity 3, no grind/filter/foreign", () => {
        render(<Summary {...defaultProps} quantity={3} clientType="uc" />);

        // (3 * (1.01 + 2.02)) + (5 + 6)
        expect(screen.getByRole("heading")).toHaveTextContent(
          "Order Total: $20.09"
        );
      });
      it("should add up the cost with external, quantity 1, no grind/filter/foreign", () => {
        render(<Summary {...defaultProps} quantity={1} clientType="other" />);

        // (1 * (3.03 + 4.03)) + (7 + 7)
        expect(screen.getByRole("heading")).toHaveTextContent(
          "Order Total: $21.06"
        );
      });
      it("should add up the cost with external, quantity 3, no grind/filter/foreign", () => {
        render(<Summary {...defaultProps} quantity={3} clientType="other" />);

        // (3 * (3.03 + 4.03)) + (7 + 7)
        expect(screen.getByRole("heading")).toHaveTextContent(
          "Order Total: $35.18"
        );
      });
    });
  });
});
