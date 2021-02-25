import * as React from 'react';
import IMessage, { MessageRoles, MessageTypes } from '../../@types/IMessage';
import * as classNames from 'classnames';
const styles = require('./index.less');
const moment = require('moment');
import 'moment/locale/zh-cn';
import { Icon, Popover, List } from 'antd';
import Zmage from 'react-zmage'

interface IChatBoxProps {
    inputArea?: IInputAreaProps,
    messageBox?: IMessageBoxProps
}

interface IInputAreaProps {
    placeholder?: string;
    disabled?: boolean;
    onSubmit?: (value: string | File) => boolean;
}

interface IMessageBoxProps {
    messages?: IMessage[],
    onRef?: (node: HTMLDivElement | null) => void
}

interface ITextMessageProps {
    message: IMessage
}

interface IQuickReplyListProps {
    data: string[];
    onItemClick?: (item: string) => void;
}

class TextMessage extends React.PureComponent<ITextMessageProps> {
    render() {
        const { message: { content, time, who } } = this.props;
        return (
            <div className={classNames(styles.message, styles.textmessage, who === MessageRoles.Self ? styles.self : styles.other)}>
                <div className={styles.body}>
                    {content}
                </div>
                <p className={styles.footer}>
                    <time className={styles.time}>{moment(time).calendar()}</time>
                </p>
            </div>
        )
    }
}

class NoticeMessage extends React.PureComponent<any> {
    render() {
        const { notice } = this.props;
        return (
            <div className={classNames(styles.message, styles.notice)}>
                <p className={styles.body}>
                    {notice.content}
                </p>
            </div>
        )
    }
}

class ImageMessage extends React.PureComponent<any> {
    render() {
        const { message } = this.props;
        const { data, who, time } = message;
        return (
            <div className={classNames(styles.message, styles.image, who === MessageRoles.Self ? styles.self : styles.other)}>
                <div className={styles.body}>
                    <Zmage src={data} backdrop="rgba(0,0,0,.3)" />
                </div>
                <p className={styles.footer}>
                    <time className={styles.time}>{moment(time).calendar()}</time>
                </p>
            </div >
        )
    }
}

class MessageBox extends React.PureComponent<IMessageBoxProps> {

    boxNode: HTMLDivElement | null;

    componentDidMount() {
        const { onRef } = this.props;
        onRef && onRef(this.boxNode);
    }

    renderMessages() {
        const { messages } = this.props;
        if (messages) {
            return messages.map((item: IMessage, index: number) => {
                if (item.who === MessageRoles.System) {
                    return <NoticeMessage key={index} notice={item} />
                }
                if (item.type === MessageTypes.Image) {
                    return <ImageMessage key={index} message={item} />
                }
                return <TextMessage key={index} message={item} />
            });
        }


        return '';
    }

    render() {

        return (
            <div className={styles.messages} ref={node => { this.boxNode = node; }}>
                {this.renderMessages()}
            </div>
        )
    }
}

class QuickReplyList extends React.PureComponent<IQuickReplyListProps> {

    render() {
        const { data, onItemClick } = this.props;
        return (
            <ul className={styles.quickreplylist}>
                {data.map((item, index) =>
                    (<li key={index} onClick={() => { onItemClick && onItemClick(item) }} className={styles.quickreplyitem}><p>{item}</p></li>))}
            </ul>
        )
    }
}

class InputArea extends React.PureComponent<IInputAreaProps, any> {
    constructor(props: IInputAreaProps) {
        super(props);
        this.state = {
            value: '',
            popoverVisible: false
        }
    }
    static defaultProps = {
        onSubmit: (value: string) => { console.log(value); return true; },
        placeholder: "请输入内容（CTRL + ENTER 快速发送）",
        disabled: false
    }

    clickHandler = () => {

        const { value } = this.state;
        if (value === '') {
            return;
        }
        this.sendText(value);
    }

    textChangeHandler(e: any) {
        this.setState({ value: e.target.value });
    }

    sendText(value: string) {
        const { onSubmit } = this.props;
        var result = true;
        if (onSubmit) {
            result = onSubmit(value);
        }
        result && this.setState({ value: '' })
    }

    sendImage() {
        const { disabled } = this.props;
        if(disabled){
            return;
        }
        const file = document.getElementById('selectfile');
        if (file && file !== null) {
            file.click();
        }
    }

    fileChange(e: React.ChangeEvent<HTMLInputElement>) {
        const files = e.target.files;
        if (files && files.length && files.length > 0) {
            // 发送图片
            const file = files[0];
            const formData = new FormData();
            formData.append('file', file);
            const { onSubmit } = this.props;
            onSubmit && onSubmit(file);
        }
    }

    QuickReply(item: string) {
        const { disabled } = this.props;
        this.setState({ popoverVisible: false });
        if(disabled){
            return;
        }
        this.sendText(item);
    }

    render() {
        const { value } = this.state;
        const { placeholder, disabled } = this.props;
        const btnDisabled = this.state.value.trim() === '' || disabled;
        const quickReplyData: string[] = [
            "您好，请问有什么可以帮助您的？",
            "好的，我们会尽快处理！",
            "感谢您的反馈！",
            "再见，祝您生活愉快！"
        ];
        return (
            <div className={styles.inputarea}>
                <input onChange={this.fileChange.bind(this)} style={{ display: 'none' }} type="file" multiple={false} id="selectfile" />
                <div className={styles.inputbox}>
                    <textarea className={styles.textarea}
                        disabled={disabled}
                        onChange={this.textChangeHandler.bind(this)}
                        onKeyDown={(e) => { if (e.keyCode == 13 && e.ctrlKey) { this.clickHandler() } }}
                        value={value}
                        placeholder={placeholder} />
                    <div className={styles.footer}>
                        <ul className={styles.footMenu}>
                            <li title="快捷回复">
                                <Popover title="快捷回复"
                                    placement="topLeft"
                                    trigger="click"
                                    visible={this.state.popoverVisible}
                                    onVisibleChange={(visible) => { this.setState({ popoverVisible: visible }) }}
                                    content={(<QuickReplyList data={quickReplyData}
                                        onItemClick={this.QuickReply.bind(this)} />)}>
                                    <Icon type="pushpin" />
                                </Popover>
                            </li>
                            <li title="发送图片" onClick={this.sendImage.bind(this)}><Icon type="picture" /></li>
                            <li className={styles.disabled} title="发送文件"><Icon type="file" /></li>
                            <li className={styles.disabled} title="发送链接"><Icon type="link" /></li>
                            <li className={styles.disabled} title="表情"><Icon type="smile" /></li>
                        </ul>
                        <button disabled={btnDisabled} className={styles.send}
                            onClick={this.clickHandler.bind(this)}>发送消息</button>
                    </div>
                </div>
            </div>
        )
    }
}

export default class ChatBox extends React.Component<IChatBoxProps> {

    render() {
        const { messageBox, inputArea } = this.props;

        return (
            <React.Fragment>
                <MessageBox {...messageBox} />
                <InputArea {...inputArea} />
            </React.Fragment>
        )
    }
}