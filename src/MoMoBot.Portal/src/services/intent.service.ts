import request from '../utils/request';
import settings from '../settings';

export const search = (keyword: string) => {
    return request(`${settings.serverUrl}/api/luis/search-intents`, {
        params: {
            search: keyword
        }
    })
}

export const examples = (id: string) => {
    return request(`${settings.serverUrl}/api/luis/intents/${id}`, {
        method: 'get'
    })
}

export const addExample = (example: any) => {
    return request(`${settings.serverUrl}/api/luis/example`, {
        method: 'POST',
        body: example
    })
}

export const updateSettings = (data: any) => {
    return request(`${settings.serverUrl}/api/luis/update-intent`, {
        method: 'POST',
        body: data
    });
}

export const deleteIntent = (id: string) => {
    return request(`${settings.serverUrl}/api/luis/delete-intent/${id}`, {
        method: 'DELETE'
    });
}

export const createIntent = (intent: any) => {
    return request(`${settings.serverUrl}/api/luis/create-intent/`, {
        method: 'POST',
        body: intent
    });
}