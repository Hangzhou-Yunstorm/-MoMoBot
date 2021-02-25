import { IMenu } from './IMenu';

export interface IManagementMenu {
    menus: IMenu[],
    parentRoute: string
}