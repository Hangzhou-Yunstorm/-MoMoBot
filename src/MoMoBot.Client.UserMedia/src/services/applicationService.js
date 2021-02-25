import React from 'react';
import axios from 'axios';
import config from '../config.js'
import { TravelApplication } from '../types/applications';
import messageService from './messageService';
import { MessageRoles, MessageTypes, guid } from 'react-yc-chatbox';
import ApplicationResultTable from '../components/ApplicationResultTable';
import ddService from './ddService.js';

let currentApplicationIndex = 0;
let applicationData = [];

const endApplication = (callback) => {
    if (typeof callback === 'function') {
        callback(applicationData);
    }
    applicationData = [];
    currentApplicationIndex = 0;
}

const checkApplicationData = (step, value) => {
    let result = false;
    if (step.isRequired) {
        result = value !== '';
        if (step.type === 'datetime') {
            result = isNaN(value) && !isNaN(Date.parse(value));
        } else if (step.type === 'boolean') {
            result = true;
        }
    } else {
        result = true;
    }

    return result;
}

const submitApplicationData = (data, callback) => {
    // add userid and departid
    const { userid, department } = ddService.getDDUserInfo();
    axios({
        url: `${config.serverUrl}/api/message/travelapproval`,
        method: 'post',
        headers: {
            'Content-Type': 'application/json'
        },
        transformRequest: [function (data) {
            let parames = {};
            let length = data.length;
            if (length) {
                for (var i = 0; i < length; i++) {
                    let item = data[i];
                    parames[item.name] = item.value;
                }
            }
            parames['userid'] = userid;
            parames['departId'] = department[0] || 0;
            data = JSON.stringify(parames)
            return data
        }],
        data: data
    }).then(response => {
        callback(true);
    }).catch((e) => {
        
        callback(false, e);
    })
}

const applicationService = {
    travelApplication: (answer, endCallback) => {
        if (answer === '退出') {
            endApplication(endCallback);
            return messageService.createNoticeMessage('已退出出差申请提报');
        }
        let result = false;
        if (currentApplicationIndex < 0 ||
            currentApplicationIndex > TravelApplication.length) {
            return;
        }
        // 给上一步骤赋值
        if (currentApplicationIndex > 0) {
            let preStep = TravelApplication[currentApplicationIndex - 1];
            // 数据校验
            result = checkApplicationData(preStep, answer);
            if (result) {
                applicationData.push({ name: preStep.name, value: answer, text: preStep.text });
            }

        }
        // 最后一级
        if (currentApplicationIndex === TravelApplication.length) {
            let applicationDataClone = applicationData;
            endApplication(endCallback);
            // todo ：发送申请
            return {
                name: '小摩摩', id: guid(), content: <div>您的出差申请内容如下：<ApplicationResultTable data={applicationDataClone} onSubmit={submitApplicationData} /></div>, type: MessageTypes.Html, role: MessageRoles.Other, datetime: new Date()
            }
        }
        if (!result && currentApplicationIndex > 0) {
            currentApplicationIndex--;
        }
        let step = TravelApplication[currentApplicationIndex];
        let content = step.question;
        currentApplicationIndex++;
        return { name: '小摩摩', id: guid(), content, type: MessageTypes.Html, role: MessageRoles.Other, datetime: new Date() }
    },
}

export default applicationService;