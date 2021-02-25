import { MessageRoles, MessageTypes, guid } from 'react-yc-chatbox';
import axios from 'axios';
import React from 'react';
import $ from 'jquery';
import config from '../config.js'
import Evaluation from '../components/Evaluation.js';

const messageService = {
    businessInquiry: (message, ddUserId, callback, selectedItem) => {
        axios({
            url: `${config.serverUrl}/api/message/multiple?title=${message.content}`,
            method: 'post',
            headers: {
                'x-dd-userid': ddUserId
            }
        }).then(response => {
            const { data } = response;
            let noResult = true;
            let ul = (<span>不好意思，我没有明白你的意思。</span>);
            if (data.length && data.length > 0) {
                noResult = false;
                let list = data.map((item, index) =>
                    (<li className="item" onClick={() => { selectedItem(item) }} key={index}>{item.title}<span>（{item.sheetName}）</span></li>)
                );
                ul = (<ul className="list">{list}</ul>);
            }

            callback(ul, noResult);
        })
    },
    renderTable: (data, header, exitCallback) => {
        let trs = data.map((item, index) => (
            <tr key={index} style={{ display: index > 5 ? 'none' : 'table-row', transition: 'all ease-in .2s' }}>
                <td>{item.tName}</td>
                <td>{item.tValue}</td>
            </tr>
        ));
        let table = (
            <table className="table">
                <thead><tr><td colSpan={2}>以下是关于“<i className="blue-text">{header}</i>”的相关内容，请查收：</td></tr></thead>
                <tbody>{trs}</tbody>
                <tfoot>
                    <tr className="table-footer">
                        <td>{data.length > 6 && <button onClick={(e) => {
                            const $this = $(e.target);
                            if ($this.text() === '展开') {
                                $this.text('收起');
                                $this.parents('.table').find('tbody tr').show();
                            } else {
                                $this.text('展开');
                                $this.parents('.table').find('tbody tr:gt(5)').hide();
                            }

                            return false;
                        }}>展开</button>}</td>
                        <td align="right"><button className="exitQuery" onClick={() => {
                            $('.exitQuery').remove();
                            if (typeof exitCallback === 'function') {
                                exitCallback();
                            }
                        }}>退出查询</button></td>
                    </tr>
                </tfoot>
            </table>
        );

        return table;
    },
    createMomoMessage(content, isHtmlMessage = false, feedback = false, feedbackData = {}) {
        return { name: '小摩摩', id: guid(), content, feedback, feedbackData, type: isHtmlMessage ? MessageTypes.Html : MessageTypes.Text, role: MessageRoles.Other, datetime: new Date() }
    },
    createNoticeMessage: (content) => ({ id: guid(), content, type: MessageTypes.Notice, role: MessageRoles.System, datetime: new Date() }),
    /**人工服务结束后的评分面板 */
    createEvaluationMessage(submitCallback) {
        const evaluation = (<Evaluation onSubmitCallback={submitCallback} />);

        return { name: '小摩摩', id: guid(), content: evaluation, type: MessageTypes.Html, role: MessageRoles.Other, datetime: new Date() }
    },
    feedback(message, result) {
        const { feedbackData } = message;
        console.log(feedbackData);
        axios({
            url: `${config.serverUrl}/api/message/feedback`,
            method: 'post',
            headers: {
                'Content-Type': 'application/json'
            },
            transformRequest: [function (data) {
                data = JSON.stringify(data)
                return data
            }],
            data: {
                ...feedbackData,
                result
            }
        }).catch((e) => {
            console.error(e);
        })
    }
}

export default messageService;