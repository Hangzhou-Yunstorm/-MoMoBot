import * as React from 'react';
import classnames from 'classnames';
import IChatContact from 'src/@types/IChatContact';
const moment = require('moment');
import 'moment/locale/zh-cn';
import { Link } from 'react-router-dom';
const styles = require('./index.less');
import PageLoading from '../PageLoading';

interface IChatListProps {
    activeId?: number;
    data?: IChatContact<number>[];
    loading?: boolean;
    location?: any;
    onItemSelected?: (id: number) => void;
}

interface IChatRecordItemProps {
    item: IChatContact<number>,
    isActive?: boolean;
    onClick?: (id: number) => void;
}

class ChatRecordItem extends React.Component<IChatRecordItemProps> {
    static defaultProps = {
        isActive: false
    }
    render() {
        const { item: { id, name, time, message, unread, online }, isActive, onClick } = this.props;
        return (

            <Link to={`/im/chat?id=${id}`}
                onClick={() => { onClick && onClick(id); }}
                title={`与${name}对话`}>
                <div className={classnames(styles.item, isActive ? styles.active : '', unread ? styles.unread : '')}>
                    <div className={styles.header}>
                        <span className={styles.itemtitle}>{name}</span>
                        <time className={styles.itemtime}>{moment(time).calendar()}</time>
                    </div>

                    <div className={styles.body}>
                        <p className={styles.content}>{message === '' || message === null ? '无消息' : message}</p>
                        <div className={styles.status}
                            title={online ? '在线' : '离线'}>
                            <span className={online ? styles.online : styles.offline} />
                        </div>
                    </div>

                </div>
            </Link>
        )
    }
}

export default class ChatList extends React.Component<IChatListProps, any> {
    static defaultProps = {
        data: [],
        activeId: -1
    }
    constructor(props: IChatListProps) {
        super(props);
        this.state = {
            activeId: props.activeId
        }
    }

    static getDerivedStateFromProps(props: IChatListProps, state: any) {
        if (props.activeId !== state.activeId) {
            return { activeId: props.activeId }
        }
        return null;
    }

    // shouldComponentUpdate(nextProps: IChatListProps) {
    //     return nextProps.activeId !== this.props.activeId ||
    //         nextProps.loading !== this.props.loading ||
    //         !this.isEqual(nextProps.data, this.props.data);
    // }

    isEqual = (arr1?: IChatContact<number>[], arr2?: IChatContact<number>[]) => {
        // 二者都有值
        if (arr1 && arr2) {
            if (arr1.length !== arr2.length) {
                return false;
            }
            return arr1!.length === this.props.data!.length && arr1!.every((e, i) =>
                e.name === this.props.data![i].name &&
                e.id === this.props.data![i].id &&
                e.time === this.props.data![i].time &&
                e.message === this.props.data![i].message &&
                e.unread === this.props.data![i].unread)
            // 二者有一个为undefined
        } else {
            return false;
        }

    }

    itemClick = (id: number) => {
        const { onItemSelected } = this.props;
        onItemSelected && onItemSelected(id);
    }

    render() {
        const { data, loading } = this.props;
        const { activeId } = this.state;
        console.log(data);
        return loading ?
            <PageLoading /> :
            <div className={styles.chatlist}>
                {
                    data && data.length > 0 ?
                        data.map((data: IChatContact<number>, index) =>
                            (<ChatRecordItem key={index} onClick={id => this.itemClick(id)} isActive={activeId === data.id} item={data} />)) :
                        <div className={styles.empty}>没有对话记录</div>
                }
            </div>
    }
}
