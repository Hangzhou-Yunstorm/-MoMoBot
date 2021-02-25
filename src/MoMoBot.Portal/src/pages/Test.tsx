import * as React from 'react';
import StackedBar from '../components/Charts/StackedBar/index';
import { AsyncLoadBizCharts } from '../components/Charts/AsyncLoadBizCharts';

class Test extends React.Component {
    render() {
        const data = [
            {
                name: "London",
                "Jan.": 18.9,
                "Feb.": 28.8,
                "Mar.": 39.3,
                "Apr.": 81.4,
                May: 47,
                "Jun.": 20.3,
                "Jul.": 24,
                "Aug.": 35.6
            },
            {
                name: "Berlin",
                "Jan.": 12.4,
                "Feb.": 23.2,
                "Mar.": 34.5,
                "Apr.": 99.7,
                May: 52.6,
                "Jun.": 35.5,
                "Jul.": 37.4,
                "Aug.": 42.4
            }
        ];
        return (
            <div>
                <StackedBar data={data} />
            </div>
        )
    }
}

export default (props: any) =>
    <AsyncLoadBizCharts>
        <Test {...props} />
    </AsyncLoadBizCharts>