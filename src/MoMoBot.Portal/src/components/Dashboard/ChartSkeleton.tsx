import * as React from 'react';
import { Skeleton } from 'antd';

const paragraphWidth = ['100%', '100%', '100%', '100%', '100%']

export default (props: any) => {
    const { children, loading } = props;
    return (
        <Skeleton loading={loading}
            active={true}
            title={false}
            paragraph={{ rows: paragraphWidth.length, width: paragraphWidth }}>
            {children}
        </Skeleton>
    )
}