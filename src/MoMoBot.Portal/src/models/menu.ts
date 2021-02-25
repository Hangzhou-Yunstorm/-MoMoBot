import { IMenuState } from '../@types/IMenuState';
import { IMenu } from '../@types/IMenu';
import { storeService } from '../services/storage.service';

const initialState: IMenuState = {
    drawerVisible: false,
    menuData: storeService.retrieve('menuData'),
    breadcrumbNameMap: storeService.retrieve('breadcrumbNameMap'),
    hasChildren: false
};

const getSubMenu = (item: any): IMenu | null => {
    if (item && item != null) {
        if (item.routes && !item.hideChildrenInMenu && item.routes.some((child: any) => child.text)) {
            return {
                text: item.text,
                link: item.path,
                icon: item.icon,
                children: filterMenuData(item.routes)
            }
        } else {
            return { text: item.text, link: item.path, icon: item.icon, children: undefined };
        }
    }
    return null
}

const filterMenuData = (menuData: any): IMenu[] => {
    if (menuData.length) {
        return menuData.filter((item: any) => item.text && !item.hideInMenu)
            .map((item: any) => getSubMenu(item))
            .filter((item: IMenu) => item != null);
    }
    return [];
}

const getBreadcrumbNameMap = (menuData: any) => {
    const routerMap = {};

    const flattenMenuData = (data: any) => {
        data.forEach((menuItem: any) => {
            if (menuItem.children) {
                flattenMenuData(menuItem.children);
            }
            // Reduce memory usage
            routerMap[menuItem.path] = menuItem;
        });
    };
    let flatData = getFlatMenu(menuData);
    flattenMenuData(flatData);
    return routerMap;
};

const getFlatMenu = (menuData: any) => {
    let result: any[] = [];
    if (menuData && menuData.length) {
        menuData.forEach((item: any) => {
            // result.push({ text: item.text, path: item.path });
            result.push({ ...item });

            result = result.concat((getFlatMenu(item.routes)))
        })
    }
    return result.filter(i => i.text && i.path);
}

export default {
    namespace: 'menu',
    state: {
        ...initialState
    },
    effects: {
        *getMenuData({ payload }: { payload: any }, { put }: { put: any }) {
            const menus: IMenu[] = storeService.retrieve('menuData', []);
            if (menus && menus.length > 0) {
                return;
            }
            const menuData = filterMenuData(payload);
            const breadcrumbNameMap = getBreadcrumbNameMap(payload);
            yield put({
                type: 'save',
                payload: { menuData, breadcrumbNameMap },
            });
        },
        *setChildrenStatus({ payload }: { payload: any }, { put }: { put: any }) {
            const { hasChildren } = payload;
            yield put({
                type: 'toggle',
                payload: { hasChildren },
            });
        }
    },
    reducers: {
        'toggle'(state: IMenuState, { payload }: { payload: any }) {
            return {
                ...state,
                hasChildren: payload.hasChildren
            }
        },
        save(state: any, { payload: { menuData, breadcrumbNameMap } }: { payload: any }) {
            storeService.store("menuData", menuData);
            storeService.store("breadcrumbNameMap", breadcrumbNameMap);
            return {
                ...state,
                menuData,
                breadcrumbNameMap
            }
        }
    }
}