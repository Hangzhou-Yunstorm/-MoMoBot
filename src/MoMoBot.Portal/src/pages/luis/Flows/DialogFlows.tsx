import * as React from 'react';
import { connect } from 'dva';
import { Table, Button, Divider, Drawer, Popconfirm } from 'antd';
import * as moment from 'moment';
import router from 'umi/router';
import request from '../../../utils/request';
import settings from '../../../settings';
import { withRouter } from 'react-router';
const styles = require('./DialogFlows.less');

const FlowInputForm = React.lazy(() => import('../../../components/Flow/FlowInputForm'));

@connect(({ flow, loading }: { flow: any, loading: any }) => ({
    flows: flow.flows,
    editModel: flow.editModel,
    loading: loading.effects['flow/fetchFlows']
}))
class Flows extends React.PureComponent<any> {
    state = {
        drawerVisible: false,
    }

    componentDidMount() {
        this.props.dispatch({ type: 'flow/fetchFlows' })
    }

    addFlow() {
        this.setState({ drawerVisible: true });
    }

    editHanlder(record: any) {
        this.setState({ drawerVisible: true });
        this.props.dispatch({
            type: 'flow/save',
            payload: {
                editModel: record
            }
        })
    }

    deleteHanlder(id: number) {

        request(`${settings.serverUrl}/api/knowledges/remove-flow?flowId=${id}`, {
            method: 'DELETE'
        }).then(() => {
            this.props.dispatch({ type: 'flow/fetchFlows' });
            this.onClose();
        })
    }

    onClose() {
        this.setState({ drawerVisible: false });
        this.props.dispatch({ type: 'flow/save', payload: { editModel: undefined } });
    }

    onFlowSubmit(values: any) {
        const { flowId } = values;
        if (flowId) {
            request(`${settings.serverUrl}/api/knowledges/edit-flow`, {
                method: 'POST',
                body: values
            }).then((response: any) => {
                this.props.dispatch({ type: 'flow/fetchFlows' });
                this.onClose();
            });
        } else {
            request(`${settings.serverUrl}/api/knowledges/create-flow`, {
                method: 'POST',
                body: values
            }).then((response: any) => {
                console.log(response);
                this.props.dispatch({ type: 'flow/fetchFlows' });
                this.onClose();
            })
        }

    }

    render() {
        const { flows, loading, editModel } = this.props;
        const { drawerVisible } = this.state;

        return (
            <React.Fragment>
                <div>
                    <div className={styles.operator}>
                        <Button type="primary" icon="plus" onClick={this.addFlow.bind(this)}>添加流程</Button>
                    </div>
                    <Table dataSource={flows}
                        rowKey="id"
                        loading={loading}>
                        <Table.Column title="名称" dataIndex="name" />
                        <Table.Column title="标识" dataIndex="key" />
                        <Table.Column title="创建时间" dataIndex="creationTime" render={date => moment(date).format('lll')} />
                        <Table.Column title="备注" dataIndex="remark" />
                        <Table.Column title="操作" width="240px" dataIndex="id" render={(id, record) => {
                            return (
                                <React.Fragment>
                                    <Button size="small" type="primary" onClick={this.editHanlder.bind(this, record)}>修改</Button>
                                    <Divider type="vertical" />
                                    <Button size="small" type="primary" onClick={() => router.push(`/luis/flows/${id}`)}>流程</Button>
                                    <Divider type="vertical" />
                                    <Popconfirm placement="top" title="确定要删除吗？" onConfirm={this.deleteHanlder.bind(this, id)} okText="删除" cancelText="取消">
                                        <Button size="small" type="danger">删除</Button>
                                    </Popconfirm>
                                </React.Fragment>
                            )
                        }} />
                    </Table>
                </div>
                <Drawer visible={drawerVisible}
                    onClose={this.onClose.bind(this)}
                    destroyOnClose={true}
                    width="350px">
                    <React.Suspense fallback={null}>
                        <FlowInputForm model={editModel} onSubmit={this.onFlowSubmit.bind(this)} />
                    </React.Suspense>
                </Drawer>
            </React.Fragment>
        )
    }
}

export default withRouter(Flows);