import * as dd from 'dingtalk-jsapi';
import axios from 'axios';
import { sessionStoreService } from './storageService';
import storeKeys from '../types/storeKeys';
import config from '../config';

const ddService = {
    getUserInfo: (corpId, token, callback) => {
        // 免登获取钉钉用户信息
        dd.runtime.permission.requestAuthCode({
            corpId: corpId,
            onSuccess: (result) => {
                const { code } = result;
                axios({
                    method: 'get',
                    url: `${config.serverUrl}/api/info/user?code=${code}&token=${token}`,
                }).then((response) => {
                    if (response.data && response.data.code === 0) {
                        const { data } = response.data;
                        // const data = { "errcode": "0", "errmsg": "ok", "userid": "06011309172371", "name": "唐振炀", "tel": "", "workPlace": "", "remark": "", "mobile": "15868124027", "email": "tang.zhenyang@yunstorm.com", "orgEmail": null, "active": true, "orderInDepts": "{ 10835173: 32144504476655344 } ", "isAdmin": true, "isBoss": false, "dingId": "$: LWCP_v1: $o6WdBge7qLNZgN7krAP81g == ", "unionid": "BEfEWOGyiPHkE3aLvn2iSKiigiEiE", "isLeaderInDepts": "{ 10835173: false } ", "isHide": false, "department": ["10835173"], "position": "研发工程师", "avatar": "", "hiredDate": "1457020800000", "jobnumber": "", "extattr": null, "stateCode": "86", "isSenior": "false" }
                        callback(data);
                    }
                }).catch(error => {
                    alert(error);
                })
            },
            onFail: (err) => {
                alert(JSON.stringify(err))
            }
        });
    },
    getDDUserInfo: () => {
        return sessionStoreService.retrieve(storeKeys.DDUSERINFO, config.ddUserDefaultValue);
    }
}

export default ddService;