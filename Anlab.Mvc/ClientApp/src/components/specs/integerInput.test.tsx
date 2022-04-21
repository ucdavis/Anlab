import * as React from 'react';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';

import { IntegerInput } from '../ui/integerInput/integerInput';

describe('<IntegerInput />', () => {
  it('should render an input', () => {
    render(<IntegerInput />);

    expect(screen.getByRole('textbox')).toBeInTheDocument();
  });

  it('should clear error on good value', async () => {
    render(<IntegerInput min={10} max={20} onChange={() => {}} />);

    const user = userEvent.setup();

    // submit a valid integer value
    await user.type(screen.getByRole('textbox'), '15');

    // should not have an error
    expect(screen.queryByText('Must be a number')).not.toBeInTheDocument();
  });

  it('should set error on non-number value', async () => {
    render(<IntegerInput min={10} max={20} onChange={() => {}} />);

    const user = userEvent.setup();

    // submit a valid integer value
    await user.type(screen.getByRole('textbox'), 'abc');
    await user.tab();

    // should have an error
    expect(screen.getByText('Must be a number.')).toBeInTheDocument();
  });

  it('should set error on less than min value', async () => {
    render(<IntegerInput min={10} onChange={() => {}} />);

    const user = userEvent.setup();

    // submit a valid integer value
    await user.type(screen.getByRole('textbox'), '2');
    await user.tab();

    // should have an error
    expect(screen.getByText('Must be a number greater than 10.')).toBeInTheDocument();
  });
});
