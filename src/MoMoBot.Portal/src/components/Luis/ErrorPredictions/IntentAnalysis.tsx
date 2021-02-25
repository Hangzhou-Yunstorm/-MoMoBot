import * as React from 'react';
import { Link } from 'react-router-dom';
import { Pie } from '../../../components/Charts';
import settings from '../../../settings';
import * as classNames from 'classnames';
import { Icon } from 'antd';
import QueueAnim from 'rc-queue-anim';

const styles = require('./index.less');

const IntentProblem = (props: any) => {
    const { modelId, modelName } = props;
    return (
        <div className={styles.problem}>
            <h3>数据不平衡</h3>
            <p>{modelName}的例句比其他意图少，<Link to={`/luis/intents/${modelId}`}>添加更多话语</Link>。</p>
        </div>
    )
}

const Accuracy = (props: any) => {
    const { data, correctly = 0.00, total = 0 } = props;

    return (
        <div className={styles.accuracy}>
            <h3>准确性</h3>
            <p>短语实例：{total} 条</p>
            <Pie height={150}
                hasLegend
                colors={settings.luisColors}
                data={data}
                total={() => <span>{Number.isNaN(correctly) ? 0.00 : correctly.toFixed(2)}%</span>} />
        </div>
    )
}

const AccuracyDetail = (props: any) => {
    const { modelId, detail: { incorrectly = [], unclear = [] } } = props;

    return (
        <div className={styles.detail}>
            <QueueAnim delay={0}>
                {
                    incorrectly.length > 0 &&
                    <div className={classNames(styles.item, styles.incorrectly)} key="incorrectly">
                        <h3><Icon type="minus-circle" theme="filled" style={{ fontSize: "10px", marginRight: '5px', color: '#a80000' }} />错误预测</h3>
                        <p>这个意图中的许多短语被错误预测。</p>
                        <p>考虑 <Link to={`/luis/intents/{id}`}>为事假添加更多短语</Link>。或使这些短语更加明显：</p>
                        <ul>
                            {
                                incorrectly.map((i: any, index: number) =>
                                    <li key={i.id}><Link to={`/luis/intents/${modelId}`}>预测为 {i.name} （{i.count}）</Link></li>
                                )
                            }
                        </ul>
                    </div>

                }
                {
                    unclear.length > 0 &&
                    <div className={classNames(styles.item, styles.unclear)} key="unclear">
                        <h3><Icon type="warning" theme="filled" style={{ fontSize: "10px", marginRight: '5px', color: '#f2610c' }} />不明确预测</h3>
                        <p>这个意图中的许多短语被错误预测。</p>
                        <p>考虑让这些短语更加清晰：</p>
                        <ul>
                            {
                                unclear.map((i: any, index: number) =>
                                    <li key={i.id}><Link to={`/luis/intents/${modelId}`}>几乎预测为 {i.name} （{i.count}）</Link></li>
                                )
                            }

                        </ul>
                    </div>
                }
            </QueueAnim>
        </div>
    )
}

interface IIntentAnalysisProps {
    metadataModel: {
        modelId: string;
        modelName: string;
        utterancesCount: number;
        misclassifiedUtterancesCount: number;
        ambiguousUtterancesCount: number;
        misclassifiedAmbiguousUtterancesCount: number;
    };
    detail?: {}
}

export default class IntentAnalysis extends React.Component<IIntentAnalysisProps> {
    getAccuracyPieData() {
        const { metadataModel } = this.props;
        return [{
            x: '错误预测',
            y: metadataModel.misclassifiedUtterancesCount
        }, {
            x: '不明确预测',
            y: metadataModel.ambiguousUtterancesCount - metadataModel.misclassifiedAmbiguousUtterancesCount
        }, {
            x: '正确预测',
            y: metadataModel.utterancesCount - metadataModel.ambiguousUtterancesCount
        }]
    }

    render() {
        const { metadataModel, detail } = this.props;
        const accuracyData = this.getAccuracyPieData();
        const correctly = metadataModel.utterancesCount <= 0 ? 0.00 :
            (metadataModel.utterancesCount - metadataModel.ambiguousUtterancesCount) / metadataModel.utterancesCount * 100;

        return (
            <div className={styles.analysis}>
                <div className={styles.header}>
                    <h2 className={styles.intent}>
                        {metadataModel.modelName}
                        <span>{Number.isNaN(correctly) ? 0.00 : correctly.toFixed(2)}％正确预测</span>
                    </h2>
                </div>
                {
                    metadataModel.utterancesCount < 6 &&
                    <IntentProblem {...metadataModel} />
                }
                <Accuracy data={accuracyData}
                    correctly={correctly}
                    total={Number.isNaN(metadataModel.utterancesCount) ? 0 : metadataModel.utterancesCount} />
                <AccuracyDetail detail={detail}
                    modelId={metadataModel.modelId} />
            </div>
        )
    }
}