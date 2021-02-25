import * as React from 'react';
import { Drawer, Form, Input, Button, Select, Tooltip, Icon } from 'antd';
import { FormComponentProps } from 'antd/lib/form';
import Password from 'antd/lib/input/Password';
import RoleCheckGroup from './RoleCheckGroup';
import { existed } from '../../services/user.service';
import { Debounce } from 'lodash-decorators/debounce';

export interface ICreateUserDrawerProps extends FormComponentProps {
    visible?: boolean;
    width?: string | number;
    title?: React.ReactNode;
    roles?: Array<{ name: string, id: string }>;
    onClose?: () => void;
    onCreate?: (values: any) => void;
}

class CreateUserDrawer extends React.PureComponent<ICreateUserDrawerProps> {
    static defaultProps = {
        title: '添加用户账号',
        width: 380,
        visible: false
    }

    onSubmit(e: React.FormEvent) {
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                const { roles: { checkedRoles } } = values;
                const { onCreate } = this.props;
                onCreate && onCreate({ ...values, roles: checkedRoles });
            }
        });
    }

    @Debounce(300)
    nameOrEmailExistedValidator(rule: any, value: any, callback: any) {
        existed(value).then((result: boolean) => {
            if (result) {
                callback(`用户 ${value} 已存在`);
            } else {
                callback();
            }
        }).catch(() => callback())

    }

    render() {
        const { visible, onClose, width, title, form, roles } = this.props;
        const { getFieldDecorator } = form;
        return (
            <Drawer visible={visible}
                destroyOnClose={true}
                onClose={onClose}
                width={width}
                title={title}>
                <Form layout="vertical" onSubmit={this.onSubmit.bind(this)} hideRequiredMark>

                    <Form.Item label={(
                        <span>
                            Username&nbsp;
                                    <Tooltip title="用于登录的用户名">
                                <Icon type="question-circle-o" />
                            </Tooltip>
                        </span>
                    )}>
                        {getFieldDecorator('username', {
                            rules: [{ required: true, message: '请输入用户名' },
                            { validator: this.nameOrEmailExistedValidator }],
                        })(<Input placeholder="请输入用户名" />)}
                    </Form.Item>

                    <Form.Item label={(
                        <span>
                            E-mail&nbsp;
                                    <Tooltip title="用于登录或者找回密码的邮箱">
                                <Icon type="question-circle-o" />
                            </Tooltip>
                        </span>
                    )}>
                        {getFieldDecorator('email', {
                            rules: [{ required: true, message: '请输入邮箱地址' },
                            { validator: this.nameOrEmailExistedValidator }
                            ],
                        })(<Input placeholder="请输入邮箱地址" />)}
                    </Form.Item>

                    <Form.Item label="Nickname">
                        {getFieldDecorator('nickname', {
                            rules: [{ required: true, message: '请输入用户昵称' }],
                        })(
                            <Input placeholder="请输入用户昵称" />
                        )}
                    </Form.Item>

                    <Form.Item label="Password">
                        {getFieldDecorator('password', {
                            rules: [{}],
                        })(
                            <Password placeholder="输入初始密码" />
                        )}
                    </Form.Item>

                    {roles &&
                        <Form.Item label="Roles">
                            {getFieldDecorator('roles', {
                                initialValue: { roles },
                                rules: [{ required: true, message: 'Please choose the role' }],
                            })(
                                <RoleCheckGroup />
                            )}
                        </Form.Item>
                    }
                    <div
                        style={{
                            position: 'absolute',
                            left: 0,
                            bottom: 0,
                            width: '100%',
                            borderTop: '1px solid #e9e9e9',
                            padding: '10px 16px',
                            background: '#fff',
                            textAlign: 'right',
                        }}
                    >
                        <Button style={{ marginRight: 8 }} onClick={onClose}>
                            取消
                        </Button>
                        <Button type="primary" htmlType="submit">
                            创建
                    </Button>
                    </div>
                </Form>
            </Drawer>
        )
    }
}

export default Form.create<ICreateUserDrawerProps>()(CreateUserDrawer)