import * as React from 'react';
import { render, screen } from '@testing-library/react';

import { Project } from '../Project';

describe('<Project />', () => {
    it('should render without crashing', () => {
        render(<Project project="1" handleChange={null} projectRef={null}/>);
    });
    it('should render an input', () => {
        render(<Project project="1" handleChange={null} projectRef={null}/>);
        
        // expect(screen.getByText('Learn React')).toBeInTheDocument();
        // expect(screen.getByLabelText('Project Title / Location')).toBeInTheDocument();
        expect(screen.getByRole('textbox')).toBeInTheDocument();

    });
});
