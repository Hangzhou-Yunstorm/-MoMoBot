import request from '../utils/request';
import settings from '../settings';

export default {
    namespace: 'department',
    state: {
        departments: []
    },
    effects: {
        *fetchDepartments({ payload }: { payload: any }, { call, put }: { call: any, put: any }) {
            const fetch = () => request(`${settings.serverUrl}/api/department/tree`, {
                method: 'get'
            })
            const response = yield call(fetch, payload);

            yield put({
                type: 'save',
                payload: {
                    departments: response
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