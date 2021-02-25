import React from 'react';
import { Spin } from 'antd';

export default (props) => (
    <div style={{ width:'100%', paddingTop: 100, textAlign: 'center' }}>
        <Spin size="large" />
    </div>
);