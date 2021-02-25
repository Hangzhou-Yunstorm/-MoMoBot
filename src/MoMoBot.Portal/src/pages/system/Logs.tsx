import * as React from 'react';
import { connect } from 'dva';
import LogDataTable from '../../components/Logs/LogDataTable';
import SearchOperations from '../../components/Logs/SearchOperations';
const styles = require("./Logs.less");

const pageSize = 10;

@connect(({ logs, loading }: { logs: any, loading: any }) => ({
    ...logs,
    loading: loading.effects['logs/fetch']
}))
export default class Logs extends React.PureComponent<any> {
    componentDidMount() {
        this.props.dispatch({ type: 'logs/fetch', payload: { pageSize, pageIndex: 1 } })
    }

    onPageChange(pagination: any) {
        const { current } = pagination;
        this.props.dispatch({ type: 'logs/fetch', payload: { pageSize, pageIndex: current } })
    }

    render() {
        const { data = [], total = 0, loading } = this.props;
        return (
            <div className={styles.logs}>
                <SearchOperations className={styles.operations} />
                <LogDataTable pagination={{ total, pageSize }}
                    loading={loading}
                    data={data}
                    onChange={this.onPageChange.bind(this)} />
            </div>
        )
    }
}