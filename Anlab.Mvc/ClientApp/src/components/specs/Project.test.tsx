import * as React from 'react';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';

import { Project } from '../Project';

describe('<Project />', () => {
  it('should render without crashing', () => {
    render(<Project project='1' handleChange={null} projectRef={null} />);
  });

  it('should render an input', () => {
    render(<Project project='1' handleChange={null} projectRef={null} />);

    expect(screen.getByRole('textbox')).toBeInTheDocument();
  });

  it('should clear error on good value', async () => {
    render(<Project project='1' handleChange={() => {}} projectRef={null} />);

    const user = userEvent.setup();

    // submit a blank value, which should cause an error
    await user.clear(screen.getByRole('textbox'));
    await user.tab();

    expect(
      screen.getByText('The project Title is required')
    ).toBeInTheDocument();

    // put in a good value, which should clear the error
    await user.type(screen.getByRole('textbox'), 'good value');
    await user.tab();

    expect(
      screen.queryByText('The project Title is required')
    ).not.toBeInTheDocument();
  });
});
