import * as React from 'react';
import { Drawer, Button, Form, Input, Rate, Checkbox } from 'antd';
import { FormComponentProps } from 'antd/lib/form';
import TextArea from 'antd/lib/input/TextArea';
import { getRecords } from '../../services/chat.service';
const moment = require('moment');
const styles = require('./RecordDetail.less');

export interface IRecordDetailProps {
    onClose?: () => void;
    onChildrenDrawerClose?: () => void;
    formProps: IDetailFromProps;
    visible?: boolean;
    childrenVisable?: boolean;
}

interface IDetailFromProps extends FormComponentProps {
    record?: any;
    submitting?: boolean;
    editable?: boolean;
    onDelete?: (id: number) => void;
    onSubmit?: (values: any) => void;
    onChatRecordOpen?: () => void;
}

class ChatRecords extends React.PureComponent<any> {
    render() {
        const { records } = this.props;
        return (
            <div>
                {JSON.stringify(records)}
            </div>
        )
    }
}

class DetailFrom extends React.PureComponent<IDetailFromProps> {
    handleSubmit = (e: any) => {
        e.preventDefault();
        const { form, onSubmit, record = {} } = this.props;
        form.validateFields((err, values) => {
            if (!err) {
                const { id } = record;
                onSubmit && onSubmit({ ...values, id });
            }
        });
    }

    remarkNode = () => { }

    render() {
        const { submitting, onDelete, editable, form, record = {}, onChatRecordOpen } = this.props;
        const { getFieldDecorator } = form;

        const { id, title, remarks, score, endOfServiceTime, recordCompletionTime, isDone } = record;

        const horizontalFormItemLayout = {
            labelCol: { span: 4 },
            wrapperCol: { span: 14 },
        }

        return (
            <Form layout="vertical" onSubmit={this.handleSubmit}>
                <Form.Item label="服务主题：">
                    {
                        editable ?
                            getFieldDecorator('title', {
                                initialValue: title,
                                rules: [{ required: true, message: '请填写本次服务主题' }],
                            })(<Input disabled={!editable} placeholder="请填写本次服务主题" />) :
                            <span className={styles.primarytext}>{title}</span>
                    }
                </Form.Item>
                <Form.Item label="服务总结：">
                    {
                        editable ?
                            getFieldDecorator('remarks', {
                                initialValue: remarks,
                                rules: [{ required: true, message: '请填写本次服务关键内容' }],
                            })(<TextArea
                                disabled={!editable}
                                style={{ width: '100%', minHeight: '150px' }}
                                placeholder="请填写本次服务关键内容"
                                rows={5}
                            />) :
                            <span className={styles.primarytext}>{remarks}</span>
                    }
                </Form.Item>
                <Form.Item label="客户评分：" {...horizontalFormItemLayout}>
                    {getFieldDecorator('score', {
                        initialValue: Number.parseInt(score)
                    })(
                        <Rate disabled />
                    )}
                </Form.Item>
                <Form.Item label="记录时间：" {...horizontalFormItemLayout}>
                    <time className={styles.primarytext}>{moment(endOfServiceTime).format('lll')}</time>
                </Form.Item>
                {
                    isDone &&
                    <Form.Item label="归档时间：" {...horizontalFormItemLayout}>
                        <time className={styles.primarytext}>{moment(recordCompletionTime).format('lll')}</time>
                    </Form.Item>
                }
                <Form.Item label="聊天记录：" {...horizontalFormItemLayout}>
                    <a onClick={onChatRecordOpen} className={styles.primarytext}>查看聊天记录</a>
                </Form.Item>
                <Form.Item label="是否完成：" {...horizontalFormItemLayout}>
                    {getFieldDecorator('isDone', {
                        valuePropName: 'checked',
                        initialValue: true
                    })(
                        <Checkbox disabled={!editable} />
                    )}
                </Form.Item>
                {
                    editable &&
                    <React.Fragment>
                        <Button htmlType="button" onClick={() => { onDelete &&onDelete(id)}} disabled={submitting} style={{ marginRight: '15px' }} type='danger'>删除</Button>
                        <Button loading={submitting} type="primary" htmlType="submit">提交</Button>
                    </React.Fragment>
                }
            </Form>
        )
    }
}
const DetailForm = Form.create()(DetailFrom)

export default class RecordDetail extends React.PureComponent<IRecordDetailProps, any> {
    constructor(props: IRecordDetailProps) {
        super(props);
        this.state = {
            chatRecords: []
        }
    }

    openChatRecord = () => {
        const { formProps: { record } } = this.props;
        if (record) {
            const { chatId } = record;
            getRecords({ chatId })
                .then((response: any) => {
                    this.setState({ chatRecords: response.rows })
                });
        }

    }

    render() {
        const { onClose, formProps, visible, childrenVisable, onChildrenDrawerClose } = this.props;
        const { onChatRecordOpen } = formProps;
        const { chatRecords } = this.state;

        const childrenOpenChatRecord = () => {
            onChatRecordOpen && onChatRecordOpen();
            this.openChatRecord();
        }

        return (
            <Drawer
                destroyOnClose={true}
                title="记录归档"
                width={520}
                closable={false}
                onClose={onClose}
                visible={visible}
            >
                <DetailForm {...formProps} onChatRecordOpen={childrenOpenChatRecord} />
                <Drawer
                    destroyOnClose={true}
                    title="聊天记录"
                    width={320}
                    closable={false}
                    onClose={onChildrenDrawerClose}
                    visible={childrenVisable}
                >
                    <ChatRecords records={chatRecords} />
                </Drawer>
            </Drawer>
        )
    }
}   