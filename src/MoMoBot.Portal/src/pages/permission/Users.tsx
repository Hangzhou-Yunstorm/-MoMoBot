import * as React from 'react';
import UsersTable from '../../components/Users/UsersTable';
import { connect } from 'dva';
import { Button, Modal } from 'antd';
import CreateUserDrawer from '../../components/Users/CreateUserDrawer';
import Animate from 'rc-animate';
const styles = require('./Users.less');

@connect(({ user, loading }: { user: any, loading: any }) => ({
    users: user.users,
    loading: loading.effects['user/fetchUsers']
}))
export default class Users extends React.PureComponent<any> {
    state = {
        createVisible: false,
        selectedRowKeys: []
    }

    componentDidMount() {
        const { dispatch } = this.props;
        dispatch({
            type: 'user/fetchUsers',
            payload: {
                pageIndex: 1,
                pageSize: 10
            }
        })
    }

    openCreateDrawer() {
        this.setState({ createVisible: true })
    }

    createUser(values: any) {
        console.log(values);
    }

    removeUsers() {
        const { selectedRowKeys: userIds } = this.state;
        if (userIds.length > 0) {
            Modal.confirm({
                title: '你确定要删除吗？',
                content: '删除这些用户将无法恢复，请谨慎操作！',
                okText: '删除',
                okType: 'danger',
                cancelText: '取消',
                onOk() {
                    console.log('delete：' + userIds);
                }
            });
        }
    }

    onSelectChange(selectedRowKeys: string[] | number[]) {
        this.setState({ selectedRowKeys });
    }

    render() {
        const { users, loading } = this.props;
        const { createVisible, selectedRowKeys } = this.state;
        const length = selectedRowKeys.length;
        return (
            <div>
                <div className={styles.operations}>
                    <Button icon="user-add" type="primary" onClick={this.openCreateDrawer.bind(this)}>添加用户</Button>
                    <Animate transitionName="fade">
                        {
                            length > 0 &&
                            <div className={styles.external}>
                                <Button icon="user-delete" type="danger" onClick={this.removeUsers.bind(this)}>删除用户</Button>
                                <span className={styles.tip}>已选择 {length} 项</span>
                            </div>
                        }
                    </Animate>
                </div>
                <UsersTable data={users}
                    loading={loading}
                    onSelectChange={this.onSelectChange.bind(this)} />
                <CreateUserDrawer visible={createVisible}
                    onClose={() => this.setState({ createVisible: false })}
                    onCreate={this.createUser}
                    roles={[{ name: '管理员', id: '123132' }, { name: '客服人员', id: '745645645' }]} />
            </div>
        )
    }
}