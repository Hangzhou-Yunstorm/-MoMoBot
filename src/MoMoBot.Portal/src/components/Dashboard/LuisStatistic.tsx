import * as React from 'react';
import { Card, Statistic } from 'antd';

export default class LuisStatistic extends React.PureComponent {
    render() {
        return (
            <div>
                <Card>
                    <Statistic title="系统异常" value={25} />
                
                </Card>
            </div>
        )
    }
}