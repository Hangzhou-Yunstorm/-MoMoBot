import { getUsers } from '../services/user.service';
export default {
    namespace: 'user',
    state: {
        users: []
    },
    effects: {
        *fetchUsers({ payload }: { payload: any }, { call, put }: { call: any, put: any }) {
            const response = yield call(getUsers, payload);
            yield put({
                type: 'save',
                payload: {
                    users: response.data
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