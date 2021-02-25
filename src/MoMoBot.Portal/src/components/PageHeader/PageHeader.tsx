import * as React from 'react';
import { Icon } from 'antd';
const styles = require('./PageHeader.less');

interface IPageHeaderProps {
    title: string
}

export default class PageHeader extends React.Component<IPageHeaderProps> {
    shouldComponentUpdate(nextProps: IPageHeaderProps) {
        return nextProps.title !== this.props.title;
    }

    render() {
        const { title } = this.props;
        return (
            <div className={styles.header}>
                <h1 className={styles.title}>
                    <span className={styles.icon}><Icon type="align-left" /></span> {title}
                </h1>
            </div>
        )
    }
}