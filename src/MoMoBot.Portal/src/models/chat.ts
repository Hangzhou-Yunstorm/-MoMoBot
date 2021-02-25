import IChatState from '../@types/IChatState';
import { getContacts, getRecords, getProfile, getServiceRecords } from '../services/chat.service';
import IChatContact from '../@types/IChatContact';
import { notification } from 'antd';

const initialState: IChatState = {
    contacts: [],
    unreadCount: 0,
    activeRecord: undefined
};

const count = 10;

export default {
    namespace: 'chat',
    state: {
        ...initialState
    },
    effects: {
        *fetchUnreadCount(_: any, { call, put, select }: { call: any, put: any, select: any }) {
            const unreadCount = count;
            yield put({
                type: 'save',
                payload: {
                    unreadCount
                },
            })
        },
        *fetchContacts({ payload }: { payload: number }, { call, put, select }: { call: any, put: any, select: any }) {
            // const feedbackStatistical = yield call();
            const contacts = yield call(getContacts);
            let current = null;
            if (payload > 0 && contacts && contacts.length && contacts.length > 0) {
                current = contacts[0];

                contacts.forEach((contact: any) => {
                    if (contact.id === payload) {
                        current = contact;
                    }
                });

                const records = yield call(getRecords, { chatId: current.id });
                const profile = yield call(getProfile, current.uid);
                //const unreadCount = yield select((state: any) => state.chat.contacts.filter((contact: any) => contact.unread).length);

                yield put({
                    type: 'save',
                    payload: {
                        contacts,
                        activeRecord: current,
                        records,
                        profile
                    },
                })
            }
            yield put({
                type: 'save',
                payload: {
                    contacts
                },
            })
        },
        *changeCurrent({ payload }: { payload: any }, { call, put, select }: { call: any, put: any, select: any }) {
            const { chatId } = payload;
            const current = yield select((state: any) => {
                const { chat: { contacts } } = state;

                if (contacts && contacts.length) {
                    let filter = contacts.filter((i: any) => i.id === chatId);
                    if (filter.length > 0) {
                        return filter[0];
                    }
                }
                return;
            });
            yield put({
                type: 'save',
                payload: {
                    activeRecord: current
                }
            })
        },
        *fetchChatRecords({ payload }: { payload: any }, { call, put, select }: { call: any, put: any, select: any }) {
            const records = yield call(getRecords, payload);

            yield put({
                type: 'save',
                payload: {
                    records
                },
            });

        },
        *changeRecordReadState({ payload }: { payload: any }, { put, select }: { put: any, select: any }) {
            const records = yield select((state: any) => {
                const { chat: { contacts } } = state;
                let result = [];
                if (contacts && contacts.length) {
                    result = contacts.map((item: any) => {
                        const contact = { ...item };
                        if (item.id === payload && contact.unread) {
                            contact.unread = false;
                        }
                        return contact;
                    });
                }
                return result;
            });
            //const unreadCount = yield select((state: any) => state.chat.contacts.filter((contact: any) => contact.unread).length);
            yield put({
                type: 'save',
                payload: {
                    contacts: records,
                    //unreadCount
                },
            });
        },
        *newMessage({ payload }: { payload: any }, { call, put, select }: { call: any, put: any, select: any }) {
            console.log(payload);
            const { groupName, content } = payload;

            let current = yield select((state: any) => state.chat.activeRecord);
            let messages = undefined;
            if (current && current.groupName && current.groupName === groupName) {
                messages = yield select((state: any) => {
                    const { chat: { records } } = state;
                    let { rows } = records;
                    if (!rows) {
                        rows = [];
                    }
                    return {
                        ...records,
                        rows: rows.concat({ ...payload })
                    };
                });
            } else {
                // 通知
                notification.open({
                    message: '人工服务通知',
                    description: `[新消息] ${content}`,
                    type: 'info',
                    duration: 2
                });
            }

            let _contacts = yield select((state: any) => {
                const { chat: { contacts } } = state
                if (contacts && contacts.length && contacts.length > 0) {
                    return contacts.map((contact: IChatContact<number>) => {
                        if (contact.groupName === groupName) {
                            contact.message = content;
                        }
                        return contact;
                    })
                }
                return contacts;
            });
            const data: any = {};
            data.contacts = _contacts;
            messages && (data.records = messages);
            yield put({
                type: 'save',
                payload: { ...data }
            })
        },
        *newCustomer({ payload }: { payload: any }, { call, put, select }: { call: any, put: any, select: any }) {
            const { chatRoom } = payload;
            let i = 0;
            let __contacts = yield select((state: any) => {
                const { chat } = state;
                const { contacts } = chat;
                if (contacts && contacts.length) {

                    return contacts.map((contact: IChatContact<number>) => {
                        if (contact.groupName === chatRoom) {
                            contact.online = true;
                        } else {
                            i++;
                        }
                        return contact;
                    });
                }
                return [];
            });
            if (i >= __contacts.length) {
                __contacts = yield call(getContacts);
            }
            console.log(__contacts);
            yield put({
                type: 'save',
                payload: {
                    contacts: __contacts
                }
            })
        },
        *customerOffline({ payload }: { payload: any }, { put, select }: { put: any, select: any }) {
            const __contacts = yield select((state: any) => {
                const { chat: { contacts } } = state;
                if (contacts && contacts.length) {
                    return contacts.map((contact: IChatContact<number>) => {
                        if (contact.groupName === payload) {
                            contact.online = false;
                        }
                        return contact;
                    })
                }
                return contacts;
            });
            yield put({
                type: 'save',
                payload: {
                    contacts: __contacts
                }
            })
        },
        *fetchProfile({ payload }: { payload: any }, { call, put, select }: { call: any, put: any, select: any }) {
            const current = yield select((state: any) => {
                const { chat: { contacts } } = state;

                if (contacts && contacts.length) {
                    let filter = contacts.filter((i: any) => i.id === payload);
                    if (filter.length > 0) {
                        return filter[0];
                    }
                }
                return {};
            });
            const profile = yield call(getProfile, current.uid);
            yield put({
                type: 'save',
                payload: {
                    profile
                }
            })
        },
        *fetchServiceRecords({ payload }: { payload: any }, { call, put, select }: { call: any, put: any, select: any }) {
            const current = yield select((state: any) => {
                const { chat: { contacts } } = state;

                if (contacts && contacts.length) {
                    let filter = contacts.filter((i: any) => i.id === payload);
                    if (filter.length > 0) {
                        return filter[0];
                    }
                }
                return {};
            });
            const serviceRecords = yield call(getServiceRecords, current.uid);
            yield put({
                type: 'save',
                payload: {
                    serviceRecords
                }
            })
        }
    },
    reducers: {
        save(state: IChatState, { payload }: { payload: any }) {
            return {
                ...state,
                ...payload
            }
        },
        clearActive(state: IChatState) {
            return {
                ...state,
                activeRecord: undefined,
                records: [],
                profile: {}
            }
        },
        clear() {
            return {
                ...initialState
            }
        }
    }
}