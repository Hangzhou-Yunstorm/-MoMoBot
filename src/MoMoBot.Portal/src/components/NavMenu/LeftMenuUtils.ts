import * as pathToRegexp from 'path-to-regexp';
import { urlToList } from '../_utils/pathTools';
import { IMenu } from '../../@types/IMenu';

/**
 * Recursively flatten the data
 * [{path:string},{path:string}] => {path,path2}
 * @param  menus
 */
export const getFlatMenuKeys = (menuData: IMenu[]) => {



    let keys: string[] = [];
    if (menuData && menuData.length) {
        menuData.forEach((item: IMenu) => {
            keys.push(item.link);
            if (item.children) {
                keys = keys.concat(getFlatMenuKeys(item.children));
            }
        });
    }
    return keys;
};

export const getMenuMatches = (flatMenuKeys: string[], path: string) =>
    flatMenuKeys.filter(item => {
        if (item) {
            return pathToRegexp(item).test(path);
        }
        return false;
    });
/**
 * 获得菜单子节点
 * @memberof SiderMenu
 */
export const getDefaultCollapsedSubMenus = (props: any) => {
    const {
        location: { pathname },
        flatMenuKeys,
    } = props;
    return urlToList(pathname)
        .map(item => getMenuMatches(flatMenuKeys, item)[0])
        .filter(item => item)
        .reduce((acc, curr) => [...acc, curr], ['/']);
};
