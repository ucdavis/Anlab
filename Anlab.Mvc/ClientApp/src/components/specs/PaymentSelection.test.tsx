import * as React from "react";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";

import { PaymentSelection } from "../PaymentSelection";
import { IPayment } from "../PaymentUcSelection";

const fakeOtherPaymentInfo = {
  paymentType:
    "Tenetur eius aspernatur asperiores et iusto non. Sequi inventore assumenda itaque ad. Ab mollitia non.",
  companyName:
    "Animi architecto unde non est asperiores. Hic ex ut quis sit. Soluta quis facere repudiandae quas rerum eligendi. Adipisci animi doloremque error laboriosam tempora illo. Veniam modi magni aut illo sint inventore quasi beatae. Laudantium aut quae dolor maiores incidunt.",
  acName: "deserunt",
  acAddr: "Molestias ab ex voluptates totam.",
  acEmail: "Quidem nihil quae.",
  acPhone:
    "Minus dolorem fuga minima perferendis omnis dolor.\nAut ipsa sit quidem molestias mollitia qui.\nId accusamus odit quae eos nemo.",
  poNum: "commodi odio dicta",
  agreementRequired: true,
};

describe("<PaymentSelection/>", () => {
  it("should render", () => {
    const onPaymentSelected = jest.fn() as (payment: IPayment) => void;
    const payment = { clientType: "uc", account: "" };
    render(
      <PaymentSelection
        payment={payment}
        onPaymentSelected={onPaymentSelected}
        ucAccountRef={null}
        creatingOrder={true}
        placingOrder={true}
        checkChart={null}
        otherPaymentInfo={fakeOtherPaymentInfo}
        otherPaymentInfoRef={null}
        updateOtherPaymentInfo={null}
        updateOtherPaymentInfoType={null}
        changeSelectedUc={null}
      />
    );
    expect(screen.getByText("UC Funds")).toBeInTheDocument();
  });
  describe("<Input /> (Uc Account Entry)", () => {
    it("should not render when creditcard payment method", () => {
      const onPaymentSelected = jest.fn() as (payment: IPayment) => void;
      const payment = { clientType: "creditcard", account: "" };
      render(
        <PaymentSelection
          payment={payment}
          onPaymentSelected={onPaymentSelected}
          ucAccountRef={null}
          creatingOrder={true}
          placingOrder={true}
          checkChart={null}
          otherPaymentInfo={null}
          otherPaymentInfoRef={null}
          updateOtherPaymentInfo={null}
          updateOtherPaymentInfoType={null}
          changeSelectedUc={null}
        />
      );
      expect(screen.queryByRole("textbox")).not.toBeInTheDocument();
    });
    it("should render when uc payment method", () => {
      const onPaymentSelected = jest.fn() as (payment: IPayment) => void;
      const payment = { clientType: "uc", account: "" };
      render(
        <PaymentSelection
          payment={payment}
          onPaymentSelected={onPaymentSelected}
          ucAccountRef={null}
          creatingOrder={true}
          placingOrder={true}
          checkChart={null}
          otherPaymentInfo={fakeOtherPaymentInfo}
          otherPaymentInfoRef={null}
          updateOtherPaymentInfo={null}
          updateOtherPaymentInfoType={null}
          changeSelectedUc={null}
        />
      );
      // look for our UC select box
      expect(screen.getByRole("combobox")).toBeInTheDocument();
    });

    it("should call checkChart when the account is changed", async () => {
      const onPaymentSelected = jest.fn() as (payment: IPayment) => void;
      const checkChart = jest.fn() as (account: string) => void;
      const payment = { clientType: "uc", account: "123" };

      render(
        <PaymentSelection
          payment={payment}
          onPaymentSelected={onPaymentSelected}
          ucAccountRef={null}
          creatingOrder={true}
          placingOrder={true}
          checkChart={checkChart}
          otherPaymentInfo={fakeOtherPaymentInfo}
          otherPaymentInfoRef={null}
          updateOtherPaymentInfo={null}
          updateOtherPaymentInfoType={null}
          changeSelectedUc={null}
        />
      );

      const user = userEvent.setup();

      // find account box and type something in
      await user.type(screen.getByDisplayValue("123"), "xxx");

      expect(checkChart).toHaveBeenCalled();
    });
  });

  describe("Credit Card Selection div", () => {
    const onPaymentSelected = jest.fn() as (payment: IPayment) => void;
    it("should add active class if selected", () => {
      const payment = { clientType: "creditcard", account: "" };
      render(
        <PaymentSelection
          payment={payment}
          onPaymentSelected={onPaymentSelected}
          ucAccountRef={null}
          creatingOrder={true}
          placingOrder={true}
          checkChart={null}
          otherPaymentInfo={null}
          otherPaymentInfoRef={null}
          updateOtherPaymentInfo={null}
          updateOtherPaymentInfoType={null}
          changeSelectedUc={null}
        />
      );

      const creditCardSelection = screen.getByText("Credit Card");

      expect(creditCardSelection.parentElement.classList).toContain(
        "active-text"
      );
    });
  });

  describe("UC Selection div", () => {
    const onPaymentSelected = jest.fn() as (payment: IPayment) => void;
    it("should add active class if selected", () => {
      const payment = { clientType: "uc", account: "" };
      render(
        <PaymentSelection
          payment={payment}
          onPaymentSelected={onPaymentSelected}
          ucAccountRef={null}
          creatingOrder={true}
          placingOrder={true}
          checkChart={null}
          otherPaymentInfo={fakeOtherPaymentInfo}
          otherPaymentInfoRef={null}
          updateOtherPaymentInfo={null}
          updateOtherPaymentInfoType={null}
          changeSelectedUc={null}
        />
      );

      const creditCardSelection = screen.getByText("UC Funds");

      expect(creditCardSelection.parentElement.classList).toContain(
        "active-text"
      );
    });
  });

  describe("other div", () => {
    const onPaymentSelected = jest.fn() as (payment: IPayment) => void;
    it("should add active class if selected", () => {
      const payment = { clientType: "other", account: "" };
      render(
        <PaymentSelection
          payment={payment}
          onPaymentSelected={onPaymentSelected}
          ucAccountRef={null}
          creatingOrder={true}
          placingOrder={true}
          checkChart={null}
          otherPaymentInfo={fakeOtherPaymentInfo}
          otherPaymentInfoRef={null}
          updateOtherPaymentInfo={null}
          updateOtherPaymentInfoType={null}
          changeSelectedUc={null}
        />
      );

      const creditCardSelection = screen.getByText("Other");

      expect(creditCardSelection.parentElement.classList).toContain(
        "active-text"
      );
    });

    it("should ask for company name if selected", () => {
      const payment = { clientType: "other", account: "" };
      render(
        <PaymentSelection
          payment={payment}
          onPaymentSelected={onPaymentSelected}
          ucAccountRef={null}
          creatingOrder={true}
          placingOrder={true}
          checkChart={null}
          otherPaymentInfo={fakeOtherPaymentInfo}
          otherPaymentInfoRef={null}
          updateOtherPaymentInfo={null}
          updateOtherPaymentInfoType={null}
          changeSelectedUc={null}
        />
      );

      expect(
        screen.getByDisplayValue(fakeOtherPaymentInfo.acEmail)
      ).toBeInTheDocument();
    });
  });

  it("should call onPaymentSelected when the payment method is changed", async () => {
    const onPaymentSelected = jest.fn() as (payment: IPayment) => void;
    const payment = { clientType: "creditcard", account: "" };
    render(
      <PaymentSelection
        payment={payment}
        onPaymentSelected={onPaymentSelected}
        ucAccountRef={null}
        creatingOrder={true}
        placingOrder={true}
        checkChart={null}
        otherPaymentInfo={null}
        otherPaymentInfoRef={null}
        updateOtherPaymentInfo={null}
        updateOtherPaymentInfoType={null}
        changeSelectedUc={null}
      />
    );

    const user = userEvent.setup();

    const ucFundButton = screen.getByText("UC Funds");

    // click on the UC Funds button
    await user.click(ucFundButton);

    expect(onPaymentSelected).toHaveBeenCalled();
  });
});
