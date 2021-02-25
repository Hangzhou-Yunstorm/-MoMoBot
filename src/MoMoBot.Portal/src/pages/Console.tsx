import * as React from 'react';
import { Row, Col } from 'antd';
import { AsyncLoadBizCharts } from '../components/Charts/AsyncLoadBizCharts';
import { connect } from 'dva';
import IFeedbackState from '../@types/IFeedbackState';
import settings from '../settings';
import { Link } from 'react-router-dom';

const Suspense = React.Suspense;
const FeedbackPie = React.lazy(() => import('../components/Dashboard/FeedbackPie'));
const LuisStatistic = React.lazy(() => import('..//components/Dashboard/LuisStatistic'));
const PopularIntentsBar = React.lazy(() => import('../components/Dashboard/IntentsBar'));
const PraiseFeedbackBar = React.lazy(() => import('../components/Dashboard/IntentsBar'));
const BadFeedbackBar = React.lazy(() => import('../components/Dashboard/IntentsBar'));

const topColResponsiveProps = {
    style: { marginBottom: 24 },
};

@connect(({ feedback, loading }: { feedback: IFeedbackState, loading: any }) => ({
    feedback,
    feedbackLoading: loading.effects['feedback/fetchFeedbackStatistical'],
    popularLoading: loading.effects['feedback/fetchPopularIntents'],
    praiseLoading: loading.effects['feedback/fetchPraiseFeedback'],
    badLoading: loading.effects['feedback/fetchBadFeedback'],
}))
class Console extends React.Component<any> {
    reqRef = 0;
    componentDidMount() {
        const { dispatch } = this.props;
        this.reqRef = requestAnimationFrame(() => {
            dispatch({
                type: 'feedback/fetchFeedbackStatistical',
            });
            dispatch({
                type: 'feedback/fetchPopularIntents',
            });
            dispatch({
                type: 'feedback/fetchPraiseFeedback',
            });
            dispatch({
                type: 'feedback/fetchBadFeedback',
            });
        });
    }

    componentWillUnmount() {
        const { dispatch } = this.props;
        dispatch({
            type: 'feedback/clear',
        });
        cancelAnimationFrame(this.reqRef);
    }

    render() {
        const { feedback, feedbackLoading, popularLoading, praiseLoading, badLoading } = this.props;
        const { feedbackStatistical, popularIntents, praiseFeedback, badFeedback } = feedback;
        return (
            <div style={{ padding: '15px' }}>
                <Row gutter={24}>
                    <Col xs={24} md={16} {...topColResponsiveProps}>
                        <Suspense fallback={null}>
                            <FeedbackPie loading={feedbackLoading} data={feedbackStatistical} />
                        </Suspense>
                    </Col>
                    <Col xs={24} md={8} {...topColResponsiveProps}>
                        <Suspense fallback={null}>
                            {/* <FeedbackTable /> */}
                            <LuisStatistic />
                        </Suspense>
                    </Col>
                </Row>
                <Row style={{ paddingTop: '30px' }}>
                    <Col md={24}>
                        <Suspense fallback={null}>
                            <PopularIntentsBar title="热门意图（TOP 10）" loading={popularLoading} data={popularIntents} color="rgb(0, 188, 197)" />
                        </Suspense>
                    </Col>
                </Row>
                <Row style={{ paddingTop: '30px' }}>
                    <Col xs={24} md={12}>
                        <Suspense fallback={null}>
                            <PraiseFeedbackBar title="最好评反馈（TOP 10）" loading={praiseLoading} data={praiseFeedback} color={settings.primaryColor} />
                        </Suspense>
                    </Col>
                    <Col xs={24} md={12}>
                        <Suspense fallback={null}>
                            <BadFeedbackBar title="最糟糕反馈（TOP 10）" loading={badLoading} data={badFeedback} color={settings.dangerColor} />
                        </Suspense>
                    </Col>
                </Row>
                <div style={{ padding: '20px 0px' }}>
                    <Link to='/luis/dashboard' style={{color:'rgb(144, 144, 144)'}}>转到 LUIS 分析页</Link>
                </div>
            </div>

        );
    }
}

//export default Home;

export default (props: any) => (
    <AsyncLoadBizCharts>
        <Console {...props} />
    </AsyncLoadBizCharts>
);
