import request from '../utils/request';
import settings from '../settings';

const server = settings.serverUrl;

/**获取用户反馈统计数据 */
export const fetchFeedbackStatistical = () => {
    return request(`${server}/api/statistics/feedback`, {
        method: 'get'
    });
}

/**获取热门意图统计数据 */
export const fetchPopularIntents = () => {
    return request(`${server}/api/statistics/popular-intents`, {
        method: 'get'
    });
}

/**获取好评TOP10的意图统计数据 */
export const fetchPraiseFeedback = () => {
    return request(`${server}/api/statistics/praisefeedback`, {
        method: 'get'
    });
}

/**获取差评TOP10的意图统计数据 */
export const fetchBadFeedback = () => {
    return request(`${server}/api/statistics/badfeedback`, {
        method: 'get'
    });
}