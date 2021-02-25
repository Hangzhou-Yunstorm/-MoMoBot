import * as React from 'react';
import { DetailPanel, NodePanel, EdgePanel, GroupPanel, MultiPanel, CanvasPanel } from 'gg-editor';
import DetailForm from './DetailForm';
import { Card } from 'antd';
const styles = require('./index.less');

export default class FlowDetailPanel extends React.PureComponent {

    render() {
        return (
            <DetailPanel className={styles.detailPanel}>
                <NodePanel>
                    <DetailForm type="node" />
                </NodePanel>
                <EdgePanel>
                    <DetailForm type="edge" />
                </EdgePanel>
                <GroupPanel>
                    <DetailForm type="group" />
                </GroupPanel>
                <MultiPanel>
                    <Card type="inner" size="small" title="Multi Select" bordered={false} />
                </MultiPanel>
                <CanvasPanel>
                    <Card type="inner" size="small" title="Canvas" bordered={false} />
                </CanvasPanel>
            </DetailPanel>
        )
    }
}