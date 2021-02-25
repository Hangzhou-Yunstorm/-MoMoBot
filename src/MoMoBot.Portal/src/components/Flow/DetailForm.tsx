import * as React from 'react';
import { Select, Form, Input, Card } from 'antd';
import { upperFirst } from 'lodash';
import { withPropsAPI } from 'gg-editor';

const Option = Select.Option;
const Item = Form.Item;
const Fragment = React.Fragment;

const inlineFormItemLayout = {
    labelCol: {
        sm: { span: 8 },
    },
    wrapperCol: {
        sm: { span: 16 },
    },
};

class DetailForm extends React.Component<any> {
    get item() {
        const { propsAPI } = this.props;

        return propsAPI.getSelected()[0];
    }

    handleSubmit = (e: any) => {
        if (e && e.preventDefault) {
            e.preventDefault();
        }

        const { form, propsAPI } = this.props;
        const { getSelected, executeCommand, update } = propsAPI;

        setTimeout(() => {
            form.validateFieldsAndScroll((err: any, values: any) => {
                if (err) {
                    return;
                }

                const item = getSelected()[0];

                if (!item) {
                    return;
                }

                executeCommand(() => {
                    update(item, {
                        ...values,
                    });
                });
            });
        }, 0);
    };

    renderEdgeFunctionSelect = () => {
        return (
            <Select onChange={this.handleSubmit}>
                <Option value="">NULL</Option>
                <Option value="MoMoBot.Service.Map.TestClass.YesOrNo">TrueOrFalse</Option>
            </Select>
        );
    };

    renderNodeDetail = () => {
        const { form } = this.props;
        const { label, question, key, func = '' } = this.item.getModel();

        return (
            <Fragment>
                <Item label="Label" {...inlineFormItemLayout}>
                    {form.getFieldDecorator('label', {
                        initialValue: label,
                    })(<Input onBlur={this.handleSubmit} />)}
                </Item>
                <Item label="回复" {...inlineFormItemLayout}>
                    {form.getFieldDecorator('question', {
                        initialValue: question,
                    })(<Input onBlur={this.handleSubmit} />)}
                </Item>
                <Item label="字段" {...inlineFormItemLayout}>
                    {form.getFieldDecorator('key', {
                        initialValue: key,
                    })(<Input onBlur={this.handleSubmit} />)}
                </Item>
                <Item label="调用" {...inlineFormItemLayout}>
                    {form.getFieldDecorator('func', {
                        initialValue: func,
                    })(this.renderEdgeFunctionSelect())}
                </Item>
            </Fragment>
        );
    };

    renderEdgeDetail = () => {
        const { form } = this.props;
        const { label = '' } = this.item.getModel();

        return (
            <Fragment>
                <Item label="条件" {...inlineFormItemLayout}>
                    {form.getFieldDecorator('label', {
                        initialValue: label,
                    })(<Input onBlur={this.handleSubmit} />)}
                </Item>
            </Fragment>
        );
    };


    render() {
        const { type } = this.props;

        if (!this.item) {
            return null;
        }

        return (
            <Card type="inner" size="small" title={upperFirst(type)} bordered={false}>
                <Form onSubmit={this.handleSubmit}>
                    {type === 'node' && this.renderNodeDetail()}
                    {type === 'edge' && this.renderEdgeDetail()}
                </Form>
            </Card>
        );
    }
}

export default Form.create()(withPropsAPI(DetailForm));