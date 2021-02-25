import * as React from 'react';
import { Skeleton, Card } from 'antd';
import { Line } from '../Charts';
import { Radio } from 'antd';
const styles = require('./EndpointHitHistory.less');

const paragraphWidth = ['100%', '100%', '100%', '100%', '100%']

export interface IEndpointHitHistoryProps {
    data: Array<{ x: string, y: number }>;
    loading?: boolean;
    days: number;
    onPreDaysChange?: (days: number) => void;
}

export default class EndpointHitHistory extends React.PureComponent<IEndpointHitHistoryProps> {
    render() {
        const { loading, data, days, onPreDaysChange } = this.props;
        const title = <span>最近<b> {days} </b>日意图命中统计（次）</span>;

        return (
            <div className={styles.container}>

                <Skeleton loading={loading}
                    active={true}
                    title={false}
                    paragraph={{ rows: paragraphWidth.length, width: paragraphWidth }}>
                    <Radio.Group value={days}
                        className={styles.buttongroup}
                        onChange={(e) => { onPreDaysChange && onPreDaysChange(e.target.value) }}>
                        <Radio.Button value={7}>一周</Radio.Button>
                        <Radio.Button value={15}>半月</Radio.Button>
                    </Radio.Group>
                    <Line data={data}
                        height={380}
                        title={title}
                    // color="l (270) 0:rgba(255, 146, 255, 1) .5:rgba(100, 268, 255, 1) 1:rgba(215, 0, 255, 1)"
                    />
                </Skeleton>
            </div>
        )
    }
}