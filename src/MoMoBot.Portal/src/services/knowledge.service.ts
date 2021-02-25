import request from '../utils/request';
import settings from '../settings';

export const fetchKnowledges = (params: any) => {
    return request(`${settings.serverUrl}/api/luis/knowledges`, {
        method: 'GET',
        params
    });
}

export const fetchKnowledge = (id: string) => {
    if (!id || id === '') {
        return new Promise((resolve, reason) => {
            resolve({});
        });
    }
    return request(`${settings.serverUrl}/api/luis/knowledge/${id}`, {
        method: 'GET'
    });
}

export const updateKnowledge = (knowledge: any) => {
    return request(`${settings.serverUrl}/api/luis/update-knowledge`, {
        method: 'POST',
        body: { ...knowledge }
    });
}

export const deleteKnowledge = (id: string) => {
    if (!id || id === '') {
        return new Promise((resolve, reason) => {
            resolve({});
        });
    }
    return request(`${settings.serverUrl}/api/luis/delete-knowledge/${id}`, {
        method: 'DELETE'
    });
}

export const addKnowledge = (knowledge: any) => {
    return request(`${settings.serverUrl}/api/luis/create-knowledge`, {
        method: 'POST',
        body: { ...knowledge }
    });
}

export const download = (format = 'excel') => {
    let formElement = document.createElement('form');
    formElement.style.display = "display:none;";
    formElement.method = 'post';
    formElement.action = `${settings.serverUrl}/api/luis/export-knowledges`;
    formElement.target = 'callBackTarget';
    let inputElement = document.createElement('input');
    inputElement.type = 'hidden';
    inputElement.name = "format";
    inputElement.value = format;
    formElement.appendChild(inputElement);
    document.body.appendChild(formElement);
    formElement.submit();
    document.body.removeChild(formElement);
}

export const fetchUnknowns = (pagination: any) => {
    return request(`${settings.serverUrl}/api/knowledges/unknowns`, {
        method: 'GET',
        params: pagination
    })
}

export const fetchDialogFlows = () => {
    return request(`${settings.serverUrl}/api/luis/dialog-flows`, {
        method: 'GET'
    }).catch(() => [])
}