import * as React from 'react';
import { Icon } from 'antd';
import { connect } from 'dva';
import router from 'umi/router';
const styles = require('./Logout.less');

@connect(({ identity, loading }: { identity: any, loading: any }) => ({
    identity,
    exit: loading.effects['identity/logout']
}))
export default class Logout extends React.PureComponent<any> {

    componentDidMount() {
        const { dispatch } = this.props;
        dispatch({
            type: 'identity/logout'
        });
    }

    render = () => {
        const { exit } = this.props;
        console.log(exit);
        !exit && setTimeout(() => {
            router.push('/account/login');
        }, 2000);
        return (
            <div className={styles.logout}>
                <Icon className={styles.loading} type="loading" />
                <p>{exit ? "正在退出..." : "正在跳转..."}</p>
            </div>
        )
    }
}