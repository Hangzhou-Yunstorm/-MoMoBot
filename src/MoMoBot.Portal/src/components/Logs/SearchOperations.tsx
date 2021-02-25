import * as React from 'react';
import { Select, Row, Col, DatePicker } from 'antd';
import { RangePickerValue } from 'antd/lib/date-picker/interface';
import * as moment from 'moment';
import 'moment/locale/zh-cn';
moment.locale('zh-cn');
const locale = require('antd/lib/date-picker/locale/zh_CN');

const Options = Select.Option;
const { RangePicker } = DatePicker;

export interface ISearchOperationsProps {
    levels?: Array<number>;
    className?: string;
    onSearch?: (levels?: Array<number>, start?: string, end?: string) => void;
}

class SearchOperations extends React.PureComponent<ISearchOperationsProps, any> {
    state = {
        levels: [],
        dates: []
    }

    selectChange(value: number[]) {
        this.setState({ levels: value });
        this.onSearch();
    }

    rangeChange(dates: RangePickerValue) {
        this.setState({ dates });
        this.onSearch();
    }

    onSearch() {
        const { levels, dates } = this.state;
        const { onSearch } = this.props;
        onSearch && onSearch(levels, dates[0], dates[1]);
    }

    render() {
        const { className } = this.props;
        return (
            <div className={className}>
                <Row gutter={24}>
                    <Col sm={24} md={12}>
                        <Row gutter={24}>
                            <Col sm={24} md={4}>起止日期： </Col>
                            <Col sm={24} md={20}>
                                <RangePicker locale={locale} defaultValue={[moment(new Date(), "YYYY-MM-DD"), moment(new Date(), "YYYY-MM-DD")]} onChange={this.rangeChange.bind(this)} />
                            </Col>
                        </Row>
                    </Col>
                    <Col sm={24} md={12}>
                        <Row gutter={24}>
                            <Col sm={24} md={4}>日志等级： </Col>
                            <Col sm={24} md={16}>
                                <Select mode="multiple"
                                    // defaultValue={[0, 1, 2, 3, 4, 5]}
                                    style={{ width: "100%" }}
                                    onChange={this.selectChange.bind(this)}>
                                    <Options value={0}>Verbose</Options>
                                    <Options value={1}>Debug</Options>
                                    <Options value={2}>Information</Options>
                                    <Options value={3}>Warning</Options>
                                    <Options value={4}>Error</Options>
                                    <Options value={5}>Fatal</Options>
                                </Select>
                            </Col>
                        </Row>
                    </Col>
                </Row>
            </div>
        )
    }
}


export default SearchOperations;