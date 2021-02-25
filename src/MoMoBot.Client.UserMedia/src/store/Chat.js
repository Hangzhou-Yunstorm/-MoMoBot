import { MessageTypes, MessageStatus } from 'react-yc-chatbox';
import axios from 'axios';
import messageService from '../services/messageService';
import applicationService from '../services/applicationService';
import React from 'react';
import config from '../config.js'
import ddService from '../services/ddService';
import { sessionStoreService } from '../services/storageService';
import storeKeys from '../types/storeKeys'

const receiveMessageType = 'ReceiveMessage';
const requestHistoryType = 'RequestHistoryMessages';
const loadingData = 'LoadingData';
const removeMessage = 'RemoveMessage';
const messageSent = 'MessageSent';
const sendMessage = 'SendMessage';
const saveDDUserInfo = 'SaveDDUserInfo';
const play = "playVoice";

const initialState = {
    messages: [],
    loading: false,
    ddUser: undefined
};
/**标识是否是在多轮查询的对话环境 */
let multipleRoundConversationStarted = false;
/**标识是否是在申请提报的对话环境 */
let applicationStarted = false;
/**发生错误的回复内容 */
const errorMessage = messageService.createMomoMessage('网络连接失败，请检查您的网络连接！');
const anonymousMessage = messageService.createMomoMessage('不好意思，小摩摩未能认出您，所以不能为您服务。');


function requestAnswer(question, ddUserId) {
    return axios({
        url: `${config.serverUrl}/api/message?question=${question}`,
        method: 'post',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'x-dd-userid': ddUserId || ''
        }
    });
}

function requestTable(id, listName, colummName) {
    let url = `${config.serverUrl}/api/message/final?listName=${listName}&id=${id}&colummName=${colummName}`;
    return axios({
        url: url,
        method: 'post'
    });
}

function exitQuery() {
    multipleRoundConversationStarted = false;
}

function startTravelApplication(dispatch, question) {
    setTimeout(() => {
        const msg = applicationService.travelApplication(question, data => {
            applicationStarted = false;
        });

        msg && dispatch({ type: receiveMessageType, message: { ...msg } });
    }, 500);
}



export const actionCreators = {
    send: (message, itemSelected) => async (dispatch) => {
        const ddUser = ddService.getDDUserInfo();
        const { userid } = ddUser;

        dispatch({ type: sendMessage, message });
        const question = message.type === MessageTypes.Text ? message.content : message.voice.text;
        if (multipleRoundConversationStarted) {
            const { listName, id, title } = message.data;
            let colummName = message.content !== title ? message.content : '';
            requestTable(id, listName, colummName)
                .then(response => {
                    dispatch({ type: messageSent, id: message.id, result: true });
                    const { data } = response;
                    const header = `${title} ${colummName}`;
                    let table = (<span>没有找到关于“<i className="blue-text">{header}</i>”的相关内容。</span>);
                    if (data.length && data.length > 0) {
                        table = messageService.renderTable(data, header, exitQuery);
                    }

                    let tableMessage = messageService.createMomoMessage(table, true);
                    dispatch({ type: receiveMessageType, message: { ...tableMessage } });
                }).catch(error => {
                    dispatch({ type: messageSent, id: message.id, result: true });
                    dispatch({ type: receiveMessageType, message: errorMessage });
                })
        } else if (applicationStarted) {
            dispatch({ type: messageSent, id: message.id, result: true });
            startTravelApplication(dispatch, question);
        } else {
            requestAnswer(question, userid)
                .then(response => {
                    const { data } = response;
                    const feedbackData = { ...data.data, answer: data.message };

                    if (data.success) {
                        // 商务模块
                        if (data.data.intent === 'None') {
                            messageService.businessInquiry(message, userid, (ul, noresult) => {
                                let content = ul;
                                if (noresult) {
                                    content = data.message
                                }
                                dispatch({ type: messageSent, id: message.id, result: true });
                                dispatch({ type: receiveMessageType, message: messageService.createMomoMessage(content, !noresult, noresult, feedbackData) });
                            }, item => {
                                multipleRoundConversationStarted = true;
                                if (typeof itemSelected === 'function') {
                                    itemSelected(item);
                                }
                            });
                            return;
                        }
                        // 出差申请
                        else if (data.data.intent === '出差申请') {
                            if (userid) {
                                const tipMessage = messageService.createNoticeMessage('终止申请发送“退出”');
                                dispatch({ type: receiveMessageType, message: tipMessage });
                                applicationStarted = true;
                                startTravelApplication(dispatch, question);
                            } else {
                                dispatch({ type: receiveMessageType, message: anonymousMessage });
                            }
                        } else {

                            const msg = messageService.createMomoMessage(data.message, false, true, feedbackData);
                            dispatch({ type: receiveMessageType, message: { ...msg } });
                        }
                    }
                    dispatch({ type: messageSent, id: message.id, result: true });
                }).catch((error) => {
                    dispatch({ type: messageSent, id: message.id, result: false });
                    dispatch({ type: receiveMessageType, message: errorMessage });
                });
        }

    },
    append: (message) => async (dispatch) => {
        dispatch({ type: sendMessage, message });
    },
    receive: (message) => async (dispatch, getState) => {
        dispatch({ type: receiveMessageType, message });
    },
    sent: (id, result = true) => async (dispatch) => {
        dispatch({ type: messageSent, id, result });
    },
    remove: (id) => async (dispatch) => {
        if (id === '') {
            return;
        }
        dispatch({ type: removeMessage, id });
    },
    requestHistoryMesssages: () => async (dispatch) => {
        dispatch({ type: loadingData, loading: true });
        setTimeout(() => {
            let messages = [];
            dispatch({ type: requestHistoryType, messages });
        }, 1000);
    },
    saveDDUserInfo: (userInfo) => async (dispatch) => {
        sessionStoreService.store(storeKeys.DDUSERINFO, userInfo);
        const { name } = userInfo;
        const userInfoMessage = messageService.createMomoMessage(`${name}你好，我是小摩摩，很高兴为您服务！`);
        dispatch({ type: saveDDUserInfo, userInfo })
        dispatch({ type: receiveMessageType, message: userInfoMessage })
    },
    playStart: (id) => async (dispatch) => {
        dispatch({ type: play, payload: { id, playing: true } })
    },
    playEnd: (id) => async (dispatch) => {
        dispatch({ type: play, payload: { id, playing: false } })
    }
};

export const reducer = (state, action) => {
    state = state || initialState;

    if (action.type === saveDDUserInfo) {
        return {
            ...state,
            ddUser: action.userInfo
        };
    }

    else if (action.type === sendMessage) {
        return {
            ...state,
            messages: state.messages.concat(action.message)
        };
    }

    else if (action.type === receiveMessageType) {
        return {
            ...state,
            messages: state.messages.concat(action.message)
        };
    }

    else if (action.type === requestHistoryType) {
        return {
            ...state,
            messages: action.messages,
            loading: false
        }
    }

    else if (action.type === loadingData) {
        return {
            ...state,
            loading: action.loading
        }
    }

    else if (action.type === removeMessage) {
        const messages = state.messages.filter(message => message.id !== action.id);
        return ({
            ...state,
            messages
        })
    }

    else if (action.type === messageSent) {
        const messages = state.messages.map(message => {
            if (message.id && message.id === action.id) {
                message.status = action.result ? MessageStatus.Success : MessageStatus.Fail;
            }
            return message;
        });
        return ({
            ...state,
            messages
        })
    }

    else if (action.type === play) {
        const { id, playing } = action.payload;
        const messages = state.messages.map(message => {
            if (message.id && message.id === id) {
                message.playing = playing;
            }
            return message;
        })
        console.log(messages);
        return ({
            ...state,
            messages
        })
    }



    return state;
};
