import * as React from 'react';
import ChatContent from '../components/Chat';
import { connect } from 'dva';
import IChatState from '../@types/IChatState';
import { IIdentity } from '../@types/IIdentity';
import { send, sendImage } from '../services/im.service';
import settings from '../settings';
import { MessageTypes, MessageRoles } from '../@types/IMessage';

@connect(({ chat, identity }: { chat: IChatState, identity: IIdentity }) => ({
    record: chat.activeRecord,
    userid: identity.userInfo.id,
    records: chat.records
}))
class IM extends React.PureComponent<any> {

    close() {
        const { dispatch } = this.props;
        dispatch({ type: 'chat/clearActive' })
    }

    onSend(value: string | File) {
        const { record, userid, dispatch } = this.props;
        const { groupName } = record;
        // 文本消息
        if (typeof value === 'string') {
            const message = {
                groupName: groupName,
                content: value,
                sender: userid,
                who: MessageRoles.Self,
                time: new Date()
            };
            send(message)
                .then(() => {
                    dispatch({
                        type: 'chat/newMessage',
                        payload: message
                    })
                });
        } else {
            // 文件消息
            sendImage(value)
                .then((url: any) => {
                    const imgUrl = `${settings.serverUrl}/${url}`;
                    const imageMessage = {
                        groupName: groupName,
                        type: MessageTypes.Image,
                        content: '[图片]',
                        data: imgUrl,
                        sender: userid,
                    };
                    send(imageMessage).then(() => {
                        dispatch({
                            type: 'chat/newMessage',
                            payload: {
                                ...imageMessage,
                                who: MessageRoles.Self
                            }
                        })
                    })

                });
        }

        return true;
    }


    render() {
        const { record, records, userid } = this.props;
        return (
            <ChatContent record={record}
                userid={userid}
                records={records}
                onSend={this.onSend.bind(this)}
                onClose={this.close.bind(this)} />
        )
    }
}

export default IM;