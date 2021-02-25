import * as React from 'react';
import { Table } from 'antd';

const Column = Table.Column;

export interface IUnknownTableProps {
    loading?: boolean;
    data?: Array<any>;
    pagination?: any;
    total?: number;
    onChange?: (index?: number, size?: number) => void;
}

export default class UnknownTable extends React.PureComponent<IUnknownTableProps> {
    render() {
        const { data, loading, onChange, pagination } = this.props;
        return (
            <Table dataSource={data}
                onChange={(pagination) => {
                    const { pageSize, current } = pagination;
                    onChange && onChange(current, pageSize)
                }}
                rowKey="id"
                pagination={...pagination}
                rowSelection={{}}
                loading={loading}>
                <Column width="auto" dataIndex="content" title="内容" />
                <Column width="250px" dataIndex="remarks" title="说明" />
                <Column width="250px" dataIndex="timeOfOccurrence" title="时间" />
                <Column width="180px" dataIndex="id" title="操作" render={() => <a href="javascript:;">整理</a>} />
            </Table>
        )
    }
}