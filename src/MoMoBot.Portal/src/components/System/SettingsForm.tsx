import * as React from 'react';
import { Card, Form, Slider, Row, Col, Input } from 'antd';
import { SliderValue } from 'antd/lib/slider';
import { Debounce } from 'lodash-decorators/debounce';
import Password from 'antd/lib/input/Password';
const styles = require('./SettingsForm.less');

export interface ISettingValues {
    default_user_avatar: string;
    default_user_password: string;
    intent_minimum_matching_degree: string;
    business_inquiry_url: string
}

export interface ISettingsFormProps {
    values: ISettingValues;
    onUpdate?: (keyvalue: any) => void;
}

class SettingsForm extends React.Component<ISettingsFormProps, any>{
    constructor(props: ISettingsFormProps) {
        super(props);
        this.state = {
            minimumMatchingDegree: 0,
            defaultAvatar: undefined,
            defaultPassword: undefined,
            businessInquiryUrl: undefined
        }
    }

    static getDerivedStateFromProps(nextProps: ISettingsFormProps, state: any) {
        const { values: { intent_minimum_matching_degree, default_user_avatar, default_user_password, business_inquiry_url } } = nextProps;
        const minimumMatchingDegree = Number.parseFloat(intent_minimum_matching_degree);
        console.log(default_user_password !== state.defaultPassword);
        let nextState = {};
        let isUpdated = true;

        if (minimumMatchingDegree !== state.minimumMatchingDegree) {
            isUpdated = false;
            nextState['minimumMatchingDegree'] = minimumMatchingDegree;
        }
        if (default_user_avatar !== state.defaultAvatar) {
            isUpdated = false;
            nextState['defaultAvatar'] = default_user_avatar;
        }
        if (default_user_password !== state.defaultPassword) {
            isUpdated = false;
            nextState['defaultPassword'] = default_user_password;
        }

        if (business_inquiry_url !== state.businessInquiryUrl) {
            isUpdated = false;
            nextState['businessInquiryUrl'] = business_inquiry_url;
        }

        if (isUpdated) {

            return null;
        }
        return nextState;
    }

    onSliderChange(value: SliderValue) {
        this.setState({ minimumMatchingDegree: value })
        this.update("intent_minimum_matching_degree", value);
    }

    @Debounce(300)
    update(key: string, value: any) {
        const { onUpdate } = this.props;
        onUpdate && onUpdate({ key, value });
    }

    render() {
        const { minimumMatchingDegree, defaultAvatar, defaultPassword, businessInquiryUrl } = this.state;
        return (
            <React.Fragment>
                <Card key="luis" bordered={false} title="LUIS 设置">
                    <Form.Item label="最小匹配度">
                        <Row>
                            <Col md={6} sm={24}>
                                <Slider
                                    min={0}
                                    max={1}
                                    defaultValue={0}
                                    value={minimumMatchingDegree}
                                    onChange={this.onSliderChange.bind(this)}
                                    step={0.01}
                                />
                            </Col>
                            <Col md={18} sm={24}>
                                <span className={styles.degreelabel}>{minimumMatchingDegree}</span>
                            </Col>
                        </Row>
                    </Form.Item>
                    <Form.Item label="商务查询地址">
                        <Row>
                            <Col md={6} sm={24}>
                                <Input value={businessInquiryUrl} />
                            </Col>
                        </Row>
                    </Form.Item>
                </Card>
                <Card key="users" bordered={false} title="用户设置">
                    <Form.Item label="默认密码">
                        <Row>
                            <Col md={6} sm={24}>
                                <Password value={defaultPassword} />
                            </Col>
                        </Row>
                    </Form.Item>
                    <Form.Item label="默认头像">
                        <Row>
                            <Col md={6} sm={24}>
                                <Input value={defaultAvatar} />
                            </Col>
                        </Row>
                    </Form.Item>
                </Card>
            </React.Fragment>
        )
    }
}

export default SettingsForm;