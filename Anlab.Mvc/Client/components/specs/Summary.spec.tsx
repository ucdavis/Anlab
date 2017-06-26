import * as React from 'react';
import { shallow, mount, render } from 'enzyme';
import { ITestItem } from '../TestList';
import { Summary } from '../Summary';

describe('<Summary />', () => {
    describe('Internal functions', () => {
        const testItems: Array<ITestItem> = [
            { id: 1, analysis: '1ABC', code: '1C-ABC', internalCost: 2.02, externalCost: 3.03, setupCost: 5, category: 'Cat1' },
            { id: 2, analysis: '2ABC', code: '2C-ABC', internalCost: 1.01, externalCost: 4.03, setupCost: 6, category: 'Cat2' }
        ];

        describe('grindCost internal function', () => {
            it('should return zero when not a grind parameter', () => {
                const payment = { clientType: 'other', account: '' };
                const target = shallow(<Summary adjustmentAmount={0}
                                                isFromLab={false}
                                                quantity={1}
                                                payment={payment}
                                                foreignSoil={false}
                                                status="Test"
                                                canSubmit={false}
                                                filterWater={false}
                                                grind={false}
                                                hideError={true}
                                                isCreate={true}
                                                onSubmit={null}
                                                testItems={testItems} />);

                const internal = target.instance();
                expect(internal.grindCost()).toEqual(0);
            });
            it('should return 6 when a grind parameter and uc payment (Note: I expect this to change))', () => {
                const payment = { clientType: 'uc', account: '' };
                const target = shallow(<Summary adjustmentAmount={0}
                                                isFromLab={false}
                                                quantity={1}
                                                payment={payment}
                                                foreignSoil={false}
                                                status="Test"
                                                canSubmit={false}
                                                filterWater={false}
                                                grind={true}
                                                hideError={true}
                                                isCreate={true}
                                                onSubmit={null}
                                                testItems={testItems} />);

                const internal = target.instance();
                expect(internal.grindCost()).toEqual(6);
            });
            it('should return 9 when a grind parameter and other payment (Note: I expect this to change))', () => {
                const payment = { clientType: 'other', account: '' };
                const target = shallow(<Summary adjustmentAmount={0}
                                                isFromLab={false}
                                                quantity={1}
                                                payment={payment}
                                                foreignSoil={false}
                                                status="Test"
                                                canSubmit={false}
                                                filterWater={false}
                                                grind={true}
                                                hideError={true}
                                                isCreate={true}
                                                onSubmit={null}
                                                testItems={testItems} />);

                const internal = target.instance();
                expect(internal.grindCost()).toEqual(9);
            });
        });
        describe('foreignSoilCost internal function', () => {
            it('should return zero when not a foreignSoil parameter', () => {
                const payment = { clientType: 'other', account: '' };
                const target = shallow(<Summary adjustmentAmount={0}
                                                isFromLab={false}
                                                quantity={1}
                                                payment={payment}
                                                foreignSoil={false}
                                                status="Test"
                                                canSubmit={false}
                                                filterWater={false}
                                                grind={false}
                                                hideError={true}
                                                isCreate={true}
                                                onSubmit={null}
                                                testItems={testItems} />);

                const internal = target.instance();
                expect(internal.foreignSoilCost()).toEqual(0);
            });
            it('should return 9 when a foreignSoil parameter and uc payment (Note: I expect this to change))', () => {
                const payment = { clientType: 'uc', account: '' };
                const target = shallow(<Summary adjustmentAmount={0}
                                                isFromLab={false}
                                                quantity={1}
                                                payment={payment}
                                                foreignSoil={true}
                                                status="Test"
                                                canSubmit={false}
                                                filterWater={false}
                                                grind={true}
                                                hideError={true}
                                                isCreate={true}
                                                onSubmit={null}
                                                testItems={testItems} />);

                const internal = target.instance();
                expect(internal.foreignSoilCost()).toEqual(9);
            });
            it('should return 14 when a foreignSoil parameter and other payment (Note: I expect this to change))', () => {
                const payment = { clientType: 'other', account: '' };
                const target = shallow(<Summary adjustmentAmount={0}
                                                isFromLab={false}
                                                quantity={1}
                                                payment={payment}
                                                foreignSoil={true}
                                                status="Test"
                                                canSubmit={false}
                                                filterWater={false}
                                                grind={true}
                                                hideError={true}
                                                isCreate={true}
                                                onSubmit={null}
                                                testItems={testItems} />);

                const internal = target.instance();
                expect(internal.foreignSoilCost()).toEqual(14);
            });
        });

        it('should add stuff', () => {
            
            

            const payment = { clientType: 'uc', account: '' };
            const target = shallow(<Summary adjustmentAmount={0}
                isFromLab={false}
                quantity={1}
                payment={payment}
                foreignSoil={false}
                status="Test"
                canSubmit={false}
                filterWater={false}
                grind={false}
                hideError={true}
                isCreate={true}
                onSubmit={null}
                testItems={testItems} />);

            const internal = target.instance();
            expect(internal.totalCost()).toEqual(14.03);
        });
    });
});
