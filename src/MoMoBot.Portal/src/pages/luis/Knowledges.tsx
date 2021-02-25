import * as React from 'react';
import { Button, Table, Divider, Drawer, Form, Input, Select, Checkbox, Modal, AutoComplete, Icon, Dropdown, Menu, Row, Col, Upload, message, Radio } from 'antd';
import { connect } from 'dva';
import { PaginationConfig } from 'antd/lib/table';
import { FormComponentProps } from 'antd/lib/form';
import PageLoading from '../../components/PageLoading';
import { fetchKnowledge, download } from '../../services/knowledge.service';
import { UploadChangeParam } from 'antd/lib/upload';
import settings from '../../settings';
import { getToken } from '../../services/identity.service';
const styles = require('./Knowledges.less');
const { TextArea } = Input;
const { Option } = Select;
const Dragger = Upload.Dragger;
const CheckboxGroup = Checkbox.Group;
const FormItem = Form.Item;
const confirm = Modal.confirm;

interface IIInputFormProps extends FormComponentProps {
    onSubmit: (values: any) => void;
    submitting?: boolean;
    knowledgeId: string;
    flows?: Array<any>
}

/**配置表单 */
class InputForm extends React.PureComponent<IIInputFormProps, any>{
    state = {
        loading: true,
        answerType: 1
    }
    static defaultProps = {
        submitting: false
    }

    componentDidMount() {
        const { knowledgeId, form } = this.props;
        fetchKnowledge(knowledgeId)
            .then((response: any) => {
                const { intent, flowId, answerType, answer, requestUrl, id, method, answerQueries = [] } = response;
                this.setState({ loading: false, answerType });
                let parameterIds: number[] = [];
                answerQueries.forEach((item: any) => {
                    const { parameterId } = item;
                    parameterIds.push(parameterId);
                });
                const fields = {
                    id, intent, answer, requestUrl, method, parameterIds, flowId, answerType
                };
                form.setFieldsValue({ ...fields });
            }).catch((e: any) => {
                this.setState({ loading: false });
                console.error(e);
            });

    }

    handleSubmit = (e: any) => {
        e.preventDefault();
        const { form, onSubmit } = this.props;
        form.validateFields((err, values) => {
            if (!err) {
                console.log(values);
                onSubmit(values);
            }
        });
    }

    urlValidator = (rule: any, value: any, callback: any) => {
        const url = value.trim();
        const urlRegex = /((([A-Za-z]{3,9}:(?:\/\/)?)(?:[\-;:&=\+\$,\w]+@)?[A-Za-z0-9\.\-]+|(?:www\.|[\-;:&=\+\$,\w]+@)[A-Za-z0-9\.\-]+)((?:\/[\+~%\/\.\w\-_]*)?\??(?:[\-\+=&;%@\.\w_]*)#?(?:[\.\!\/\\\w]*))?)/;
        // const urlRegex1 = /^([a-zA-Z0-9]([a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?\.)+[a-zA-Z]{2,6}$/;
        if (url !== '') {
            if (!(urlRegex.test(url))) {
                callback("请输入正确的URL！");
                return;
            }
        }
        callback();
    }

    methodValidator = (rule: any, value: any, callback: any) => {
        const { form } = this.props;
        let url = form.getFieldValue('requestUrl');
        if (url.trim() !== '') {
            if (value <= 0) {
                callback('请选择请求方式！');
                return;
            }
        }
        callback();
    }

    render() {
        const { submitting, form, flows } = this.props;
        const { loading, answerType } = this.state;

        const params = [
            { label: 'username', value: 2 },
            { label: 'email', value: 3 }
        ];

        return loading ?
            <PageLoading /> :
            <Form onSubmit={this.handleSubmit} className={styles.login_form}>
                <FormItem>
                    {form.getFieldDecorator('id', {
                        rules: [{ required: true }],
                    })(<Input type="hidden" />)}
                </FormItem>
                <FormItem label="意图">
                    {form.getFieldDecorator('intent', {
                        rules: [{ required: true, message: '请输入意图' }],
                    })(<Input placeholder="请输入意图" />)}
                </FormItem>
                <Form.Item label="回复类型">
                    {
                        form.getFieldDecorator('answerType', {
                        })(
                            <Radio.Group defaultValue={1} onChange={(e) => this.setState({ answerType: e.target.value })}>
                                <Radio.Button value={1}>文本</Radio.Button>
                                <Radio.Button value={6}>流程</Radio.Button>
                            </Radio.Group>
                        )
                    }
                </Form.Item>
                <FormItem label="回复内容">
                    {
                        answerType === 6 ?
                            form.getFieldDecorator('flowId', {
                                rules: [{ required: true, message: '请选择问答流程' }],
                            })(<Select>{flows && flows.map(flow => <Select.Option value={flow.flowId}>{flow.name}</Select.Option>)}</Select>) :
                            form.getFieldDecorator('answer', {
                                rules: [{ required: true, message: '请输入至少四个字符的规则描述！', min: 4 }],
                            })(<TextArea placeholder="请输入回复内容" rows={5} />)
                    }
                </FormItem>
                {/* <FormItem style={{ display: answerType === 6 ? '' : 'none' }}>
                    {
                        form.getFieldDecorator('flow', {
                            // rules: [{ required: true, message: '请选择问答流程' }],
                        })(<Select>{flows.map(flow => <Select.Option value={flow.key}>{flow.name}</Select.Option>)}</Select>)
                    }
                </FormItem> */}

                <FormItem label="请求地址">
                    {form.getFieldDecorator('requestUrl', {
                        rules: [{ validator: this.urlValidator }],
                    })(<Input placeholder="请求地址" />)}
                </FormItem>
                <FormItem label="请求方式">
                    {form.getFieldDecorator('method', {
                        rules: [{ validator: this.methodValidator }]
                    })(<Select>
                        <Option value={0}>请选择</Option>
                        <Option value={1}>GET</Option>
                        <Option value={2}>POST</Option>
                    </Select>)}
                </FormItem>
                <FormItem label="请求参数">
                    {form.getFieldDecorator('parameterIds')(<CheckboxGroup options={params} />)}
                </FormItem>
                <Button loading={submitting} type="primary" htmlType="submit">
                    {submitting ? '更新中' : '更新'}
                </Button>
            </Form>
    }
}
const UpdateForm = Form.create<any>()(InputForm);

/**添加表单 */
const CreateForm = Form.create<any>()((props: any) => {
    const { modalVisible, intents, form, handleAdd, handleModalVisible, resetSearchData, handleSearch } = props;

    const okHandle = () => {
        form.validateFields((err: any, fieldsValue: any) => {
            if (err) return;
            form.resetFields();
            handleAdd && handleAdd(fieldsValue);
        });
    }

    const onSearch = () => {
        const value = form.getFieldValue('intent');
        handleSearch && handleSearch(value);
    }

    return (
        <Modal visible={modalVisible}
            title="添加知识"
            destroyOnClose={true}
            onOk={okHandle}
            onCancel={() => handleModalVisible()}>
            <FormItem labelCol={{ span: 5 }} wrapperCol={{ span: 15 }} label="意图">
                {form.getFieldDecorator('intent', {
                    rules: [{ required: true, message: '请输入意图！' }],
                })(<AutoComplete dataSource={intents}
                    placeholder="请输入意图"
                    onFocus={() => resetSearchData()}
                    style={{ width: '100%' }}>
                    <Input suffix={<Icon type="search" onClick={onSearch} className="certain-category-icon" />} />
                </AutoComplete>)}
            </FormItem>
            <FormItem labelCol={{ span: 5 }} wrapperCol={{ span: 15 }} label="回复内容">
                {form.getFieldDecorator('answer', {
                    rules: [{ required: true, message: '请输入回复内容！' }],
                })(<TextArea placeholder="请输入回复内容" rows={3} />)}
            </FormItem>
        </Modal>
    )
})

/**导入数据 */
const UploadModal = (props: any) => {
    const { visible, onClose } = props;

    const handleChange = (info: UploadChangeParam) => {
        const { file: { status, response } } = info;
        if (status === "done") {
            message.success(response);
            onClose && onClose(true);
        } else if (status === 'error') {
            message.error(response)
        }
    }
    const token = getToken();

    return (
        <Modal visible={visible}
            destroyOnClose={true}
            onCancel={() => { onClose && onClose(false) }}
            footer={null}>
            <Dragger multiple={false}
                action={`${settings.serverUrl}/api/luis/import-knowledges`}
                headers={{ 'Authorization': `Bearer ${token}` }}
                onChange={handleChange}
                accept=".xls,.xlsx,.json">
                <p className="ant-upload-drag-icon">
                    <Icon type="inbox" />
                </p>
                <p className="ant-upload-text">点击或者拖动文件到这里以上传</p>
                <p className="ant-upload-hint">只能导入 Excel/Json 文件，一次只能上传单个文件</p>
            </Dragger>
        </Modal>
    )

}

@connect(({ knowledge, loading }: { knowledge: any, loading: any }) => ({
    knowledge,
    loading: loading.effects['knowledge/fetch'] || loading.effects['knowledge/delete'],
    updating: loading.effects['knowledge/update']
}))
export default class Knowledges extends React.PureComponent<any> {
    state = {
        searchText: '',
        knowledgeId: '',
        modalVisible: false,
        uploadModalVisiable: false
    }

    componentDidMount() {
        this.fetchData();
        this.props.dispatch({ type: 'knowledge/fetchDialogFlows' })
    }

    fetchData(payload = {}) {
        const { dispatch } = this.props;
        dispatch({
            type: 'knowledge/fetch',
            payload
        });
    }

    handleTableChange = (pagination: PaginationConfig) => {
        const { dispatch } = this.props;
        // todo : filter search 
        const params = {
            pageIndex: pagination.current,
            pageSize: pagination.pageSize
        }
        dispatch({
            type: 'knowledge/fetch',
            payload: { ...params }
        })
    }

    handleSetting(id: string) {
        this.setState({ knowledgeId: id });
        this.openDrawer();
    }

    handleDelete(id: string) {
        const { dispatch } = this.props;
        confirm({
            title: '操作提醒',
            content: '确认要删除这条知识吗？删除后无法恢复，请谨慎操作！',
            okText: "删了",
            cancelText: '算了',
            onOk() {
                dispatch({ type: 'knowledge/delete', payload: id })
            }
        });
    }

    handleUpdate = (value: any) => {
        const { dispatch } = this.props;
        dispatch({
            type: 'knowledge/update',
            payload: value
        });
    }

    onClose = () => {
        this.setState({
            knowledgeId: ''
        });
        this.closeDrawer();
    };

    openDrawer = () => {
        const { dispatch } = this.props;
        dispatch({ type: 'knowledge/open' });
    }
    closeDrawer = () => {
        const { dispatch } = this.props;
        dispatch({ type: 'knowledge/close' });
    }

    addClickHanlder() {
        this.setState({ modalVisible: true });
    }

    handleAdd = (values: any) => {
        this.setState({ modalVisible: false })
        const { dispatch } = this.props;
        dispatch({
            type: 'knowledge/add',
            payload: {
                ...values
            }
        })
    }

    handleModalVisible = () => { this.setState({ modalVisible: false }) }

    handleSearch = (value: string) => {
        const { dispatch } = this.props;
        dispatch({
            type: 'knowledge/searchIntents',
            payload: value
        })
    }

    resetSearchData = () => {
        const { dispatch } = this.props;
        dispatch({ type: "knowledge/resetIntents" })
    }

    onSearch = () => {
        const { searchText } = this.state;
        this.fetchData({ search: searchText })
    }

    exportFile = (format: string) => {
        download(format);
    }

    onUploadClose = (success: boolean = false) => {
        if (success) {
            const { searchText } = this.state;
            this.fetchData({ search: searchText });
        }
        this.setState({ uploadModalVisiable: false })
    }

    render() {
        const { knowledge: { data, open, intents, flows }, loading, updating } = this.props;
        const { list, pagination } = data;
        const { knowledgeId, modalVisible, uploadModalVisiable } = this.state;
        const eventHanles = {
            handleAdd: this.handleAdd,
            handleModalVisible: this.handleModalVisible,
            handleSearch: this.handleSearch,
            resetSearchData: this.resetSearchData
        }

        const menu = (
            <Menu>
                <Menu.Item key="1" onClick={this.exportFile.bind(this, 'excel')}>导出为Excel文件</Menu.Item>
                <Menu.Item key="2" onClick={this.exportFile.bind(this, 'json')}>导出为Json文件</Menu.Item>
                <Menu.Divider />
                <Menu.Item key="3" onClick={() => { this.setState({ uploadModalVisiable: true }) }}>从Excel/Json文件导入</Menu.Item>
                <Menu.Divider />
                <Menu.Item key="4">下载示例数据</Menu.Item>
            </Menu>
        );

        console.log(flows);

        return (
            <React.Fragment>
                <div className={styles.tableContainer}>
                    <Row className={styles.tableOperator}>
                        <Col className={styles.searchinput} lg={4} md={8} sm={24}>
                            <Input suffix={<Icon type="search" onClick={this.onSearch} className="certain-category-icon" />}
                                placeholder="搜索意图或者回复内容"
                                onChange={(e) => { this.setState({ searchText: e.target.value }) }}
                                onKeyDown={(e) => { e.keyCode === 13 && this.onSearch() }} />
                        </Col>
                        <Col className={styles.buttons} lg={20} md={16} sm={24}>
                            <Button icon="plus" type="primary" onClick={this.addClickHanlder.bind(this)}>添加知识</Button>
                            <Dropdown overlay={menu}>
                                <Button style={{ marginLeft: 8 }}>
                                    导入/导出 <Icon type="down" />
                                </Button>
                            </Dropdown>
                        </Col>
                    </Row>
                    <Table
                        loading={loading}
                        rowKey="id"
                        rowSelection={{}}
                        dataSource={list}
                        pagination={pagination}
                        onChange={this.handleTableChange}
                        size="middle"
                    >
                        <Table.Column width="120px" key="intent" title="意图" dataIndex="intent" />
                        <Table.Column key="answer" width="auto" title="回复内容" dataIndex="answer" render={(text, record: any) => text} />
                        <Table.Column width="180px" title="操作" render={(text, record: any) => {
                            const { id } = record;
                            return (
                                <React.Fragment>
                                    <Button type="primary" size="small" onClick={this.handleSetting.bind(this, id)}>配置</Button>
                                    <Divider type="vertical" />
                                    <Button type="danger" size="small" onClick={this.handleDelete.bind(this, id)}>删除</Button>
                                </React.Fragment>
                            )
                        }} />
                    </Table>
                </div>
                <Drawer visible={open}
                    onClose={this.onClose}
                    destroyOnClose={true}
                    width="350px">
                    <UpdateForm onSubmit={this.handleUpdate}
                        submitting={updating}
                        knowledgeId={knowledgeId}
                        flows={flows} />
                </Drawer>
                <CreateForm intents={intents} modalVisible={modalVisible} {...eventHanles} />
                <UploadModal visible={uploadModalVisiable} onClose={this.onUploadClose} />
            </React.Fragment>
        )
    }
}
