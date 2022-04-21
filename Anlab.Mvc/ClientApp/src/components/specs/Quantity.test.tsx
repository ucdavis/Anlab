import * as React from 'react';
import { render, screen } from '@testing-library/react';

import { Quantity } from '../Quantity';

describe('<Quantity />', () => {
  it('should render an IntegerInput', () => {
    render(
      <Quantity quantity={1} onQuantityChanged={null} quantityRef={null} />
    );
    expect(screen.getByRole('textbox')).toBeInTheDocument();
  });

  describe('properties', () => {
    it('should have a name', () => {
      render(
        <Quantity quantity={1} onQuantityChanged={null} quantityRef={null} />
      );
      expect(screen.getByRole('textbox')).toHaveAttribute('name', 'quantity');
    });

    it('should have a value 1', () => {
      render(
        <Quantity quantity={1} onQuantityChanged={null} quantityRef={null} />
      );
      expect(screen.getByRole('textbox')).toHaveAttribute('value', '1');
    });
    it('should have a value 2', () => {
      render(
        <Quantity quantity={33} onQuantityChanged={null} quantityRef={null} />
      );
      expect(screen.getByRole('textbox')).toHaveAttribute('value', '33');
    });
    it('should have be required', () => {
      render(
        <Quantity quantity={33} onQuantityChanged={null} quantityRef={null} />
      );
      expect(screen.getByRole('textbox')).toHaveAttribute('required');
    });
  });
});
