import * as signalR from '@aspnet/signalr';
import config from '../config'
import { guid } from 'react-yc-chatbox'

const getIdentityId = (defaultValue) => {
    let id = sessionStorage.getItem('IdentityId');
    if (!id || id === '') {
        id = defaultValue || guid();
        sessionStorage.setItem('IdentityId', id);
    }
    return id;
};
let connection = null;
let server = {};
let _ddUser = {};

const connectHub = (ddUser, funcs) => {
    const { Connected, Started, Error, Receive, Hangup, Cancel, Waiting } = funcs;
    const { userid, name } = ddUser;
    console.log(ddUser);
    if (!userid || !name) {
        Error && Error("未能获取到您的身份信息，此功能暂不可用！");
        return;
    }
    _ddUser = ddUser;
    const identityId = getIdentityId(userid);
    connection = new signalR.HubConnectionBuilder()
        .withUrl(`${config.serverUrl}/humanservice?identityId=${identityId}`)
        .build();

    connection.start()
        .then(() => {
            connection.invoke('NewCustomer', { identityId, name });
            Connected && Connected();
        })
        .catch(err => {
            Error && Error(err);
        });

    connection.on('startService', args => {
        server = { ...args }
        console.log(server);
        Started && Started();
    })

    connection.on('receiveMessage', message => {
        Receive && Receive(message);
    })

    connection.on('serviceScore', recordId => {
        Hangup && Hangup(recordId);
    })

    connection.on('waitService', () => {
        Waiting && Waiting();
    })

    connection.on('cancelWaiting', () => {
        Cancel && Cancel();
    })
}

const signalrService = {
    connect: connectHub,
    disconnect: () => {
        return connection.stop();
    },
    send: (message, True, False) => {
        const { userid } = _ddUser;
        connection.invoke("SendMessage", { ...message, groupName: server.groupName, sender: userid })
            .then(() => {
                True && True();
            }).catch(e => {
                False && False(e);
            });
    },
    hangup: (waiting = false) => {
        return connection.invoke('Hangup', waiting);
    }
}


export default signalrService;