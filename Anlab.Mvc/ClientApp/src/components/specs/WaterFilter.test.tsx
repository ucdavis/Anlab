import * as React from 'react';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';

import { WaterFilter } from '../WaterFilter';

describe('<Grind />', () => {
  it('should render null when not water', () => {
    render(
      <WaterFilter sampleType='Dirt' handleChange={null} filterWater={false} />
    );

    expect(screen.queryByRole('checkbox')).not.toBeInTheDocument();
  });
  it('should render a Checkbox when sampleType is water', () => {
    render(
      <WaterFilter sampleType='Water' handleChange={null} filterWater={false} />
    );

    expect(screen.queryByRole('checkbox')).toBeInTheDocument();
  });
  it('should have a checked false', () => {
    render(
      <WaterFilter sampleType='Water' handleChange={null} filterWater={false} />
    );

    expect(screen.queryByRole('checkbox')).not.toBeChecked();
  });
  it('should have a checked true', () => {
    render(
      <WaterFilter sampleType='Water' handleChange={null} filterWater={true} />
    );

    expect(screen.queryByRole('checkbox')).toBeChecked();
  });

  it('should call handleChange with on change event', async () => {
    const handleChange = jest.fn() as (value: string) => void;
    
    const target = render(
      <WaterFilter
        sampleType='Water'
        handleChange={handleChange}
        filterWater={false}
      />
    );

    const user = userEvent.setup();

    await user.click(target.getByRole('checkbox'));

    expect(handleChange).toHaveBeenCalled();
  });
});
