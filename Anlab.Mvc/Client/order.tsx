import './css/site.scss';

import * as React from 'react';
import * as ReactDOM from 'react-dom';

class Hi extends React.Component<any, any> {
    render() {
        return <h1>Hello world</h1>;
    }
}

ReactDOM.render(<Hi />,document.getElementById('react-app')
);
