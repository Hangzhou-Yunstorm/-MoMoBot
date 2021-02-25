import * as React from 'react';
import IChartBaseProps from './IChartBaseProps';
import ChartSkeleton from './ChartSkeleton';
import { Bar } from '../Charts';

interface IIntentsBarProps extends IChartBaseProps {
    title: string;
    height?: number;
    color?: string
}

export default class IntentsBar extends React.PureComponent<IIntentsBarProps> {
    static defaultProps = {
        data: [],
        height: 380
    }

    render() {
        const { loading, data, title, height, color } = this.props;
        return (
            <ChartSkeleton loading={loading}>
                <Bar data={data!} height={height!} title={title} color={color} />
            </ChartSkeleton>
        )
    }
}