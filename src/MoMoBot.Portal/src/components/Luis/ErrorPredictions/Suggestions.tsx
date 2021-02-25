import * as React from 'react';
const styles = require('./index.less');
import QueueAnim from 'rc-queue-anim';



export default class Suggestions extends React.Component<any> {
    render() {
        const { className, suggestions = [] } = this.props;

        return (
            <div className={className}>
                <h2 className={styles.title}>针对此意图的改善建议</h2>
                <QueueAnim delay={0}>
                    {
                        suggestions.map((item: any) => {
                            return (
                                <div className={styles.suggestion} key={item.key}>
                                    {item.content}
                                </div>
                            )
                        })
                    }
                </QueueAnim>
            </div>
        )
    }
} 