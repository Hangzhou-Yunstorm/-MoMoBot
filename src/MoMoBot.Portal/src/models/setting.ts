import request from '../utils/request';
import settings from '../settings';

export default {
    namespace: 'setting',
    state: {
        settings: {
            intent_minimum_matching_degree: 0,
            default_user_avatar: '',
            default_user_password: ''
        }
    },
    effects: {
        *fetchAllSettings(_: any, { call, put }: { call: any, put: any }) {
            const fetchSettings = () => request(`${settings.serverUrl}/api/settings/all-values`, {
                mehtod: 'GET'
            });
            const response = yield call(fetchSettings);
            if (response && response.length) {
                var keyvalue = {};
                for (var i = 0; i < response.length; i++) {
                    var item = response[i];
                    keyvalue[item.key] = item.value;
                }
                yield put({
                    type: 'save',
                    payload: {
                        settings: keyvalue
                    }
                })
            }

        },
        *updateSetting({ payload }: { payload: any }, { select, call, put }: { select: any, call: any, put: any }) {
            const update = () => request(`${settings.serverUrl}/api/settings/set`, {
                method: 'POST',
                body: {
                    ...payload
                }
            });
            yield call(update);
            const preSettings = yield select((state: any) => state.setting.settings);
            const { key, value } = payload;
            console.log(preSettings);
            preSettings[key] = value;
            // todo : save
            yield put({
                type: 'save',
                payload: {
                    ...preSettings,
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