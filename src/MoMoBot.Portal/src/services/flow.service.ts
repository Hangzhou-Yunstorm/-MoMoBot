import request from '../utils/request';
import settings from '../settings';

export const fetchFlows = () => {
    return request(`${settings.serverUrl}/api/knowledges/flows`, {
        method: 'GET'
    })
}