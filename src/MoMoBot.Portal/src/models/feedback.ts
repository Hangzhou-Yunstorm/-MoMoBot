import IFeedbackState from '../@types/IFeedbackState';
import { fetchFeedbackStatistical, fetchPopularIntents, fetchPraiseFeedback, fetchBadFeedback } from '../services/feedback.service';
import ITwoDData from 'src/@types/ITwoDData';

const initialState: IFeedbackState = {
    feedbackStatistical: [],
    popularIntents: [],
    praiseFeedback: [],
    badFeedback: []
};

const responseDataConvertTo2DData = (data: any) => {
    let result: ITwoDData[] = [];
    if (data && data.length) {
        data.forEach((item: any) => {
            result.push({ x: item.intent, y: item.count })
        })
    }
    return result;
}

export default {
    namespace: 'feedback',
    state: {
        ...initialState
    },
    effects: {
        *fetchFeedbackStatistical(_: any, { call, put }: { call: any, put: any }) {
            const feedbackStatistical = yield call(fetchFeedbackStatistical);
            yield put({
                type: 'save',
                payload: {
                    feedbackStatistical
                },
            })
        },
        *fetchPopularIntents(_: any, { call, put }: { call: any, put: any }) {
            const response = yield call(fetchPopularIntents);
            const popularIntents = responseDataConvertTo2DData(response);
            yield put({
                type: 'save',
                payload: {
                    popularIntents
                },
            })
        },
        *fetchPraiseFeedback(_: any, { call, put }: { call: any, put: any }) {
            const response = yield call(fetchPraiseFeedback);
            const praiseFeedback = responseDataConvertTo2DData(response);
            yield put({
                type: 'save',
                payload: {
                    praiseFeedback
                },
            })
        },
        *fetchBadFeedback(_: any, { call, put }: { call: any, put: any }) {
            const response = yield call(fetchBadFeedback);
            const badFeedback = responseDataConvertTo2DData(response);
            yield put({
                type: 'save',
                payload: {
                    badFeedback
                },
            })
        }
    },
    reducers: {
        save(state: IFeedbackState, { payload }: { payload: any }) {
            return {
                ...state,
                ...payload
            }
        },
        clear() {
            return {
                ...initialState
            }
        }
    }
}