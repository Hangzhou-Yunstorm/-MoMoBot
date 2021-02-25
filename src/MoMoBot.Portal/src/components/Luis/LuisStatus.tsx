import * as React from 'react';
import * as moment from 'moment';
const styles = require('./LuisStatus.less');

export default class LuisStatus extends React.PureComponent<any> {
    render() {
        const { info = {} } = this.props;
        const { endpoints = {} } = info;

        let first = '';
        let endpoint: any = {};
        for (var key in endpoints) {
            first = key;
            break;
        }
        endpoint = endpoints[first] || {};
        return (
            <div className={styles.card}>
                <div className={styles.item}>
                    <h1 className={styles.name}>{info.name}</h1>
                </div>
                <div className={styles.item}>
                    <label className={styles.label}>创建时间：</label>
                    <p>{moment(info.createdDateTime).format('lll')}</p>
                </div>
                <div className={styles.item}>
                    <label className={styles.label}>最后发布时间：</label>
                    <p>{moment(info.publishedDateTime).format('lll')}</p>
                </div>
                <div className={styles.item}>
                    <label className={styles.label}>版本：</label>
                    <span>{endpoint.versionId}</span>
                </div>
                <div className={styles.item}>
                    <label className={styles.label}>终结点：</label>
                    <span>{first}</span>
                </div>
                <div className={styles.item}>
                    <label className={styles.label}>区域：</label>
                    <p>{endpoint.endpointRegion}</p>
                </div>
                <div className={styles.item}>
                    前往 <a href="http://www.luis.ai" target="_blank">luis.ai</a> 管理应用
                </div>
            </div >
        )
    }
}