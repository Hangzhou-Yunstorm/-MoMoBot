import * as React from 'react';
import { Empty, Skeleton, Tag, Button } from 'antd';
const styles = require('./index.less');
const moment = require('moment')
import 'moment/locale/zh-cn';

export interface IServiceRecordsProps {
    loading?: boolean;
    records?: [];
    canReload?: boolean;
    reload?: () => void;
    recordClick?: (record: any) => void;
}

class RecordList extends React.PureComponent<any> {

    getRecordTitle = (record: any) => {
        if (record.title === null || record.title === '') {
            return `未归档记录${record.id}`
        }
        return record.title;
    }

    render() {
        const { records, onItemClick } = this.props;
        return (
            <ul className={styles.list}>
                {records.map((record: any, index: number) => (
                    <li key={index} className={styles.recorditem}
                        title={moment(record.endOfServiceTime).format('lll')}
                        onClick={() => { onItemClick && onItemClick(record) }}>
                        <span className={styles.record_title}>
                            <Tag color={record.isDone ? 'green' : 'magenta'}>{record.isDone ? '已归档' : '未归档'}</Tag>
                            {this.getRecordTitle(record)}
                        </span>
                        <time className={styles.record_time}>{moment(record.endOfServiceTime).calendar()}</time>
                    </li>
                ))}
            </ul>
        )
    }
}

export default class ServiceRecords extends React.PureComponent<IServiceRecordsProps> {

    render() {
        const { loading, records, recordClick, reload, canReload } = this.props;
        return (
            <div className={styles.content}>
                <Skeleton active={true}
                    title={false}
                    loading={loading}
                    paragraph={{ rows: 5, width: ['100%', '100%', '100%', '100%', '100%'] }}>
                    {
                        records && records.length > 0 ?
                            <RecordList records={records}
                                onItemClick={recordClick} /> :
                            <Empty description="未能获取到数据">
                                {canReload && <Button type="primary" onClick={reload}>刷新</Button>}
                            </Empty>
                    }
                </Skeleton>
            </div>
        )
    }
}