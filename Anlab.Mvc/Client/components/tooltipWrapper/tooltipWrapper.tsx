import * as React from 'react';
import Tooltip from 'react-toolbox/lib/tooltip';


const Other = (props) => (
    <div {...props}>{props.children}</div>
);

export default Tooltip((Other) as any);



