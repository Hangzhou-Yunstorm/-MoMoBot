import * as React from 'react';
import NavMenu from '../components/NavMenu';
import { connect } from 'dva';
import { IMenuState } from '../@types/IMenuState';
import { IIdentity } from '../@types/IIdentity';
const styles = require('./BasicLayout.less');
import Layout from '../components/PageLayout';
import { getPageTitle } from '../utils/getPageTitle';
import DocumentTitle from 'react-document-title';
import IChatState from '../@types/IChatState';
import Media from 'react-media';
import '../services/im.service';
import { offline, online } from '../services/im.service';

class BasicLayout extends React.Component<any> {

    componentDidMount() {
        const { dispatch, route: { routes } } = this.props;
        dispatch({
            type: 'menu/getMenuData',
            payload: routes
        })
        online();
    }

    componentWillUnmount() {
        offline();
    }

    getContentStyle = () => {
        const { open, isMobile } = this.props;
        if (isMobile) {
            return { top: '50px', width: '100%' }
        }
        if (open) {

            return { left: '286px', right: '0px' }
        }
        return { left: '56px', right: '0px' }
    };

    render() {
        const { children, breadcrumbNameMap, location: { pathname } } = this.props;
        return (
            <DocumentTitle title={getPageTitle(pathname, breadcrumbNameMap)}>
                <React.Fragment>
                    <NavMenu  {...this.props} />
                    <div className={styles.container}>
                        <div className={styles.content} style={{ ...this.getContentStyle() }}>
                            <Layout {...this.props}>
                                {children}
                            </Layout>
                        </div>
                    </div>
                </React.Fragment>
            </DocumentTitle >
        );
    }
}

export default connect(({ menu, identity }: { menu: IMenuState, chat: IChatState, identity: IIdentity }) => ({
    menuData: menu.menuData,
    breadcrumbNameMap: menu.breadcrumbNameMap,
    open: menu.hasChildren,
    userInfo: identity.userInfo
}))((props: any) =>
    <Media query='(max-width: 599px)'>
        {(isMobile: boolean) => (<BasicLayout {...props} isMobile={isMobile} />)}
    </Media>
);

