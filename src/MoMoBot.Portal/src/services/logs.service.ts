import request from '../utils/request';
import settings from '../settings';

export const fetchLogs = (pagination: any) => {
    return request(`${settings.serverUrl}/api/system/logs`, {
        method: 'POST',
        body: {
            ...pagination
        }
    })
}