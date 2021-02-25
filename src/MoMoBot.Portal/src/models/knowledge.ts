import { fetchKnowledges, updateKnowledge, deleteKnowledge, addKnowledge, fetchDialogFlows } from '../services/knowledge.service';
import { search } from '../services/intent.service';

const fetchData = async (payload: any) => {
    const response = await fetchKnowledges({ ...payload });
    const { rows, total, pageSize, pageIndex } = response;
    const data = {
        list: rows,
        pagination: {
            total: total,
            pageSize,
            current: pageIndex || 1,
        }
    }
    return data;
}

export default {
    namespace: 'knowledge',
    state: {
        intents: [],
        open: false,
        data: {
            list: [],
            pagination: {}
        },
        flows: []
    },
    effects: {
        *open(_: any, { put }: { put: any }) {
            yield put({
                type: 'save',
                payload: {
                    open: true
                }
            })
        },
        *close(_: any, { put }: { put: any }) {
            yield put({
                type: 'save',
                payload: {
                    open: false
                }
            })
        },
        *fetch({ payload }: { payload: any }, { call, put }: { call: any, put: any }) {
            const data = yield call(fetchData, { ...payload });
            yield put({
                'type': 'save',
                payload: {
                    data
                }
            });
        },
        *update({ payload }: { payload: any }, { call, put }: { call: any, put: any }) {
            yield call(updateKnowledge, payload);
            const data = yield call(fetchData, {});
            yield put({
                'type': 'save',
                payload: {
                    data,
                    open: false
                }
            });
        },
        *delete({ payload }: { payload: any }, { call, put }: { call: any, put: any }) {
            yield call(deleteKnowledge, payload);
            const data = yield call(fetchData, {});
            yield put({
                'type': 'save',
                payload: {
                    data
                }
            });
        },
        *add({ payload }: { payload: any }, { call, put }: { call: any, put: any }) {
            yield call(addKnowledge, payload);
            const data = yield call(fetchData, {});
            yield put({
                'type': 'save',
                payload: {
                    data
                }
            });
        },
        *searchIntents({ payload }: { payload: any }, { call, put }: { call: any, put: any }) {

            let intents: any = [];
            if (payload) {
                var response = yield call(search, payload);
                console.log(response);
                intents = response.map((item: any) => item.name);
            }
            yield put({
                type: 'save',
                payload: {
                    intents
                }
            })
        },
        *fetchDialogFlows(_: any, { put, call }: { put: any, call: any }) {
            const flows = yield call(fetchDialogFlows);
            console.log(flows);
            yield put({
                type: 'save',
                payload: {
                    flows
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
        },
        resetIntents(state: any) {
            return {
                ...state,
                intents: []
            }
        }
    }
}