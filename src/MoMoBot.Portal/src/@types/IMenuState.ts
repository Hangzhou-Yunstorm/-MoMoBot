import { IMenu } from './IMenu';

export interface IMenuState {
    menuData: IMenu[],
    breadcrumbNameMap: any,
    drawerVisible: boolean,
    hasChildren: boolean
}