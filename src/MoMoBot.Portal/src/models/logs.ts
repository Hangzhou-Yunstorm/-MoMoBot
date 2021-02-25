import { fetchLogs } from '../services/logs.service';
export default {
    namespace: 'logs',
    state: {
        data: [],
        total: 0
    },
    effects: {
        *fetch({ payload }: { payload: any }, { call, put }: { call: any, put: any }) {
            const response = yield call(fetchLogs, payload);
            yield put({
                type: 'save',
                payload: {
                    ...response
                }
            })
        }
    },
    reducers: {
        save(state: any, { payload }: { payload: any }) {
            return {
                ...state,
                ...payload
            }
        }
    }
}