import * as React from 'react';
import { connect } from 'dva';
import { Input, Table, Select, Dropdown, Menu, Icon } from 'antd';
import PageLoading from '../../components/PageLoading';
import { ClickParam } from 'antd/lib/menu';
import { addExample } from '../../services/intent.service';
const styles = require('./Intent.less');



@connect(({ luis, loading }: { luis: any, loading: any }) => ({
    detail: luis.detail,
    loading: loading.effects['luis/fetchExamples']
}))
export default class Intent extends React.PureComponent<any, any> {
    constructor(props: any) {
        super(props);
        this.state = {
            utterance: ''
        }
    }
    componentDidMount() {
        this.fetchExamples();
    }

    fetchExamples() {
        const { dispatch, match: { params: { id } } } = this.props;
        dispatch({ type: 'luis/fetchExamples', payload: id });
    }

    addUtterance = () => {
        const { detail: { intent: { name } } } = this.props;
        const { utterance } = this.state;
        console.log(utterance);
        this.setState({ utterance: '' });
        const data = { text: utterance, intent: name };
        console.log(data);
        addExample({ ...data }).then(() => {
            this.fetchExamples();
        })
    }

    tableMenuItemClick = (id: number, params: ClickParam) => {
        const { key } = params;
        // edit
        if (key === '0') {

        } else if (key === '1') {
            // delete
        }
    }

    render() {
        const { detail, loading } = this.props;
        const { utterance } = this.state;
        const { intent = {}, examples = [] } = detail;

        return (
            <div className={styles.container}>
                {
                    loading ?
                        <PageLoading /> :
                        <React.Fragment>
                            <div className={styles.headerContent}>
                                {intent.name}
                            </div>
                            <div className={styles.inputContent}>
                                <Input onKeyDown={(e) => { if (e.keyCode === 13) { this.addUtterance() } }}
                                    onChange={(e) => this.setState({ utterance: e.target.value })}
                                    value={utterance}
                                    placeholder="键入大约5个用户可能会说的内容的示例，然后按回车键"
                                />
                            </div>
                            <div className={styles.tableContent}>
                                <Table dataSource={examples}
                                    pagination={false}
                                    size="small">
                                    <Table.Column title="短语实例" width="auto" key="text" dataIndex="text" />
                                    <Table.Column
                                        title="标记的意图"
                                        width="180px"
                                        key="intentPredictions"
                                        dataIndex="intentPredictions"
                                        render={(item, row) => {
                                            return (
                                                <Select style={{ width: '180px' }} defaultValue={`${item[0].name}（${item[0].score.toFixed(3)}）`} >
                                                    {item.map((intent: any, index: number) => {
                                                        const title = `${intent.name}（${intent.score.toFixed(3)}）`;
                                                        return <Select.Option title={title} value={item.name}>{title}</Select.Option>
                                                    })}
                                                </Select>
                                            )
                                        }} />
                                    <Table.Column align="right" width="120px" dataIndex="id" key="id" render={(text, record) => (
                                        <Dropdown
                                            overlay={(
                                                <Menu onClick={this.tableMenuItemClick.bind(this, text)}>
                                                    <Menu.Item key="0">
                                                        <Icon type="edit" />编辑
                                                    </Menu.Item>
                                                    <Menu.Item key="1">
                                                        <Icon type="delete" />删除
                                                    </Menu.Item>
                                                </Menu>
                                            )}
                                            trigger={['click']}>
                                            <Icon type="more" />
                                        </Dropdown>
                                    )} />
                                </Table>
                            </div>
                        </React.Fragment>
                }
            </div>
        )
    }
}