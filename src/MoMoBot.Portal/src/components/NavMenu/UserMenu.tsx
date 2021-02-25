import * as React from 'react';
import classNames from 'classnames';
import { IUserInfo } from '../../@types/IUserInfo';
const styles = require('./LeftMenu.less');
import { Tooltip, Modal, Icon } from 'antd';
import { Link } from 'react-router-dom';
import router from 'umi/router';

interface IUserMenuProps {
    className?: string,
    userInfo: IUserInfo,
    itemClick?:()=>{};
}

const confirm = Modal.confirm;

export default class UserMenu extends React.Component<IUserMenuProps> {

    logout() {
        confirm({
            title: '需要退出登录吗？',
            content: '退出登录会将您的登录信息全部清除',
            cancelText:'算了',
            okText:'退出',
            onOk() {
                router.push('/account/logout');
            },
            onCancel() {
            },
          });
    }

    render() {
        const { className, userInfo ,itemClick} = this.props;
        return (
            <ul className={classNames(className, styles.menus)}>
                <Tooltip placement="right" title="退出登录">
                    <li className={styles.menuitem}
                        onClick={this.logout.bind(this)}>
                        <Icon type="poweroff" />
                    </li>
                </Tooltip>
                <Tooltip placement="right" title={userInfo.nickname}>
                    <li className={styles.menuitem} onClick={()=>{itemClick && itemClick()}}>
                        <Link to="/user/center"><img className={styles.avatar} src={userInfo.avatar} /></Link>
                    </li>
                </Tooltip>

            </ul>
        )
    }
}