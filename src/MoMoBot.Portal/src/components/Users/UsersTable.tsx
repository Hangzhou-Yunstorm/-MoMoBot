import { Component } from 'react';
import { Table, Tag, Divider } from 'antd';
import * as React from 'react';

export interface IUsersTableProps {
    loading?: boolean;
    data: Array<any>;
    onSelectChange?: (selectedRowKeys: string[] | number[]) => void;
}

export default class UsersTable extends Component<IUsersTableProps> {
    render() {
        const { loading, data, onSelectChange } = this.props;
        return (
            <Table
                rowSelection={{
                    onChange: onSelectChange,
                    getCheckboxProps: record => ({
                        disabled: record.userName === 'admin', // Column configuration not to be checked
                        // name: record.name
                    })
                }}
                rowKey="id"
                loading={loading}
                dataSource={data}>
                <Table.Column title="昵称" dataIndex="nickname" />
                <Table.Column title="用户名" dataIndex="userName" />
                <Table.Column title="邮箱" dataIndex="email" />
                {/* <Table.Column title="锁定状态" dataIndex="lockoutEnabled" /> */}
                <Table.Column title="角色" dataIndex="roles" render={(roles, record: any) => roles.map((role: string) => (
                    role !== '' ? <Tag key={`${record.id}_${role}`} color={role === '管理员' ? 'gold' : 'blue'}>{role}</Tag> : '-'
                ))} />
                <Table.Column dataIndex="id" render={(id, record: any) => record.userName !== 'admin' ? (
                    <span>
                        <a href="javascript:;">编辑</a>
                        <Divider type="vertical" />
                        <a href="javascript:;">删除</a>
                    </span>
                ) : ''} />
            </Table>
        )
    }
}