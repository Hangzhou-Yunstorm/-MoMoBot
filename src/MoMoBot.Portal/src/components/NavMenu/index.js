import React from 'react';
import LeftMenu from './LeftMenu';
import TopMenu from './TopMenu';
import styles from './index.less';
import { getFlatMenuKeys } from './LeftMenuUtils';

const NavMenuWrapper = React.memo(props => {
    const { menuData, isMobile } = props;
    const flatMenuKeys = getFlatMenuKeys(menuData);
    const style = getMenuContainerStyles(isMobile);
    return (
        <div className={styles.container} style={style}>
            {isMobile ?
                <TopMenu /> :
                <LeftMenu {...props} flatMenuKeys={flatMenuKeys} />
            }
        </div>
    )
});

const getMenuContainerStyles = (isMobile) => {
    if (isMobile) {
        return { width: '100%', height: '50px', backgroundColor: '#000000',zIndex: '999' }
    }
    return { width: '286px', height: '100%'}
}

export default NavMenuWrapper;