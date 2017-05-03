import * as React from 'react';

const users = [
    { name: 'Javi Jimenez', twitter: '@soyjavi', birthdate: new Date(1980, 3, 11), cats: 1 },
    { name: 'Javi Velasco', twitter: '@javivelasco', birthdate: new Date(1987, 1, 1), dogs: 1, active: true }
];

export default class TestList extends React.Component<any, any> {
    state = { selected: [], source: users };

    renderRows = () => {
        return users.map(u => {
            return (
                <tr key={u.name}>
                    <td>{u.name}</td>
                    <td>r2</td>
                    <td>r3</td>
                </tr>
            );
        });
    }
    render() {
        return (
            <table className="table">
                <thead>
                    <tr>
                        <th>Column1</th>
                        <th>Col2</th>
                        <th>Col3</th>
                    </tr>
                </thead>
                <tbody>
                    {this.renderRows()}
                </tbody>
            </table>
        );
    }
}