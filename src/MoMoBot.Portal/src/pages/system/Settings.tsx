import * as React from 'react';
import SettingsForm from '../../components/System/SettingsForm';
import { connect } from 'dva';
import { Card, Skeleton } from 'antd';
import { default as config } from '../../settings';

@connect(({ setting, loading }: { setting: any, loading: any }) => ({
    loading: loading.effects['setting/fetchAllSettings'],
    settings: setting.settings
}))
export default class Settings extends React.PureComponent<any> {
    componentDidMount() {
        this.props.dispatch({ type: 'setting/fetchAllSettings' })
    }

    updateSetting(keyvalue: any) {
        this.props.dispatch({
            type: 'setting/updateSetting',
            payload: { ...keyvalue }
        })
    }

    render() {
        const { settings, loading } = this.props;
        return (
            <Skeleton loading={loading} active={true} title={false} paragraph={{ rows: 5, width: ['100%', '100%', '100%', '100%', '100%'] }}>
                <SettingsForm values={...settings}
                    onUpdate={this.updateSetting.bind(this)} />
                <Card bordered={false} title="其它设置">
                    <a href={`${config.serverUrl}/_hangfire`} target="_blank">任务调度</a>
                </Card>
            </Skeleton>

        )
    }
}