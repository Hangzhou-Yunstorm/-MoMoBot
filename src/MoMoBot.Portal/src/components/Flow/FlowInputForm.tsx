import * as React from 'react';
import { Form, Input, Button } from 'antd';
import * as moment from 'moment';
const TextArea = Input.TextArea;

class FlowInputForm extends React.PureComponent<any>{

    submitHanlder(e: any) {
        const { form, onSubmit } = this.props;
        e.preventDefault();
        form.validateFields((err: any, values: any) => {
            if (err) return;
            form.resetFields();
            console.log(values);
            onSubmit && onSubmit(values);
        });
    }

    render() {
        const { model = {}, form: { getFieldDecorator } } = this.props;

        return (
            <Form onSubmit={this.submitHanlder.bind(this)}>
                {
                    model.id &&
                    getFieldDecorator('flowId', {
                        initialValue: model.id,
                    })(
                        <input type="hidden" />
                    )
                }
                <Form.Item label="流程名称">
                    {getFieldDecorator('name', {
                        initialValue: model.name,
                        rules: [{ required: true, message: '请输入流程名称' }],
                    })(
                        <Input placeholder="请输入流程名称" />
                    )}
                </Form.Item>
                <Form.Item label="标识">
                    {getFieldDecorator('key', {
                        initialValue: model.key,
                        rules: [{ required: true, message: '请输入流程标识' }],
                    })(
                        <Input placeholder="请输入流程标识" />
                    )}
                </Form.Item>
                <Form.Item label="备注">
                    {getFieldDecorator('remark', {
                        initialValue: model.remark,
                    })(
                        <TextArea placeholder="输入备注信息" rows={5} />
                    )}
                </Form.Item>
                <Form.Item label="创建时间">
                    {getFieldDecorator('creationTime', {
                        initialValue: model.creationTime ? model.creationTime : moment().format('lll')
                    })(
                        <Input readOnly />
                    )}
                </Form.Item>
                <Button type="primary" htmlType="submit">提交</Button>
            </Form>
        )
    }
}

export default Form.create()(FlowInputForm);