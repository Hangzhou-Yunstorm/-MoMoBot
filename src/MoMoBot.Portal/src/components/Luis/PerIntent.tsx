import * as React from 'react';
import { StackedPercentageBar } from '../Charts';
import settings from '../../settings';
// import { AsyncLoadBizCharts } from '../Charts/AsyncLoadBizCharts';

export interface IPerIntentProps {
    data?: Array<{ x: string; y: string; value: number }>;
    color?: string | [string, string] | [string, string[]]
}

export default class PerIntent extends React.Component<IPerIntentProps> {
    static defaultProps: IPerIntentProps = {
        color: ['y', settings.luisColors]
    }

    render() {
        const { data, color } = this.props;

        return (
            <div>
                <StackedPercentageBar color={color} data={data} height={400} visibleX={false} />
            </div>
        )
    }
}

// export default (props: IPerIntentProps) =>
//     <AsyncLoadBizCharts>
//         <PerIntent {...props} />
//     </AsyncLoadBizCharts>