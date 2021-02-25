import * as React from 'react';
import { IMenu } from '../../@types/IMenu';
import { IUserInfo } from '../../@types/IUserInfo';
import { Link } from 'react-router-dom';
import { Icon, Tooltip } from 'antd';
const styles = require('./LeftMenu.less');
const logo = require('../../assets/logo.png');
import classnames from 'classnames';
import UserMenu from './UserMenu';
import { getDefaultCollapsedSubMenus } from './LeftMenuUtils';

interface ILeftMenuProps {
    menuData: IMenu[],
    dispatch?: any,
    userInfo: IUserInfo,
    location?: any,
    flatMenuKeys?: string[],
}

interface ITopMenuItemProps {
    onItemClick?: (menu: IMenu) => void;
    menu: IMenu,
    isactive: boolean,
    openKeys?: string[];
}

interface ISecondMenuItemProps {
    menu: IMenu,
    isactive?: boolean
}

class SecondMenuItem extends React.Component<ISecondMenuItemProps> {
    static defaultProps = {
        isactive: false
    }

    render() {
        const { menu, isactive } = this.props;
        return (
            <li className={classnames(styles.menuitem, isactive ? styles.active : '')}>
                <Link className={styles.menulink} to={menu.link}>{menu.text}</Link>
            </li>
        )
    }
}

class TopMenuItem extends React.Component<ITopMenuItemProps, any> {
    constructor(props: ITopMenuItemProps) {
        super(props);
        this.state = {
            tipVisible: false
        }
    }

    static defaultProps = {
        openKeys: [],
        badge:0
    }

    shouldComponentUpdate(nextProps: ITopMenuItemProps, nextState: any) {
        return nextProps.isactive !== this.props.isactive ||
            nextState.tipVisible !== this.state.tipVisible ||
            nextProps.openKeys !== this.props.openKeys;
    }

    menuItemClick = (menu: IMenu) => {
        const { onItemClick } = this.props;
        onItemClick && onItemClick(menu);
    }
    renderSecondMenus = (title: string, menus: IMenu[]) => {
        const { openKeys } = this.props;

        return (
            <ul className={styles.lowLevelmenus}
                onMouseEnter={() => this.setState({ tipVisible: false })}>
                <div className={styles.header}>
                    <h1 className={styles.title}>{title}</h1>
                </div>
                {
                    menus.map((menu, index) => {
                        let isactive = openKeys!.filter((k: string) => k === menu.link).length > 0;
                        return (<SecondMenuItem key={index} menu={menu} isactive={isactive} />)
                    })
                }
            </ul>
        )
    }

    render() {
        const { menu, isactive } = this.props;
        const { tipVisible } = this.state;
        return (
            <Tooltip visible={tipVisible} placement="right" title={menu.text}>
                <li onMouseEnter={() => this.setState({ tipVisible: true })}
                    onMouseLeave={() => this.setState({ tipVisible: false })}
                    onClick={this.menuItemClick.bind(this, menu)}
                    className={classnames(styles.menuitem, isactive ? styles.active : '')}>

                    <Link to={menu.link} className={styles.link}>
                        <Icon className={styles.icon} type={menu.icon} />
                    </Link>
                    {menu.children && menu.children.length > 0 && this.renderSecondMenus(menu.text, menu.children)}
                </li>
            </Tooltip>
        )
    }
}


class LeftMenu extends React.Component<ILeftMenuProps, any> {
    constructor(props: ILeftMenuProps) {
        super(props);
        this.state = {
            openKeys: getDefaultCollapsedSubMenus(props)
        }
    }

    static getDerivedStateFromProps(props: ILeftMenuProps, state: any) {
        const { pathname, flatMenuKeysLen } = state;
        if (props.location.pathname !== pathname || props.flatMenuKeys!.length !== flatMenuKeysLen) {
            return {
                pathname: props.location.pathname,
                flatMenuKeysLen: props.flatMenuKeys!.length,
                openKeys: getDefaultCollapsedSubMenus(props),
            };
        }
        return null;
    }

    static defaultProps = {
        menuData: [],
        flatMenuKeys: [],
        unreadCount: 0
    }

    menuItemClick = (menu: IMenu) => {
        this.setChildrenStatus(menu);
    }

    setChildrenStatus = (menu: IMenu) => {
        let hasChildren = menu.children && menu.children.length > 0;
        this.props.dispatch({
            type: 'menu/setChildrenStatus',
            payload: {
                hasChildren
            }
        })
    }

    renderTopMenus = () => {
        const { menuData } = this.props;
        const { openKeys } = this.state;
        if (menuData) {
            return menuData.map((menu, index) => {
                let isactive = openKeys.filter((k: string) => k === menu.link).length > 0;
                if (isactive) {
                    this.setChildrenStatus(menu);
                }
                return <TopMenuItem openKeys={openKeys} key={index} onItemClick={(menu) => { this.menuItemClick(menu) }} isactive={isactive} menu={menu} />
            });
        }
        return;
    }

    render() {
        const { userInfo } = this.props;
        return (
            <div className={styles.container}>
                <ul className={styles.menus}>
                    <Tooltip placement="right" title="控制台">
                        <li className={styles.menuitem}
                            onClick={this.menuItemClick.bind(this, [])}>
                            <Link to="/console">
                                <img className={styles.logo} src={logo} />
                            </Link>
                        </li>
                    </Tooltip>
                    {this.renderTopMenus()}

                    <UserMenu itemClick={this.menuItemClick.bind(this, [])} userInfo={userInfo} className={styles.usermenus} />
                </ul>
            </div>
        )
    }
}

export default LeftMenu;