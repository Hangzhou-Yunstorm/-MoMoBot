import * as React from 'react';
import { FlowChart } from '../../../components/Charts';
import request from '../../../utils/request';
import settings from '../../../settings';
import GGEditor, { withPropsAPI } from 'gg-editor';
import { Row, Col, message, Modal } from 'antd';
import FlowToolbar from '../../../components/Flow/FlowToolbar';
import FlowItemPanel from '../../../components/Flow/FlowItemPanel';
import FlowDetailPanel from '../../../components/Flow/FlowDetailPanel';
import withRouter from 'umi/withRouter';
import { connect } from 'dva';
const styles = require('./DialogFlow.less');

const confirm = Modal.confirm;

interface IDialogFlowState {
    saveEnabled: boolean,
    isLatest: boolean,
    flowId: number,
    name: string,
    data: {
        nodes: Array<any>,
        edges: Array<any>
    }
}

@connect(({ flow }: { flow: any }) => ({
    commands: flow.commands
}))
class DialogFlow extends React.Component<any, IDialogFlowState> {
    constructor(props: any) {
        super(props);
        this.state = {
            name: '',
            saveEnabled: true,
            isLatest: true,
            flowId: 0,
            data: {
                nodes: [],
                edges: []
            }
        }
    }

    componentDidMount() {
        console.log(this.props.propsAPI);
        const { match: { params: { id } } } = this.props;
        this.fatchDate(id);
    }

    fatchDate(id: number) {
        this.setState({ flowId: id });
        request(`${settings.serverUrl}/api/knowledges/flow?flowId=${id}`, {
            method: 'GET'
        }).then((response: any) => {
            this.setState({ ...response })
        })
    }

    onChangeHandler(e: any) {
        console.log(e);
        const { action, updateModel, affectedItemIds, model, item = {} } = e;
        const { id, type } = item;
        console.log(updateModel);
        console.log(id);
        switch (action) {
            case 'add':
                this.add(id, type, model);
                break;
            case 'update':
                this.update(id, type, updateModel);
                break;
            case 'remove':
                this.remove(affectedItemIds, type);
                break;
            case 'changeData':
                break;
            default:
                console.error('no match action!');
                break;
        }
    }

    add(id: string, type: string, model: any) {
        if (type !== 'node' && type !== 'edge') {
            console.error('invalid type');
            return;
        }
        let { commands } = this.props;
        commands.push({ id, action: 'add', type, model });
        this.storeCommands(commands);
    }
    update(id: string, type: string, model: any) {
        let { data: { nodes, edges } } = this.state;
        let { commands } = this.props;

        const existed = commands.filter((m: any) => m.id === id);
        if (existed.length > 0) {
            commands = commands.map((command: any) => {
                if (command.id === id) {
                    command.model = { ...command.model, ...model }
                }
                return command;
            })
        } else {
            let preModels = type === 'node' ? nodes : edges;
            if (type === 'node') {
                var filters = preModels.filter(m => m.id === id);
                if (filters.length > 0) {
                    commands.push({ id, action: 'update', type, model: { ...filters[0], ...model } })
                }
            }
        }

        this.storeCommands(commands);
    }
    remove(ids: Array<string>, type: string) {
        let { commands } = this.props;
        ids.forEach(id => {
            const existed = commands.filter((m: any) => m.id === id);
            if (existed.length > 0) {
                commands = commands.filter((c: any) => c.id !== id);
            } else {
                // todo:
                commands.push({ id, action: 'remove' })
            }
        });

        this.storeCommands(commands);
    }

    storeCommands(commands: Array<any>) {
        // this.props.dispatch({
        //     type: 'flow/storeCommands',
        //     payload: commands
        // })
        console.log(commands);
        this.setState({ isLatest: commands.length <= 0 });
    }

    onSaveHanlder(data: any) {
        confirm({
            title: '保存提醒',
            content: '保存会覆盖原来的数据，请确认操作！',
            okText: '确定',
            cancelText: '取消',
            onOk: () => {
                if (!this.validate()) {
                    message.error("数据格式不正确，请检查重试！");
                    return;
                }
                this.setState({ saveEnabled: false });
                console.log(this.props.commands);
                const { flowId } = this.state;
                request(`${settings.serverUrl}/api/knowledges/update-flow`, {
                    method: 'POST',
                    body: {
                        flowId,
                        ...data
                    }
                }).then(() => {
                    this.setState({ saveEnabled: true });
                })
            },
            onCancel() {
            },
        });

    }

    validate() {
        // todo: 数据校验
        return true;
    }

    render() {
        const { saveEnabled, name } = this.state;

        return (
            <React.Fragment>
                <GGEditor className={styles.ggeditor}>
                <div className={styles.header}>{name}</div>
                    <Row type="flex" className={styles.editorHd}>
                        <Col span={24}>
                            <FlowToolbar saveEnabled={saveEnabled}
                                onSave={this.onSaveHanlder.bind(this)} />
                        </Col>
                    </Row>
                    <Row type="flex" className={styles.editorBd}>
                        <Col span={4} className={styles.editorSidebar}>
                            <FlowItemPanel />
                        </Col>
                        <Col span={16} className={styles.editorContent}>
                            <FlowChart onChange={this.onChangeHandler.bind(this)} className={styles.flow} data={this.state.data} />
                        </Col>
                        <Col span={4} className={styles.editorSidebar}>
                            <FlowDetailPanel />
                            {/* <EditorMinimap /> */}
                        </Col>
                    </Row>
                    {/* <FlowContextMenu /> */}

                </GGEditor>
            </React.Fragment>
        )
    }
}

export default withRouter(withPropsAPI(DialogFlow));