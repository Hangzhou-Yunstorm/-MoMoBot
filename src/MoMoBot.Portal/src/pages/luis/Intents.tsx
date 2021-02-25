import * as React from 'react';
import { Card, Table, Button, Divider, Input, Icon, Modal, Form, Drawer, Tree, message, Alert, notification } from 'antd';
import { connect } from 'dva';
import { Link } from 'react-router-dom';
import request from '../../utils/request';
import settings from '../../settings';
import { updateSettings, deleteIntent, createIntent } from '../../services/intent.service';
import { publish, train, getTrainingStatus } from '../../services/luis.service';
const styles = require('./Intents.less');

const { TreeNode } = Tree;

/**添加新意图 */
const CreateIntentModal = Form.create()((props: any) => {
    const { form, visible, handleModalVisible, handleCreate } = props;
    const { getFieldDecorator } = form;
    const okHandle = () => {
        form.validateFields((err: any, fieldsValue: any) => {
            if (err) return;
            form.resetFields();
            handleCreate && handleCreate(fieldsValue);
        });
    }

    return (
        <Modal
            title="添加意图"
            visible={visible}
            onOk={okHandle}
            onCancel={handleModalVisible}
        >
            <Form>
                <Form.Item label="意图名称（必填）">
                    {
                        getFieldDecorator('intent', {
                            rules: [{ required: true, message: '请输入意图名称' }],
                        })(
                            <Input placeholder="输入意图名称" />
                        )
                    }
                </Form.Item>
            </Form>
        </Modal>
    )
})

/**意图配置 */
class SettingFormContent extends React.PureComponent<any, any> {
    constructor(props: any) {
        super(props);
        this.state = {
            permissionChecked: [],
            permissions: []
        }
    }

    componentDidMount() {
        const { intent: { name } } = this.props;
        request(`${settings.serverUrl}/api/department/intentpower?intent=${name}`, {
            method: 'get'
        }).then((response: any) => {
            const checked = response.map((item: any) => `${item}`);
            this.setState({ permissions: checked, permissionChecked: checked })

        })
    }

    renderTreeNodes = (data: any) => data.map((item: any) => {
        if (item.children) {
            return (
                <TreeNode title={item.text} key={item.id} dataRef={item}>
                    {this.renderTreeNodes(item.children)}
                </TreeNode>
            );
        }
        return <TreeNode {...item} dataRef={item} />;
    });
    let: any = [];

    submit = (e: any) => {
        const { form, onSubmit } = this.props;
        const { permissionChecked, permissions } = this.state;

        e.preventDefault();
        form.validateFields((err: any, fieldsValue: any) => {
            if (err) return;
            form.resetFields();
            let values = { ...fieldsValue, permissions: permissionChecked };
            if (permissionChecked.sort().toString() !== permissions.sort().toString()) {
                values.updatePermission = true;
            }
            onSubmit && onSubmit(values);
        });
    }

    render() {
        const { departments, intent, form } = this.props;
        const { getFieldDecorator } = form;
        const { permissionChecked } = this.state;

        return (
            <Form onSubmit={this.submit}>
                <Form.Item>
                    {
                        getFieldDecorator('id', {
                            initialValue: intent.id
                        })(
                            <Input type="hidden" />
                        )
                    }
                </Form.Item>
                <Form.Item label="意图名称">
                    {
                        getFieldDecorator('intent', {
                            rules: [{ required: true, message: '请输入意图名称' }],
                            initialValue: intent.name
                        })(
                            <Input type="text" placeholder="输入意图名称" />
                        )
                    }
                </Form.Item>
                <Form.Item label="权限设置">
                    <Tree onCheck={(checkedKeys) => {
                        if (Array.isArray(checkedKeys)) {
                            this.setState({ permissionChecked: checkedKeys })
                        } else {
                            this.setState({ permissionChecked: checkedKeys.checked })
                        }
                    }}
                        checkedKeys={permissionChecked}
                        checkable={true}
                        checkStrictly={true}
                        defaultExpandAll={true}>
                        {this.renderTreeNodes(departments)}
                    </Tree>
                </Form.Item>
                <Button type="primary" htmlType="submit">更新</Button>
            </Form>
        )
    }
}
const SettingForm = Form.create()(SettingFormContent)


@connect(({ luis, department, loading }: { luis: any, department: any, loading: any }) => ({
    intents: luis.intents,
    departments: department.departments,
    pagination: luis.pagination,
    alert: luis.alert,
    loading: loading.effects['luis/fetchIntents']
}))
export default class Intents extends React.PureComponent<any, any> {
    constructor(props: any) {
        super(props);
        this.state = {
            publishing: false,
            training: false,
            searchText: '',
            visible: false,
            drawerVisible: false,
            editIntent: {},
        }
    }
    componentDidMount() {
        const { dispatch } = this.props;
        const { searchText } = this.state;
        dispatch({ type: 'luis/fetchIntents', payload: searchText });
        dispatch({ type: 'department/fetchDepartments' });
    }

    onSearch = () => {
        const { dispatch } = this.props;
        const { searchText } = this.state;
        dispatch({ type: 'luis/fetchIntents', payload: searchText })
    }

    handleTableChange = () => { }

    /**添加新意图 */
    createNewIntent = (value: any) => {
        this.setState({ visible: false });
        createIntent(value).then(() => {
            message.success('添加成功!');
            this.onSearch();
        }).catch(() => {
            message.error('添加失败!');
        });
    }

    /**配置 */
    settingIntent = (intent: any) => {
        this.setState({ drawerVisible: true, editIntent: intent });
    }

    /**更新 */
    onIntentSettingSubmit = (values: any) => {
        this.setState({ drawerVisible: false, editIntent: {} })
        updateSettings({ ...values }).then((response: any) => {
            this.onSearch();
        }).catch((e: any) => {
            console.error(e);
        })
    }

    /**删除意图 */
    onDeleteIntent = (intent: any) => {
        const { id } = intent;
        deleteIntent(id).then(() => {
            this.onSearch();
        }).catch((e: any) => {
            console.error(e);
        })
    }

    /**提示信息 */
    renderAlert(): React.ReactNode {
        const { alert: { msg = undefined, type = 'info' }, dispatch } = this.props;
        if (msg) {
            return (
                <Alert message={msg} type={type} closable={true} style={{ marginBottom: '15px' }}
                    onClose={() => {
                        dispatch({ type: 'luis/closeAlert' })
                    }}
                />
            )
        }
        return;
    }

    /**发布 */
    luisPublish() {
        this.setState({ publishing: true });
        const hide = message.loading('正在发布，请勿离开...', 0);
        publish().then((response: any) => {
            this.setState({ publishing: false });
            hide();
            notification.open({
                message: 'LUIS操作提醒',
                description: response,
                duration: 0,
            });
        }).catch(() => {
            this.setState({ publishing: false });
        })
    }

    /**训练 */
    luisTrain() {
        const { dispatch } = this.props;
        this.setState({ training: true })
        dispatch({ type: 'luis/alert', payload: { msg: '正在加入训练队列中，请勿离开！', type: 'info' } })
        train()
            .then((result: any) => {
                const { statusId } = result;
                if (statusId === 2) {
                    this.setState({ training: false });
                    dispatch({ type: 'luis/alert', payload: { msg: '训练成功！', type: 'success' } })
                } else {
                    const timer = setInterval(() => {
                        getTrainingStatus()
                            .then((response: Array<any>) => {
                                const fails = response.filter(s => s.details.statusId === 1);
                                const inProgress = response.filter(s => s.details.statusId === 3 || s.details.statusId === 9);
                                if (fails.length > 0) {
                                    this.setState({ training: false });
                                    clearInterval(timer);
                                    dispatch({ type: 'luis/alert', payload: { msg: `训练失败！（${fails[0].details.failureReason}）`, type: 'error' } })
                                } else if (inProgress.length > 0) {
                                    dispatch({ type: 'luis/alert', payload: { msg: '正在训练队列中，请等待训练完成！', type: 'info' } })
                                } else {
                                    this.setState({ training: false });
                                    clearInterval(timer);
                                    dispatch({ type: 'luis/alert', payload: { msg: '训练成功！', type: 'success' } })
                                }
                            })
                    }, 1000);
                }
            })
            .catch(() => {
                this.setState({ training: false });
                dispatch({ type: 'luis/alert', payload: { msg: '发生未知错误，请重试！', type: 'error' } })
            });
    }

    render() {
        const { searchText, visible, drawerVisible, editIntent, publishing, training } = this.state;
        const { loading, intents, pagination, departments } = this.props;
        console.log();

        return (
            <React.Fragment>
                {this.renderAlert()}
                <div className={styles.tableContainer}>
                    <div className={styles.header}>

                        <Button loading={training} onClick={this.luisTrain.bind(this)} type="default">训练</Button>
                        <Button disabled={publishing} type="primary" onClick={this.luisPublish.bind(this)}>发布</Button>

                    </div>
                    <div className={styles.tableOperator}>
                        <div>
                            <Button ghost={true} icon="plus" type="primary" onClick={() => this.setState({ visible: true })}>创建意图</Button>
                        </div>
                        <div>
                            <Input suffix={<Icon type="search"
                                onClick={this.onSearch}
                                className="certain-category-icon" />}
                                value={searchText}
                                placeholder="搜索意图"
                                onChange={(e) => { this.setState({ searchText: e.target.value }) }}
                                onKeyDown={(e) => { e.keyCode === 13 && this.onSearch() }} />
                        </div>
                    </div>
                    <Table
                        loading={loading}
                        rowKey="id"
                        rowSelection={{}}
                        dataSource={intents}
                        pagination={pagination}
                        onChange={this.handleTableChange}
                        size="middle"
                    >
                        <Table.Column width="auto" title="意图" dataIndex="name" render={(text, record: any) => <Link to={`/luis/intents/${record.id}`}>{text}</Link>} />
                        <Table.Column width="180px" title="操作" render={(text, intent: any) => {
                            if (intent.name === 'None') {
                                return "-"
                            }
                            return (
                                <React.Fragment>
                                    <Button type="primary" onClick={this.settingIntent.bind(this, intent)} size="small" >配置</Button>
                                    <Divider type="vertical" />
                                    <Button onClick={this.onDeleteIntent.bind(this, intent)} type="danger" size="small" >删除</Button>
                                </React.Fragment>
                            )
                        }} />
                    </Table>
                </div>


                <Drawer visible={drawerVisible}
                    onClose={() => this.setState({ drawerVisible: false, editIntent: {} })}
                    destroyOnClose={true}
                    width="350px">
                    <SettingForm departments={departments} intent={editIntent}
                        onSubmit={this.onIntentSettingSubmit} />
                </Drawer>

                <CreateIntentModal visible={visible}
                    handleCreate={this.createNewIntent}
                    handleModalVisible={() => this.setState({ visible: false })} />
            </React.Fragment >
        )
    }

}