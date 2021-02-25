import request from '../utils/request';
import { storeService } from './storage.service';
import settings from '../settings';

export const authorize = async (params: any) => {
    const { username, password, remember,code } = params;
    return request(`${settings.serverUrl}/api/account/authenticate`, {
        method: 'POST',
        body: {
            email: username,
            password,
            remember,
            code
        },
    });
}

export const getUserInfo = async (params: any) => {
    return request('/api/account/userinfo', {
        method: 'GET'
    });
}

export const getToken = () => {
    return storeService.retrieve("Token", '');;
}

export const resetAuthority = () => {
    storeService.store('IsAuthorized', false);
    storeService.store('TokenExpire', '');
    storeService.store('UserData', '');
    storeService.store('Token', '');
    storeService.store('breadcrumbNameMap', '');
    storeService.store('menuData', '');
}