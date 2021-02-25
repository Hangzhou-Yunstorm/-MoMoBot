import * as dd from 'dingtalk-jsapi';

const saveDDUserInfo = 'SaveDDUserInfo';

const initialState = {
    ddUser: {}
};

export const actionCreators = {
    ddIdentity: (corpid) => async (dispatch) => {
        dd.runtime.permission.requestAuthCode({
            corpId: corpid,
            onSuccess: function (result) {
                alert(JSON.stringify(result));
                dispatch({ type: saveDDUserInfo, result })
            },
            onFail: function (err) { }

        })
    }
};

export const reducer = (state, action) => {
    state = state || initialState;
    const { type } = action;
    if (type === 'saveDDUserInfo') {
        return ({
            state,
            ddUser: action.result
        })
    }

    return state;
};