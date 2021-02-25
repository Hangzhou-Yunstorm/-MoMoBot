import * as React from 'react';
import { connect } from 'dva';
import { Row, Col, Collapse, Popover, Icon, Spin } from 'antd';
import { AsyncLoadBizCharts } from '../../components/Charts/AsyncLoadBizCharts';
const styles = require('./Dashboard.less');

const LuisStatus = React.lazy(() => import('../../components/Luis/LuisStatus'));
const EndpointHitHistory = React.lazy(() => import('../../components/Luis/EndpointHitHistory'));
const Overall = React.lazy(() => import('../../components/Luis/Overall'));
const PerIntentBar = React.lazy(() => import('../../components/Luis/PerIntent'));
const ErrorPredictions = React.lazy(() => import('../../components/Luis/ErrorPredictions/index'));
const Suspense = React.Suspense;
const Panel = Collapse.Panel;

@connect(({ dashboard, loading }: { dashboard: any, loading: any }) => ({
    historyLoadin: loading.effects['dashboard/fetchEndpointHitHistory'],
    endpointhitshistories: dashboard.endpointhitshistories,
    metadata: dashboard.metadata,
    statusInfo: dashboard.statusInfo,
    errorIntents: dashboard.errorIntents,
    activeIntent: dashboard.activeIntent,
    errorDetail: dashboard.errorDetail
}))
class Dashboard extends React.PureComponent<any, any> {
    constructor(props: any) {
        super(props);
        this.state = {
            preDays: 7
        }
    }

    componentDidMount() {
        const { preDays } = this.state;
        this.fetchHitHistories(preDays);
        this.fetchLuisAppStatusInfo();
        this.fetchStatsMetadata();
    }
    fetchLuisAppStatusInfo() {
        this.props.dispatch({
            type: 'dashboard/fetchLuisAppStatus'
        })
    }

    fetchStatsMetadata() {
        this.props.dispatch({
            type: 'dashboard/fetchStatsMetadata'
        })
    }

    fetchHitHistories(days: number) {
        const { dispatch } = this.props;

        dispatch({
            type: 'dashboard/fetchEndpointHitHistory',
            payload: days
        })
    }

    handlePreDaysChange(days: number) {
        console.log(days);
        this.setState({ preDays: days });
        this.fetchHitHistories(days);
    }

    renderTip = (title: React.ReactNode, content: string) => (
        <React.Fragment>{title}
            <Popover placement="right" content={content} trigger="hover">
                <Icon className={styles.tip} type="question-circle" />
            </Popover>
        </React.Fragment>
    )

    /**获取所有意图预测数据 */
    getPerIntentBarData() {
        const { metadata = {} } = this.props;
        const { modelsMetadata = [] } = metadata;

        let data: Array<any> = [];

        modelsMetadata
            .filter((model: any) => model.modelName !== 'None')
            .sort((a: any, b: any) => {
                var value1 = b.ambiguousUtterancesCount / b.utterancesCount,
                    value2 = a.ambiguousUtterancesCount / a.utterancesCount;
                return value1 - value2;
            })
            .forEach((model: any) => {
                data.push(
                    {
                        x: model.modelName,
                        y: '错误预测',
                        value: model.misclassifiedUtterancesCount,
                    }, {
                        x: model.modelName,
                        y: '不明确预测',
                        value: model.ambiguousUtterancesCount - model.misclassifiedAmbiguousUtterancesCount
                    }, {
                        x: model.modelName,
                        y: '正确预测',
                        value: model.utterancesCount - model.ambiguousUtterancesCount
                        // value: model.misclassifiedAmbiguousUtterancesCount > 0 ?
                        //     model.utterancesCount - model.misclassifiedAmbiguousUtterancesCount :
                        //     model.utterancesCount - model.misclassifiedUtterancesCount - model.ambiguousUtterancesCount

                    }
                );
            });
        return data;
    }

    /**获取问题和建议数据 */
    getProblemsAndSuggestionsData() {
        const { metadata = {} } = this.props;
        const { modelsMetadata = [] } = metadata;
        let data = {
            imbalance: [],
            incorrect: [],
            unclear: []
        };
        data.imbalance = modelsMetadata
            .sort((a: any, b: any) => a.utterancesCount - b.utterancesCount)
            .slice(0, 3);
        data.incorrect = modelsMetadata.filter((item: any) => item.utterancesCount > 0 && item.misclassifiedUtterancesCount > 0)
            .sort((a: any, b: any) => b.misclassifiedUtterancesCount / b.utterancesCount - a.misclassifiedUtterancesCount / a.utterancesCount)
            .slice(0, 3);
        data.unclear = modelsMetadata.filter((item: any) => item.utterancesCount > 0 && item.ambiguousUtterancesCount > 0)
            .sort((a: any, b: any) => b.ambiguousUtterancesCount / b.utterancesCount - a.ambiguousUtterancesCount / a.utterancesCount)
            .slice(0, 3);

        return data;
    }

    getOverallPieData() {
        const { metadata: { appVersionUtterancesCount: totle, misclassifiedAmbiguousUtterancesCount = 0, ambiguousUtterancesCount = 0, misclassifiedUtterancesCount = 0 } } = this.props;

        return [{
            x: '错误预测',
            y: misclassifiedUtterancesCount
        }, {
            x: '不明确预测',
            y: ambiguousUtterancesCount - misclassifiedAmbiguousUtterancesCount
        }, {
            x: '正确预测',
            y: totle - ambiguousUtterancesCount
        }]
    }

    onIntentClick(modelId: string) {
        this.props.dispatch({
            type: 'dashboard/changeActiveIntent',
            payload: modelId
        })
    }

    render() {
        const { endpointhitshistories, historyLoadin, statusInfo, errorIntents, activeIntent, errorDetail } = this.props;
        const { preDays } = this.state;

        const perIntentBarData = this.getPerIntentBarData();
        const problemData = this.getProblemsAndSuggestionsData();
        const pieData = this.getOverallPieData();

        console.log(statusInfo);

        const fallback = <Spin />;

        return (
            <div className={styles.container}>
                <h1 className={styles.title}>
                    {this.renderTip('发布应用', "选择已发布的插槽以查看应用程序的当前状态，包括服务，区域和端点命中。")}
                </h1>
                <Row gutter={16} style={{ marginBottom: "15px" }}>
                    <Col md={6} sm={24} className={styles.card}>
                        <Suspense fallback={null}>
                            <LuisStatus info={statusInfo} />
                        </Suspense>
                    </Col>
                    <Col md={18} sm={24} className={styles.card}>
                        <Suspense fallback={null}>
                            <EndpointHitHistory days={preDays}
                                data={endpointhitshistories}
                                onPreDaysChange={this.handlePreDaysChange.bind(this)}
                                loading={historyLoadin} />
                        </Suspense>
                    </Col>
                </Row>
                <h1 className={styles.title}>
                    {this.renderTip('训练评估', "您上次培训的预测质量摘要。 专注于不明确和错误预测的意图，以改善应用程序预测。")}
                </h1>
                <Collapse bordered={false}
                    defaultActiveKey={['1']}
                    expandIconPosition="right">
                    <Panel header={this.renderTip(<span className={styles.secTitle}>总体预测</span>, "正确百分比通过预测意图与标记意图匹配的话语百分比来度量。")}
                        key="1"
                        style={{ marginBottom: '30px' }} >
                        <div className={styles.panel}>
                            <Suspense fallback={fallback}>
                                <Overall problemData={problemData}
                                    pieData={pieData} />
                            </Suspense>
                        </div>
                    </Panel>
                    <Panel header={this.renderTip(<span className={styles.secTitle}>所有意图的预测</span>, "意图总体质量的直观总结。 有效的应用程序具有高度正确预测的示例话语，无论是在意图内还是在意图内。")}
                        key="2"
                        style={{ marginBottom: '30px' }}>
                        <div className={styles.panel}>
                            <Suspense fallback={fallback}>
                                <PerIntentBar data={perIntentBarData} color={['y', ['#a80000', '#f2610c', '#0078d7']]} />
                            </Suspense>
                        </div>
                    </Panel>
                    <Panel header={this.renderTip(<span className={styles.secTitle}>有预测错误的意图</span>, "选择过滤器和意图以查看有关改进意图的建议的问题。")}
                        key="3" >
                        <div className={styles.panel}>
                            <Suspense fallback={fallback}>
                                <ErrorPredictions intents={errorIntents}
                                    activeIntent={activeIntent}
                                    errorDetail={errorDetail}
                                    onItemClick={this.onIntentClick.bind(this)} />
                            </Suspense>
                        </div>
                    </Panel>
                </Collapse>
            </div >
        )
    }
}

export default (props: any) =>
    <AsyncLoadBizCharts>
        <Dashboard {...props} />
    </AsyncLoadBizCharts>
