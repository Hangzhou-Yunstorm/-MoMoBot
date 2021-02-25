import * as React from 'react';
import PageHeader from '../PageHeader/PageHeader';
const styles = require('./index.less');

export default class PageLayout extends React.Component<any> {
    render() {
        const { children, title } = this.props;
        return (
            <div style={{ height: "100%" }} >
                {title && <PageHeader title={title} />}
                <div className={styles.container}>
                    {children}
                </div>
            </div>
        )
    }
}