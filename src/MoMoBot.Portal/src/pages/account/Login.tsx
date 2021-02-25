import * as React from 'react';
import LoginForm from '../../components/LoginForm';
import { connect } from 'dva';
const styles = require('./Login.less');


class Login extends React.Component<any> {
    componentWillMount() {
        this.props.dispatch({
            type: 'identity/logout'
        })
    }
    
    render() {
        return (
            <div className={styles.login_container}>
                <h1 style={{ textAlign: 'center' }}>LOGIN</h1>
                <LoginForm login={this.login} {...this.props} />
            </div>
        );
    }

    login = (values: any) => {
        const { username, password, remember, code } = values;
        this.props.dispatch({
            type: 'identity/login',
            payload: {
                username,
                password,
                remember,
                code
            }
        })
    }
}

export default connect(
    ({ loading }: { loading: any }) => ({
        submitting: loading.effects['identity/login'],
    })
)(Login)