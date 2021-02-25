import * as React from 'react';
import { Avatar, Empty, Tag, Typography, Icon, Skeleton, Button } from 'antd';
const styles = require('./index.less');
const { Title } = Typography;

export interface IProfileProps {
    customer?: any;
    loading?: boolean;
    canReload?: boolean;
    reload?: () => void;
}

export default class CustomerProfile extends React.PureComponent<IProfileProps>{
    render() {
        const { customer, loading, reload, canReload } = this.props;
        console.log(customer);
        return (
            <div className={styles.content}>
                <Skeleton active={true}
                    title={false}
                    loading={loading}
                    paragraph={{ rows: 5, width: ['100%', '100%', '100%', '100%', '100%'] }}>
                    {
                        customer && customer.identityId ?
                            <React.Fragment>
                                <div className={styles.header}>
                                    <Avatar className={styles.avatar} size={64} icon="user" />
                                    <Title className={styles.name} level={4}>{customer.name}</Title>
                                    <Tag className={styles.from} color="blue">{customer.from}</Tag>
                                </div>
                                <div className={styles.details}>
                                    <table>
                                        <tbody>
                                            <tr className={styles.item}>
                                                <td><Icon type="check-circle" theme="twoTone" /></td>
                                                <td><span className={styles.item_title}>手机号码</span> {customer.mobile}</td>
                                            </tr>
                                            <tr className={styles.item}>
                                                <td><Icon type="check-circle" theme="twoTone" /></td>
                                                <td><span className={styles.item_title}>邮箱</span>{customer.email === null || customer.email === '' ? '未知' : customer.email}</td>
                                            </tr>
                                            <tr className={styles.item}>
                                                <td><Icon type="check-circle" theme="twoTone" /></td>
                                                <td><span className={styles.item_title}>部门</span>{customer.department}</td>
                                            </tr>
                                            <tr className={styles.item}>
                                                <td><Icon type="check-circle" theme="twoTone" /></td>
                                                <td><span className={styles.item_title}>职位</span>{customer.position}</td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                            </React.Fragment> :
                            <Empty style={{ margin: "0px auto" }}
                                description="未能获取到数据">
                                {canReload && <Button type="primary" onClick={reload}>刷新</Button>}
                            </Empty>
                    }
                </Skeleton>
            </div>
        )
    }
}