import * as React from "react";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";

import { ORDER_QUANTITY_MAX, Quantity } from "../Quantity";

const QuantityHarness = () => {
  const [quantity, setQuantity] = React.useState<number>();
  return (
    <Quantity
      quantity={quantity}
      onQuantityChanged={setQuantity}
      quantityRef={() => {}}
    />
  );
};

describe("<Quantity />", () => {
  it("should render an IntegerInput", () => {
    render(
      <Quantity quantity={1} onQuantityChanged={null} quantityRef={null} />
    );
    expect(screen.getByRole("textbox")).toBeInTheDocument();
  });

  describe("properties", () => {
    it("should have a max quantity of 200", () => {
      expect(ORDER_QUANTITY_MAX).toBe(200);
    });

    it("should have a name", () => {
      render(
        <Quantity quantity={1} onQuantityChanged={null} quantityRef={null} />
      );
      expect(screen.getByRole("textbox")).toHaveAttribute("name", "quantity");
    });

    it("should have a value 1", () => {
      render(
        <Quantity quantity={1} onQuantityChanged={null} quantityRef={null} />
      );
      expect(screen.getByRole("textbox")).toHaveAttribute("value", "1");
    });
    it("should have a value 2", () => {
      render(
        <Quantity quantity={33} onQuantityChanged={null} quantityRef={null} />
      );
      expect(screen.getByRole("textbox")).toHaveAttribute("value", "33");
    });
    it("should have be required", () => {
      render(
        <Quantity quantity={33} onQuantityChanged={null} quantityRef={null} />
      );
      expect(screen.getByRole("textbox")).toHaveAttribute("required");
    });

    it("should allow 200 samples", async () => {
      render(<QuantityHarness />);

      const user = userEvent.setup();
      await user.type(screen.getByRole("textbox"), String(ORDER_QUANTITY_MAX));

      expect(
        screen.queryByText(
          `Must be a number less than or equal to ${ORDER_QUANTITY_MAX}.`
        )
      ).not.toBeInTheDocument();
    });

    it("should reject more than 200 samples", async () => {
      render(<QuantityHarness />);

      const user = userEvent.setup();
      await user.type(
        screen.getByRole("textbox"),
        String(ORDER_QUANTITY_MAX + 1)
      );

      expect(
        screen.getByText(
          `Must be a number less than or equal to ${ORDER_QUANTITY_MAX}.`
        )
      ).toBeInTheDocument();
    });
  });
});
