import * as React from 'react';
import ChatBox from './ChatBox';
const styles = require('./index.less');
import { scrollTo } from 'react-yc-chatbox';
import { Icon } from 'antd';
import * as classNames from 'classnames';

export default class ChatContent extends React.PureComponent<any> {
    chatBox: HTMLDivElement | null;

    getSnapshotBeforeUpdate() {
        if (this.chatBox && this.chatBox !== null) {
            return this.chatBox.scrollTop;
        }
        return 0;
    }

    componentDidUpdate(perProps: any, perState: any, perScrollTop: number) {
        if (this.chatBox && this.chatBox !== null) {

            const to = this.chatBox.scrollHeight;
            scrollTo(this.chatBox, to, 300)
        }
    }

    close() {
        const { onClose } = this.props;
        onClose && onClose();
    }

    render() {
        const { record, records: { rows } = { rows: [] } } = this.props;
        const { online = false } = record || {};

        const chatBoxProps = {
            messages: rows,
            onRef: (node: HTMLDivElement) => { this.chatBox = node }
        }
        const inputProps = {
            onSubmit: this.props.onSend,
            disabled: !online
        }
        return (
            <div className={styles.chatcontent}>
                {record ?
                    (
                        <React.Fragment>
                            <div className={styles.header}>
                                <span className={styles.name}>
                                    {record.name}
                                    {
                                        record.online ?
                                            <Icon title="在线" className={classNames(styles.onlinestatus, styles.online)} type="bell" /> :
                                            <Icon title="离线" className={classNames(styles.onlinestatus, styles.offline)} type="disconnect" />
                                    }
                                </span>
                                <div>
                                    <button className={styles.close}
                                        onClick={this.close.bind(this)}>
                                        <Icon type="close" />
                                    </button>
                                </div>
                            </div>
                            <div className={styles.chatbox}>
                                <ChatBox messageBox={chatBoxProps}
                                    inputArea={inputProps} />
                            </div>
                        </React.Fragment>
                    ) :
                    <div className={styles.empty}>无对话</div>
                }
            </div>
        )
    }
}