import * as React from 'react';
import { connect } from 'dva';
import { Row, Col, Avatar } from 'antd';

class Center extends React.Component<any> {
    render() {
        const { userInfo } = this.props;
        console.log(userInfo);
        return (
            <Row>
                <Col sm={24} md={8} lg={6}>
                    <Avatar size="large" src={userInfo.avatar}/>
                </Col>
                <Col sm={24} md={16} lg={18}>
                    <h1>{userInfo.nickname}</h1>
                </Col>
            </Row>
        )
    }
}

export default connect(
    (state: any) => ({
        userInfo: state.identity.userInfo
    })
)(Center)