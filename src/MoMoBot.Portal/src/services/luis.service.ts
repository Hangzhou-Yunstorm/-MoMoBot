import request from '../utils/request';
import settings from '../settings';

export const publish = () => {
    return request(`${settings.serverUrl}/api/luis/publish`, {
        method: 'post'
    });
}

export const train = () => {
    return request(`${settings.serverUrl}/api/luis/train`, {
        method: 'post'
    })
}

export const getTrainingStatus = () => {
    return request(`${settings.serverUrl}/api/luis/trainingstatus`, {
        method: 'get'
    })
}

export const getEndpointHitHistory = (perDays = 7) => {
    return request(`${settings.serverUrl}/api/luis/endpointhitshistory`, {
        method: 'get',
        params: { perDays }
    });
}

export const getSatsMetadata = () => {
    return request(`${settings.serverUrl}/api/luis/statsmetadata`, {
        method: "GET"
    }).catch(() => ({}))
}

export const getAppStatusInfo = () => {
    return request(`${settings.serverUrl}/api/luis/appinfo`, {
        method: 'GET'
    })
}

export const getLuisStats = (id: string) => {
    return request(`${settings.serverUrl}/api/luis/intentstats?intentId=${id}`, {
        method: "GET"
    })
}