import * as React from "react";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";

import { SampleTypeSelection } from "../SampleTypeSelection";

describe("<SampleTypeSelection/>", () => {
  it("should render", () => {
    render(<SampleTypeSelection sampleType="soil" onSampleSelected={null} />);
    expect(screen.getByText("Soil")).toBeInTheDocument();
  });

  describe("Soil selector", () => {
    it("should call on click with soil parameter", async () => {
      const onSampleSelected = jest.fn() as (sampleType: string) => void;

      render(
        <SampleTypeSelection
          sampleType="Soil"
          onSampleSelected={onSampleSelected}
        />
      );

      const user = userEvent.setup();
      await user.click(screen.getByText("Soil"));

      expect(onSampleSelected).toHaveBeenCalled();
      expect(onSampleSelected).toHaveBeenCalledWith("Soil");
    });
  });

  describe("Plant selector", () => {
    it("should call on click with Plant parameter", async () => {
      const onSampleSelected = jest.fn() as (sampleType: string) => void;

      render(
        <SampleTypeSelection
          sampleType="Plant"
          onSampleSelected={onSampleSelected}
        />
      );

      const user = userEvent.setup();
      await user.click(screen.getByText("Plant / Feed"));

      expect(onSampleSelected).toHaveBeenCalled();
      expect(onSampleSelected).toHaveBeenCalledWith("Plant");
    });
  });

  describe("Water selector", () => {
    it("should call on click with Water parameter", async () => {
      const onSampleSelected = jest.fn() as (sampleType: string) => void;

      render(
        <SampleTypeSelection
          sampleType="Water"
          onSampleSelected={onSampleSelected}
        />
      );

      const user = userEvent.setup();
      await user.click(screen.getByText("Water / Misc. Liquid"));

      expect(onSampleSelected).toHaveBeenCalled();
      expect(onSampleSelected).toHaveBeenCalledWith("Water");
    });
  });
});
