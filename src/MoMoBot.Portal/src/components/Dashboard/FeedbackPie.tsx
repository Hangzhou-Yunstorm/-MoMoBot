import * as React from 'react';

import { Pie } from '../Charts';
import IChartBaseProps from './IChartBaseProps';
import ChartSkeleton from './ChartSkeleton';
import settings from '../../settings';

const memo = React.memo;

// const salesPieData: IPieData[] = [
//     {
//         x: '家用电器',
//         y: 4544,
//     },
//     {
//         x: '食用酒水',
//         y: 3321,
//     },
//     {
//         x: '个护健康',
//         y: 3113,
//     }
// ];


interface IFeedbackPicProps extends IChartBaseProps {
}

class FeedbackPic extends React.PureComponent<IFeedbackPicProps> {

    static defaultProps = {
        data: [],
        loading: true
    }

    render() {
        const { data, loading } = this.props;
        return (
            <ChartSkeleton loading={loading}>
                <Pie
                    hasLegend
                    title="总反馈数"
                    subTitle="总反馈数"
                    colors={settings.luisColors}
                    total={() => (
                        <span
                            dangerouslySetInnerHTML={{
                                __html: data!.reduce((pre, now) => now.y + pre, 0) + ''
                            }}
                        />
                    )}
                    data={data}
                    valueFormat={val => <span dangerouslySetInnerHTML={{ __html: val }} />}
                    height={250}
                />
            </ChartSkeleton>
        )
    }
}

export default memo(FeedbackPic);