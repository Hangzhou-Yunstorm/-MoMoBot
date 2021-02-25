import * as React from 'react';
import { Row, Col, Icon } from 'antd';
import { Pie } from '../Charts';
import settings from '../../settings';
// import { AsyncLoadBizCharts } from '../Charts/AsyncLoadBizCharts';
const styles = require('./index.less');

const OverallPie = (props: any) => {
    const { data = [] } = props;
    const sum = data.reduce((total: number, item: any) => (total + Number.parseInt(item.y)), 0);
    return (
        <Pie
            hasLegend
            data={data}
            height={350}
            colors={settings.luisColors}
            total={() => <div>
                <h2>{Number.isNaN(sum) ? 0 : sum}</h2>
                <p>短语实例</p>
            </div>}
        />
    )
}

const Suggestions = (props: any) => {
    const { data = {} } = props;
    return (
        <div className={styles.suggestions}>
            <h2 className={styles.hreader}>问题和建议</h2>
            {data.imbalance.length > 0 && <div className={styles.block}>
                <h3 className={styles.title}>数据不平衡</h3>
                <p>这些意图比你的应用程序中的其他意图的短语更少，这可以权衡预测远离这个意图。考虑为这些意图添加更多短语实例。</p>
                <ul className={styles.list}>
                    {data.imbalance.map((item: any, index: number) => (<li key={index}><a href={`/luis/intents/${item.modelId}`}>{item.modelName}</a></li>))}
                </ul>
            </div>}
            {data.incorrect.length > 0 && <div className={styles.block}>
                <h3 className={styles.title}>错误预测<Icon type="minus-circle" theme="filled" style={{ fontSize: "10px", marginLeft: '5px', color: '#a80000' }} /></h3>
                <p>这些意图的错误预测百分比最高。考虑修改这些意图中错误预测的话语。</p>
                <ul className={styles.list}>
                    {data.incorrect.map((item: any, index: number) => (<li key={index}><a href={`/luis/intents/${item.modelId}`}>{item.modelName}</a></li>))}
                </ul>
            </div>}
            {data.unclear.length > 0 && <div className={styles.block}>
                <h3 className={styles.title}>不明确预测<Icon type="warning" theme="filled" style={{ fontSize: "10px", marginLeft: '5px', color: '#f2610c' }} /></h3>
                <p>这些意图的预测不明确率最高。考虑修改这些意图中不清楚的话语</p>
                <ul className={styles.list}>
                    {data.unclear.map((item: any, index: number) => (<li key={index}><a href={`/luis/intents/${item.modelId}`}>{item.modelName}</a></li>))}
                </ul>
            </div>}
        </div>
    )
}

export interface IOverallProps {
    problemData?: { imbalance: Array<any>; incorrect: Array<any>; unclear: Array<any> },
    pieData: Array<{ x: string; y: number }>;
}

export default class Overall extends React.PureComponent<IOverallProps> {
    render() {
        const { problemData, pieData } = this.props;
        return (
            <Row gutter={24}>
                <Col sm={24} md={12}>
                    <OverallPie data={pieData} />
                </Col>
                <Col sm={24} md={12}>
                    <Suggestions data={problemData} />
                </Col>
            </Row>
        )
    }
}

// export default (props: IOverallProps) =>
//     <AsyncLoadBizCharts>
//         <Overall {...props} />
//     </AsyncLoadBizCharts>;