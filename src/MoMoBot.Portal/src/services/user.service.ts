import request from '../utils/request';
import settings from '../settings';

export const getUsers = (pagination: any) => {
    return request(`${settings.serverUrl}/api/users/all`, {
        method: 'get',
        params: pagination
    })
}

export const existed = (nameOrEmail: string) => {
    return request(`${settings.serverUrl}/api/users/existed?nameOrEmail=${nameOrEmail}`, {
        method: 'POST'
    })
}