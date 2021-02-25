import settings from '../settings';
import request from '../utils/request';

export const getContacts=()=>{
    return request(`${settings.serverUrl}/api/chat/list`,{
        method:'GET'
    });
}

export const getRecords=(payload:any)=>{
    return request(`${settings.serverUrl}/api/chat/records`,{
        method:'GET',
        params:{...payload}
    });
}

export const getProfile=(identityId:string,from='dtalk')=>{
    return request(`${settings.serverUrl}/api/chat/customerinfo`,{
        method:'GET',
        params:{
            identityId,
            from
        }
    });
}

export const getServiceRecords = (identityId:string)=>{
    return request(`${settings.serverUrl}/api/chat/service-records`,{
        method:'GET',
        params:{
            identityId
        }
    });
} 