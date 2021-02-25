import React from 'react';
import IntentList from './IntentList';
import Suggestions from './Suggestions';
import IntentAnalysis from './IntentAnalysis';
import { Row, Col } from 'antd';
const styles = require('./index.less');
import request from '../../../utils/request';
import settings from '../../../settings';

const allSuggestions = [{
    key: '1',
    content: (
        <div>
            <p>与其他意图相比，此意图包含更少的示例话语。</p>
            <ul>
                <li>为此意图添加更多话语实例。</li>
            </ul>
        </div>
    )
}, {
    key: '2',
    content: (
        <div>
            <p>此意图及其最近的意图使用相似的词语。</p>
            <ul>
                <li>使用这些词语为此意图添加更多话语。这将减少这些词对其他意图的重量。</li>
                <li>如果两个意图需要使用相似的单词，但顺序不同，请考虑添加一个模式。</li>
            </ul>
        </div>
    )
},
{
    key: '3',
    content: (
        <div>
            <p>这个意图和它最接近的意图有相似的词。</p>
            <ul>
                <li>考虑将此意图与其竞争对手合并。例如，“Order Sandwich”和“Order Pizza”可能会被合并为一个更广泛的意图，称为“订单午餐”。</li>
            </ul>
        </div>
    )
}, {
    key: '4',
    content: (
        <div>
            <p>这个意图及其最近的意图有相似的词。</p>
            <ul>
                <li>考虑将不明确的预测分成不同的意图。</li>
            </ul>
        </div>
    )
}]

export default class ErrorPredictions extends React.Component {
    state = {
        detail: {}
    }

    itemClick(id) {
        const { onItemClick } = this.props;
        onItemClick && onItemClick(id);
    }

    getSuggestions() {
        const { activeIntent = {}, errorDetail: { incorrectly = [], unclear = [] } } = this.props;
        let suggestions = [];
        if (activeIntent.utterancesCount < 6) {
            suggestions.push({
                key: '1',
                content: (
                    <div>
                        <p>与其他意图相比，此意图包含更少的示例话语。</p>
                        <ul>
                            <li>为此意图添加更多话语实例。</li>
                        </ul>
                    </div>
                )
            })
        }
        if (incorrectly.length > 0) {
            suggestions.push({
                key: '2',
                content: (
                    <div>
                        <p>此意图及其最近的意图使用相似的词语。</p>
                        <ul>
                            <li>使用这些词语为此意图添加更多话语。这将减少这些词对其他意图的重量。</li>
                            <li>如果两个意图需要使用相似的单词，但顺序不同，请考虑添加一个模式。</li>
                        </ul>
                    </div>
                )
            })
        }
        if (unclear.length > 0) {
            suggestions.push({
                key: '3',
                content: (
                    <div>
                        <p>这个意图和它最接近的意图有相似的词。</p>
                        <ul>
                            <li>考虑将此意图与其竞争对手合并。例如，“Order Sandwich”和“Order Pizza”可能会被合并为一个更广泛的意图，称为“订单午餐”。</li>
                        </ul>
                    </div>
                )
            })
        }

        return suggestions;
    }

    render() {
        const { detail } = this.state;
        const { errorDetail, intents, activeIntent = {} } = this.props;

        const { modelId } = activeIntent;

        return (
            <div className={styles.container}>
                <Row style={{ height: '100%' }}>
                    <Col md={6} sm={24} className={styles.left}>
                        <IntentList className={styles.intents}
                            data={intents}
                            activeKey={modelId}
                            onItemClick={this.itemClick.bind(this)} />
                        <Suggestions className={styles.suggestions}
                            suggestions={this.getSuggestions()} />
                    </Col>
                    <Col md={18} sm={24}>
                        <IntentAnalysis metadataModel={activeIntent}
                            detail={errorDetail} />
                    </Col>
                </Row>
            </div >
        )
    }
}