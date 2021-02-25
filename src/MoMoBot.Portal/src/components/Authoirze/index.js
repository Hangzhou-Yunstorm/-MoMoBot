
import React from 'react';
import { Redirect } from 'react-router';
import withRouter from 'umi/withRouter';
import { connect } from 'dva';

class Authoirze extends React.Component {
    render() {
        const { children, location, isAuthenticated } = this.props;
        let search = '/';
        if (location.pathname && location.pathname !== '') {
            search = `redirect=${encodeURIComponent(location.pathname)}`;
        }
        if (isAuthenticated) {
            return children;
        }
        return (<Redirect to={{
            pathname: '/account/login',
            search
        }} />)
    }
}

export default connect(
    ({ identity }) => ({
        isAuthenticated: identity.isAuthenticated
    })
)(withRouter(Authoirze));
