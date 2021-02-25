import React from 'react';
import { Spin, Icon } from 'antd';
import { connect } from 'dva';
const styles = require('./index.less');
const ServiceRecords = React.lazy(() => import('./ServiceRecords'));
const Profile = React.lazy(() => import('./Profile'));

const Suspense = React.Suspense;

const Loading = <Spin indicator={<Icon type="loading" style={{ fontSize: 24 }} spin />} />;

export default class CustomerProfile extends React.PureComponent {
    static defaultProps = {
        profile: { customer: undefined, loading: false },
        record: { records: [], loading: false }
    }

    render() {
        const { profile, record } = this.props;
        return (
            <div className={styles.customer}>
                <div className={styles.profile}>
                    <Suspense fallback={Loading}>
                        <Profile {...profile} />
                    </Suspense>
                </div>

                <div className={styles.records}>
                    <h2 className={styles.title}>服务记录</h2>
                    <Suspense fallback={Loading}>
                        <ServiceRecords {...record} />
                    </Suspense>
                </div>
            </div>
        )
    }
}