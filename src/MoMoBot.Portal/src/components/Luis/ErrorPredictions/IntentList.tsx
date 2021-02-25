import * as React from 'react';
import * as classNames from 'classnames';
const styles = require('./index.less');

interface IIntentListProps {
    className?: string;
    data: Array<{
        modelName: string;
        modelId: string
    }>;
    activeKey?: string;
    onItemClick?: (id: string) => void;
}

export default class IntentList extends React.Component<IIntentListProps> {
    render() {
        const { activeKey, className, onItemClick, data = [] } = this.props;
        var first = data[0] || {};
        let key = activeKey && data.length > 0 ? activeKey : first.modelId;

        return (
            <div className={className}>
                {
                    data.length > 0 ?
                        <ul className={styles.list}>
                            {data.map(item => {
                                return (
                                    <li className={classNames(styles.item, key === item.modelId ? styles.active : '')}
                                        key={item.modelId}
                                        onClick={() => { if (key === item.modelId) return; onItemClick && onItemClick(item.modelId) }}>
                                        {item.modelName}
                                    </li>
                                )
                            })}
                        </ul> :
                        '无'
                }
            </div>
        )
    }
}