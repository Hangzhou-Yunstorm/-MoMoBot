import * as signalR from "@aspnet/signalr";
import settings from '../settings';
import { getToken } from './identity.service';
import { notification } from 'antd';
import request from '../utils/request';

let connected = false;

const options: signalR.IHttpConnectionOptions = {
    accessTokenFactory: () => {
        return new Promise<string>((resolve, reject) => {
            try {
                const token = getToken();
                resolve(token)
            } catch (e) {
                reject(e);
            }
        })
    }
};
const connection = new signalR.HubConnectionBuilder()
    .withUrl(`${settings.serverUrl}/humanservice`, options)
    .build();

connection.on("connected", (args) => {
    console.log(args);
})

/**新客户加入 */
connection.on('newCustomerJoined', customer => {
    console.log(customer);
    window.g_app._store.dispatch({
        type: 'chat/newCustomer',
        payload: { ...customer }
    });
})

connection.on('customerOffline', groupName=>{
    console.log(groupName);
    window.g_app._store.dispatch({
        type: 'chat/customerOffline',
        payload: groupName
    });
})

/**接收到消息 */
connection.on('receiveMessage', message => {
    window.g_app._store.dispatch({
        type: 'chat/newMessage',
        payload: { ...message }
    });
    console.log(message);
})

connection.onclose(error => {
    console.error(error);
    connected = false;
    notification.warning({ message: '人工服务', description: "已断开人工服务！" });
})

export const online = () => {
    connection.start()
        .then(() => {
            console.log('hub connected!')
            connected = true;
            connection.invoke("CustomerServiceOnline")
                .then(() => {
                    notification.success({ message: '人工服务', description: "已连接人工服务，开始接收客户消息！" });
                });
        }).catch(e => {
            console.error(e);
            connected = false;
        })
}

export const offline = () => {
    connected && connection.stop()
        .then(() => {
            connected = false;
        })
        .catch(e => console.error(e));
}

export const send = (message: any) => {
    return connection.invoke("SendMessage", message)
}

export const sendImage = (value: File | string) => {
    const file = <File>value;
    const data = new FormData();
    data.append('file', file);
    
    return request(`${settings.serverUrl}/api/file/upload-image`, {
        method: 'post',
        body: data
    })
}