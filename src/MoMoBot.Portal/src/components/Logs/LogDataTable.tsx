import * as React from 'react';
import { Table, Tag, Typography } from 'antd';
import { PaginationConfig } from 'antd/lib/table';
import ReactJson from 'react-json-view'
import * as classNames from 'classnames';
const { Paragraph } = Typography;
const Column = Table.Column;

export interface ILogDataTableProps {
    loading?: boolean;
    data: Array<any>;
    pagination?: PaginationConfig;
    onChange?: (pagination: PaginationConfig) => void;
}

export default class LogDataTable extends React.PureComponent<ILogDataTableProps>{

    renderLevelTag(level: number) {
        switch (level) {
            case 0: {
                return <Tag>Verbose</Tag>
            }
            case 1: {
                return <Tag color="geekblue">Debug</Tag>
            }
            case 2: {
                return <Tag color="blue">Information</Tag>
            }
            case 3: {
                return <Tag color="orange">Warning</Tag>
            }
            case 4: {
                return <Tag color="volcano">Error</Tag>
            }
            case 5: {
                return <Tag color="#f50">Fatal</Tag>
            }
            default: {
                return <Tag color="blue">Unknown</Tag>
            }
        }
    }

    render() {
        const { data, pagination, loading, onChange } = this.props;
        return (
            <Table pagination={pagination}
                rowKey="timestamp"
                dataSource={data}
                loading={loading}
                expandedRowRender={record => <ReactJson displayDataTypes={false} name="log_event" src={JSON.parse(record.log_event)} />}
                onChange={onChange}>
                <Column title="日志消息" dataIndex="message" render={message =>
                    <Paragraph style={{ marginBottom: '0px' }}
                        ellipsis={{ rows: 2, expandable: true }}>{message}
                    </Paragraph>}
                />
                <Column width="100px" title="日志等级" dataIndex="level" render={level => this.renderLevelTag(level)} />
                <Column width="250px" title="发生时间" dataIndex="timestamp" />
            </Table>
        )
    }
}