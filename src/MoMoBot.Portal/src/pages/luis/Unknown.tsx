import * as React from 'react';
import UnknownTable from '../../components/Unknown/UnknownTable';
import { connect } from 'dva';


@connect(({ unknown, loading }: { unknown: any, loading: any }) => ({
    ...unknown,
    loading: loading.effects['unknown/fetchUnknowns']
}))
export default class Unknown extends React.PureComponent<any> {
    componentDidMount() {
        this.props.dispatch({ type: 'unknown/fetchUnknowns' })
    }

    tableChange(pageIndex = 1, pageSize = 10) {
        this.props.dispatch({ type: 'unknown/fetchUnknowns', payload: { pageIndex, pageSize } })
    }

    render() {
        const { data, loading, total } = this.props;
        return (
            <div >
                <UnknownTable data={data}
                    onChange={this.tableChange.bind(this)}
                    loading={loading}
                    pagination={{ total }} />
            </div>

        )
    }
}