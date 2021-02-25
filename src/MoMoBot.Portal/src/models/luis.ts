import { search, examples, createIntent } from '../services/intent.service';

import 'moment/locale/zh-cn';

export default {
    namespace: 'luis',
    state: {
        alert: { msg: undefined, type: '' },
        intents: [],
        detail: {},
        pagination: {},
    },
    effects: {
        *fetchIntents({ payload }: { payload: any }, { call, put }: { call: any, put: any }) {
            const response = yield call(search, payload);

            yield put({
                type: 'save',
                payload: {
                    intents: response
                }
            })
        },
        *fetchExamples({ payload }: { payload: any }, { call, put }: { call: any, put: any }) {
            const response = yield call(examples, payload);

            yield put({
                type: 'save',
                payload: {
                    detail: response
                }
            })
        },
        *addIntent({ payload }: { payload: any }, { call, put }: { call: any, put: any }) {
            yield call(createIntent, payload);
            const response = yield call(search, '');
            yield put({
                type: 'save',
                payload: {
                    intents: response
                }
            })
        },
        *alert({ payload }: { payload: any }, { call, put }: { call: any, put: any }) {
            yield put({
                type: 'save',
                payload: {
                    alert: payload
                }
            })
            
        }
        // *publish(_: any, { call, put }: { call: any, put: any }) {
        //     const response = yield call(publish);
        // }
    },
    reducers: {
        save(state: any, { payload }: { payload: any }) {
            return {
                ...state,
                ...payload
            }
        },
        closeAlert(state: any) {
            return {
                ...state,
                alert: {}
            }
        }
    }
}