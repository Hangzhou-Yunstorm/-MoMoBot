import { fetchFlows } from '../services/flow.service';

export default {
    namespace: 'flow',
    state: {
        commands: [],
        flows: [],
        editModel: undefined
    },
    effects: {
        *fetchFlows(_: any, { call, put }: { call: any, put: any }) {
            const response = yield call(fetchFlows);
            const { data: flows = [] } = response;
            yield put({
                type: 'save',
                payload: {
                    flows
                }
            })
        },
        *storeCommands({ payload }: { payload: any }, { put }: { put: any }) {
            console.log(payload);
            yield put({
                type: 'save',
                payload: {
                    commands: payload
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