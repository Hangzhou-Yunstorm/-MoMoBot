import { fetchUnknowns } from '../services/knowledge.service';
export default {
    namespace: 'unknown',
    state: {
        data: [],
        total: 0
    },
    effects: {
        *fetchUnknowns({ payload }: { payload: any }, { put, call }: { put: any, call: any }) {
            const response = yield call(fetchUnknowns, payload);

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