import * as React from 'react';
import { connect } from 'dva';
import { IMenuState } from '../../@types/IMenuState';
import { Table, Tag } from 'antd';
import request from '../../utils/request';
import { PaginationConfig } from 'antd/lib/table';
import { PageHeader } from 'ant-design-pro';

const breadcrumbList = [{
    title: '首页',
    href: '/',
}, {
    title: '用户管理',
}];

class Users extends React.Component<any, any> {

    constructor(props: any) {
        super(props);
        this.state = {
            data: [],
            pagination: {},
            loading: false,
        };
    }

    componentDidMount() {
        this.fetchData();
    }

    fetchData(params = {}) {
        this.setState({ loading: true });
        request('/api/users', {
            method: 'GET',
            params: {
                pageSize: 10,
                pageIndex: 1,
                ...params,
            },
        }).then((response: any) => {
            const pagination = { ...this.state.pagination };
            pagination.total = response.total;
            this.setState({
                loading: false,
                data: response.data,
                pagination
            })
        })

    }

    handleTableChange = (pagination: PaginationConfig) => {
        const pager = { ...this.state.pagination };
        pager.current = pagination.current;
        this.setState({
            pagination: pager,
        });
        this.fetchData({
            pageSize: pagination.pageSize,
            pageIndex: pagination.current,
        });
    }

    handleEdit = (row: any) => {
        console.log('edit' + row.id);
    }

    render() {

        const columns = [{
            title: 'Id',
            dataIndex: 'id',
            sorter: true,
        }, {
            title: 'UserName',
            dataIndex: 'username',
            sorter: true,
        }, {
            title: 'Email',
            dataIndex: 'email',
        }, {
            title: '状态',
            dataIndex: 'state',
            render: (state: number) => (state === 0 ? <Tag color="red">锁定</Tag> : <Tag color="green">正常</Tag>)
        }, {
            title: '操作',
            key: 'action',
            render: (value: any, row: any, index: number) => (<a href="javascript:;" onClick={() => this.handleEdit(row)}>编辑</a>)
        }];

        return (
            <div>
                <PageHeader title="用户管理" breadcrumbList={breadcrumbList} />
                <Table
                    rowKey="id"
                    size="small"
                    columns={columns}
                    dataSource={this.state.data}
                    pagination={this.state.pagination}
                    loading={this.state.loading}
                    onChange={this.handleTableChange}
                />
            </div>
        )
    }
}

export default connect(
    ({ menu }: { menu: IMenuState }) => ({
        menuData: menu.menuData
    })
)(Users)