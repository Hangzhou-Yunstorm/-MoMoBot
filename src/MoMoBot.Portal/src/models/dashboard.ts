import * as moment from 'moment';
import { getEndpointHitHistory, getSatsMetadata, getAppStatusInfo, getLuisStats } from '../services/luis.service';

const getXValue = (date: string, days: number) => {
    if (days === 7) {
        return moment(date).format('dddd')
    }
    return moment(date).format('l')
}

export default {
    namespace: 'dashboard',
    state: {
        metadata: {},
        errorIntents: [],
        activeMetadata: {},
        statusInfo: {},
        endpointhitshistories: [],
        errorDetail: {}
    },
    effects: {
        *fetchEndpointHitHistory({ payload }: { payload: any }, { call, put }: { call: any, put: any }) {
            const response = yield call(getEndpointHitHistory, payload);
            const data = Object.keys(response).map((key: string) => ({ x: getXValue(key, payload), y: response[key] }));

            yield put({
                type: 'save',
                payload: {
                    endpointhitshistories: data
                }
            })
        },
        *fetchLuisAppStatus(_: any, { call, put }: { call: any, put: any }) {
            const statusInfo = yield call(getAppStatusInfo);
            yield put({
                type: "save",
                payload: {
                    statusInfo
                }
            })
        },
        *fetchStatsMetadata(_: any, { call, put }: { call: any, put: any }) {
            const metadata = yield call(getSatsMetadata);
            const { modelsMetadata = [] } = metadata;
            var payload = {
                metadata,
                activeIntent: {},
                errorIntents: [],
                errorDetail: {}
            };
            var errorIntents = modelsMetadata.filter((item: any) => item.utterancesCount <= 0 || item.misclassifiedUtterancesCount > 0 || item.ambiguousUtterancesCount > 0 || item.misclassifiedAmbiguousUtterancesCount > 0) || [];
            var activeIntent = {};
            var errorDetail = {};
            if (errorIntents.length > 0) {
                activeIntent = errorIntents[0];
                errorDetail = yield call(getLuisStats, errorIntents[0].modelId || '');
            }

            payload = {
                ...payload,
                activeIntent,
                errorIntents,
                errorDetail
            }

            console.log(payload);
            yield put({
                type: "save",
                payload
            })
        },
        *changeActiveIntent({ payload }: { payload: any }, { call, select, put }: { call: any, select: any, put: any }) {
            const active = yield select((state: any) => {
                const result = state.dashboard.errorIntents.filter((e: any) => e.modelId === payload);
                if (result.length) {
                    return result[0]
                }
                return undefined;
            });
            console.log(active);
           
            var errorDetail = {};
            yield put({
                type: 'save',
                payload: {
                    activeIntent: active,
                    errorDetail
                }
            });

            if (active && active.modelId) {
                errorDetail = yield call(getLuisStats, active.modelId || '');
            }
            yield put({
                type: 'save',
                payload: {
                    errorDetail
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