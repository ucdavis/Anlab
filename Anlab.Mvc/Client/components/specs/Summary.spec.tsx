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
        describe('waterFilterCost internal function', () => {
            it('should return zero when not a waterFilterCost parameter', () => {
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
                expect(internal.waterFilterCost()).toEqual(0);
            });
            it('should return 11 when a waterFilterCost parameter and uc payment (Note: I expect this to change))', () => {
                const payment = { clientType: 'uc', account: '' };
                const target = shallow(<Summary adjustmentAmount={0}
                                                isFromLab={false}
                                                quantity={1}
                                                payment={payment}
                                                foreignSoil={true}
                                                status="Test"
                                                canSubmit={false}
                                                filterWater={true}
                                                grind={true}
                                                hideError={true}
                                                isCreate={true}
                                                onSubmit={null}
                                                testItems={testItems} />);

                const internal = target.instance();
                expect(internal.waterFilterCost()).toEqual(11);
            });
            it('should return 17 when a waterFilterCost parameter and other payment (Note: I expect this to change))', () => {
                const payment = { clientType: 'other', account: '' };
                const target = shallow(<Summary adjustmentAmount={0}
                                                isFromLab={false}
                                                quantity={1}
                                                payment={payment}
                                                foreignSoil={true}
                                                status="Test"
                                                canSubmit={false}
                                                filterWater={true}
                                                grind={true}
                                                hideError={true}
                                                isCreate={true}
                                                onSubmit={null}
                                                testItems={testItems} />);

                const internal = target.instance();
                expect(internal.waterFilterCost()).toEqual(17);
            });
        });
        describe('totalCost internal function', () => {
            it('should add up the cost with internal, quantity 1, no grind/filter/foreign', () => {
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
            it('should add up the cost with internal, quantity 3, no grind/filter/foreign', () => {
                const payment = { clientType: 'uc', account: '' };
                const target = shallow(<Summary adjustmentAmount={0}
                                                isFromLab={false}
                                                quantity={3}
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
                expect(internal.totalCost()).toEqual(20.09); //(3 * 3.03) + 11
            });
            it('should add up the cost with external, quantity 1, no grind/filter/foreign', () => {
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
                expect(internal.totalCost()).toEqual(18.06); //(7.06 + 11 )
            });
            it('should add up the cost with external, quantity 3, no grind/filter/foreign', () => {
                const payment = { clientType: 'other', account: '' };
                const target = shallow(<Summary adjustmentAmount={0}
                                                isFromLab={false}
                                                quantity={3}
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
                expect(internal.totalCost()).toEqual(32.18); //(3 * 7.06) + 11
            });
            it('should add up the cost with internal, quantity 1, grind', () => {
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
                expect(internal.totalCost()).toEqual(20.03); //(3.03 + 6) + 11
            });
            it('should add up the cost with internal, quantity 3, grind', () => {
                const payment = { clientType: 'uc', account: '' };
                const target = shallow(<Summary adjustmentAmount={0}
                                                isFromLab={false}
                                                quantity={3}
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
                expect(internal.totalCost()).toEqual(38.09); // 3 * (3.03 + 6) + 11
            });
            it('should add up the cost with external, quantity 1, grind', () => {
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
                expect(internal.totalCost()).toEqual(27.06);
            });
            it('should add up the cost with external, quantity 3, grind', () => {
                const payment = { clientType: 'other', account: '' };
                const target = shallow(<Summary adjustmentAmount={0}
                                                isFromLab={false}
                                                quantity={3}
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
                expect(internal.totalCost()).toEqual(59.18); 
            });
            it('should add up the cost with internal, quantity 3, grind, foriegn, filter', () => {
                const payment = { clientType: 'uc', account: '' };
                const target = shallow(<Summary adjustmentAmount={0}
                                                isFromLab={false}
                                                quantity={3}
                                                payment={payment}
                                                foreignSoil={true}
                                                status="Test"
                                                canSubmit={false}
                                                filterWater={true}
                                                grind={true}
                                                hideError={true}
                                                isCreate={true}
                                                onSubmit={null}
                                                testItems={testItems} />);
                const internal = target.instance();
                expect(internal.totalCost()).toEqual(98.09); // 3 * (3.03 + 6 + 9 + 11) + 11
            });
        });
    });
});
