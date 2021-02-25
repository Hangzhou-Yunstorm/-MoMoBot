import React from 'react';
import {
    Form, Icon, Input, Button, Checkbox,
} from 'antd';
import styles from './index.less';
import connect from 'dva';
import { importCDN } from '../../utils/common';
import settings from '../../settings';

const FormItem = Form.Item;

class LoginForm extends React.Component {
    constructor(props) {
        super(props);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.state = {
            btnDisabled: true
        }
    }

    componentDidMount() {
        const { form: { setFieldsValue } } = this.props;
        importCDN(`https://www.recaptcha.net/recaptcha/api.js?render=${settings.siteKey}`)
            .then(() => {
                grecaptcha.ready(() => {
                    grecaptcha.execute(settings.siteKey, { action: 'login' }).then(token => {
                        //将Token写入隐藏域等等
                        setFieldsValue({ "code": token })
                        this.setState({ btnDisabled: false })
                    });
                });
            })
    }

    componentWillUnmount() {
        const badge = document.getElementsByClassName('grecaptcha-badge')[0];
        if (badge) {
            document.body.removeChild(badge.parentNode);
        }
    }


    render() {
        const { getFieldDecorator } = this.props.form;
        return (
            <Form onSubmit={this.handleSubmit} className={styles.login_form}>
                {getFieldDecorator('code')(
                    <Input name="code" type="hidden" />
                )}
                <FormItem>
                    {getFieldDecorator('username', {
                        rules: [{ required: true, message: 'Please input your username!' }],
                    })(
                        <Input prefix={<Icon type="user" style={{ color: 'rgba(0,0,0,.25)' }} />} name="username" placeholder="Username" />
                    )}
                </FormItem>
                <FormItem>
                    {getFieldDecorator('password', {
                        rules: [{ required: true, message: 'Please input your Password!' }],
                    })(
                        <Input prefix={<Icon type="lock" style={{ color: 'rgba(0,0,0,.25)' }} />} name="password" type="password" placeholder="Password" />
                    )}
                </FormItem>
                <FormItem>
                    {getFieldDecorator('remember', {
                        valuePropName: 'checked',
                        initialValue: true,
                    })(
                        <Checkbox>Remember me</Checkbox>
                    )}
                    <a className={styles.login_form_forgot} href="">Forgot password</a>
                    <Button disabled={this.state.btnDisabled} loading={this.props.submitting} type="primary" htmlType="submit" className={styles.login_form_button}>
                        {this.props.submitting ? '登录中' : '登录'}
                    </Button>
                </FormItem>
            </Form>
        )
    }

    handleSubmit(e) {
        e.preventDefault();
        this.props.form.validateFields((err, values) => {
            if (!err) {
                const { login } = this.props;
                login(values);
            }
        });
    }

}

export default Form.create()(LoginForm)