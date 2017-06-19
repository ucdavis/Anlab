import * as React from 'react';
import { shallow, mount, render } from 'enzyme';
import { Project } from '../Project';

describe('<Project />', () => {
    it('should render an input', () => {
        const target = mount(<Project />);
        expect(target.find('input').length).toEqual(1);
    });
});