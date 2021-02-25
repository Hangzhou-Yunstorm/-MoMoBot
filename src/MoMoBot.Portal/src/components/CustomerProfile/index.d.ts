import * as React from 'react';
import { IProfileProps } from './Profile';
import { IServiceRecordsProps } from './ServiceRecords';

export interface ICustomerProfileProps {
    profile?: IProfileProps;
    record?: IServiceRecordsProps
}

declare class CustomerProfile extends React.PureComponent<ICustomerProfileProps> {
    static defaultProps: Partial<ICustomerProfileProps>;
    render(): JSX.Element;
}

export default CustomerProfile;