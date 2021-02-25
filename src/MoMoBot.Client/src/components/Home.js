import React from 'react';
import * as dd from 'dingtalk-jsapi';
import axios from 'axios';
import { ChatBox, MessageRoles, MessageTypes, MessageStatus, scrollTo, guid } from 'react-yc-chatbox';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { actionCreators } from '../store/Chat';
import Toast from 'antd-mobile/lib/toast';
import 'antd-mobile/lib/toast/style/css'
import ddService from '../services/ddService';
import Feedback from './Feedback';
import messageService from '../services/messageService';
import { convertToText } from '../services/voiceService';
import config from '../config';
import signalrService from '../services/signalrService';
import Zmage from 'react-zmage';
import request from '../utils/request';
import Translate from './Translate';


// T
// const startSpeekKey = 32;

class Home extends React.Component {
    state = {
        loadding: true,
        canRecord: false,
        config: {},
        showFeedback: false,
        showTranslate: false,
        calling: false,
        humanServiceStarted: false,
    }

    _fileNode = null;
    _voiceId = '';
    _config = {};
    _selectItem = {};
    _recorder = null;

    _keydowned = false;

    UNSAFE_componentWillReceiveProps(nextProps) {
        if (nextProps.messages.length > this.props.messages.length) {
            const chatbox = document.getElementById('messages');
            if (chatbox !== null) {
                scrollTo(chatbox, chatbox.scrollHeight, 300);
            }
        }
    }

    componentDidMount() {
        this.getDDIdentity();
        // this.fetchData();

        // window.onkeydown = (e) => {
        //     e = e || window.event;
        //     if (e.keyCode === startSpeekKey) {
        //         if (this._keydowned) {
        //             return;
        //         }
        //         this.startTalk();
        //         this._keydowned = true;
        //     }
        // }

        // window.onkeyup = (e) => {
        //     e = e || window.event;
        //     if (e.keyCode === startSpeekKey) {
        //         this.endTalk();
        //         this._keydowned = false;
        //     }
        // }

        // document.addEventListener('click', () => {
        //     document.getElementById('voiceplayer').play();
        // })
    }

    // componentWillUnmount() {

    // }

    getDDIdentity = () => {
        const { receive } = this.props;
        // 在钉钉中
        if (dd.version && (dd.ios || dd.android || dd.pc)) {
            if (dd.ios || dd.android) {
                this.setState({ canRecord: true });
            }
            axios.get(`${config.serverUrl}/api/info/dd`)
                .then((response) => {
                    const config = response.data;
                    dd.config({
                        agentId: config.agentId, // 必填，微应用ID
                        corpId: config.corpId,//必填，企业ID
                        timeStamp: config.timeStamp, // 必填，生成签名的时间戳
                        nonceStr: config.nonceStr, // 必填，生成签名的随机串
                        signature: config.signature, // 必填，签名
                        jsApiList: [
                            'runtime.info',
                            'device.audio.startRecord',
                            'device.audio.stopRecord',
                            'device.audio.onRecordEnd',
                            'device.audio.download',
                            'device.audio.play',
                            'device.audio.translateVoice',
                            'runtime.permission.requestAuthCode'
                        ]
                    });

                    dd.ready(() => {
                        const { corpId, token } = config;
                        // 免登获取钉钉用户信息
                        ddService.getUserInfo(corpId, token, (userInfo) => {
                            this.state.loadding && this.setState({ loadding: false });
                            // 存放钉钉用户信息                            
                            this.props.saveDDUserInfo(userInfo);
                        })

                        // 并且在苹果或者安卓设备中
                        // if (dd.ios || dd.android) {
                        //     this.setState({ canRecord: true });
                        // }
                        // dd.device.audio.onRecordEnd({
                        //     onSuccess: (res) => {
                        //         this.onSubmit(res, MessageTypes.Voice);
                        //     },
                        //     onFail: function (err) {

                        //     }
                        // });


                    })
                })
                .catch((e) => {
                    this.state.loadding && this.setState({ loadding: false });
                });

            dd.error(error => {
                console.error(error);
                // alert(JSON.stringify(error));
                this.state.loadding && this.setState({ loadding: false });
            });
        } else {
            this.setState({ loadding: false })
            receive && receive({ name: '小摩AI', content: "未能获取到您的身份信息，部分功能暂不可用！", id: guid(), type: MessageTypes.Text, role: MessageRoles.Other, datetime: new Date() });
        }
    }


    fetchData() {
        this.props.requestHistoryMesssages();
    }

    onSubmit(message, type) {
        const { send, receive, sent } = this.props;
        const { humanServiceStarted } = this.state;

        // 人工服务
        if (humanServiceStarted) {
            const id = guid();
            const content = type === MessageTypes.Voice ? message.text : message;
            const msg = { name: '我', id, status: MessageStatus.Sending, content, type, role: MessageRoles.Self, datetime: new Date() };
            receive(msg);
            signalrService.send(msg, () => {
                sent(id);
            }, e => {
                console.error(e);
                sent(id, false);
            })
        } else {
            const id = guid();
            const content = type === MessageTypes.Voice ? message.text : message;
            const voice = type === MessageTypes.Voice ? message : null;

            let data = null;
            if (this._selectItem.title) {
                data = this._selectItem;
            }

            let question = { name: '我', id, status: MessageStatus.Sending, content, data, voice, type, role: MessageRoles.Self, datetime: new Date() };
            send && send(question, item => {
                this._selectItem = item;
                let itemQuestion = { name: '我', id: guid(), data: item, status: MessageStatus.Sending, content: item.title, type: MessageTypes.Text, role: MessageRoles.Self, datetime: new Date() };
                send(itemQuestion, null);
            });
        }
        return true;
    }

    /**开始录音 */
    startTalk = () => {
        const { receive } = this.props;
        this._voiceId = guid();
        if (this.state.canRecord) {
            dd.device.audio.startRecord({
                onSuccess: () => {//支持最长为60秒（包括）的音频录制
                    receive && receive({ content: "正在说话...", id: this._voiceId, type: MessageTypes.Text, role: MessageRoles.Self, datetime: new Date() });
                },
                onFail: (err) => {
                    // alert('error');
                }
            })
            // this._recorder.start();
        }
    }

    /**结束录音 */
    endTalk = () => {
        const { remove } = this.props;
        console.log(this.state.canRecord);
        if (this.state.canRecord) {
            dd.device.audio.stopRecord({
                onSuccess: (res) => {
                    // res.mediaId; // 返回音频的MediaID，可用于本地播放和音频下载
                    // res.duration; // 返回音频的时长，单位：秒
                    if (res.duration <= 0.8) {
                        Toast.fail('说话时间太短！', 1);
                        remove && remove(this._voiceId);
                        return;
                    }
                    dd.device.audio.translateVoice({
                        mediaId: res.mediaId,
                        duration: res.duration,
                        onSuccess: (tResult) => {
                            const result = { media: { url: res.mediaId }, duration: res.duration, text: tResult.content };
                            console.log(result);
                            remove && remove(this._voiceId);
                            this.onSubmit({ ...result }, MessageTypes.Voice);
                        }
                    });
                },
                onFail: (err) => {
                }
            });
        }

    }

    componentWillUnmount() {
        this._recorder.close();//释放录音资源
    }

    convertToText = (blob, callback) => {
        // const { mediaId, duration } = res;
        // if (this.state.canRecord && mediaId) {
        //     dd.device.audio.translateVoice({
        //         mediaId: mediaId,
        //         duration: duration,
        //         onSuccess: function (res) {
        //             let text = res.content;
        //             if (!text || text === '') {
        //                 text = "没有听清。";
        //             }
        //             callback(text);
        //         },
        //         onFail: () => {
        //             callback('没有听清。')
        //         }
        //     });
        // }
        if (blob) {
            convertToText(blob, callback);
            // .then(response => {
            //     console.log(response);
            //     let text = response;
            //     if (!text || text === '') {
            //         text = "没有听清。";
            //     }
            //     callback(text);
            // })
            // .catch(e => {
            //     callback('没有听清。')
            // })
        }
    }

    TTS = (text) => {
        return request({
            url: `${config.serverUrl}/api/voice/tts?text=${text}`,
            method: 'get'
        }).then(response => {
            return `${config.serverUrl}/${response}`;
        }).catch(()=>{
            // todo : error
        })

        // return new Promise((resolve) => {
        //     resolve(`${config.serverUrl}/api/voice/tts?text=${text}`);
        // })

    }

    /**播放语音消息 */
    playRecord = (media, callback) => {
        const { url } = media;
        dd.device.audio.download({
            mediaId: url,
            onSuccess: (res) => {
                dd.device.audio.play({
                    localAudioId: res.localAudioId,

                    onSuccess: () => {
                        callback && callback();
                    }
                });
            },
            onFail: (err) => {
            }
        });
    };

    play = (id, url, Error, Start, End) => {

        const { loadingVoice, loadedVoice } = this.props;
        var player = document.getElementById('voiceplayer');
        // player.volume = 0.3
        player.src = url;

        loadingVoice(id);
        // player.play();
        // player.pause();

        const play = () => {
            player.play();

            Start && Start();
            // reset();
        };

        const error = (err) => {
            // alert(JSON.stringify(err));
            Error && Error(err);
            reset();
        }

        const end = () => {
            End && End();
            reset();
        }

        const reset = () => {
            loadedVoice(id);
            player.removeEventListener('error', error);
            player.removeEventListener('pause', end);
            player.removeEventListener('ended', end);
        }


        player.addEventListener('error', error);
        player.addEventListener('pause', end);
        player.addEventListener('ended', end);
        play();
    }

    /**开始人工服务 */
    startHumanServices = () => {
        const { ddUser } = this.props;
        this.setState({ calling: true });
        const { receive } = this.props;

        const funcs = {
            Connected: () => {

            },
            Waiting: () => {
                //this.setState({ calling: false });
                let connectNotice = messageService.createNoticeMessage('等待客服接入');
                receive && receive(connectNotice);
            },
            Started: () => {
                this.setState({ calling: false, humanServiceStarted: true });
                let connectNotice = messageService.createNoticeMessage('已为您接通人工服务');
                receive && receive(connectNotice);
            },
            Error: (e) => {
                console.error(e);
                this.setState({ calling: false });
                receive && receive({ content: e, id: guid(), type: MessageTypes.Notice, role: MessageRoles.System, datetime: new Date() });
            },
            Receive: (msg) => {
                const { type, data } = msg;
                let message = {};
                if (type === 4) {
                    const img = (
                        <div>
                            <Zmage src={data} style={{ maxWidth: '100%' }} backdrop="rgba(0,0,0,.3)" />
                        </div>
                    )
                    message = { content: img, datetime: new Date(), name: '客服', role: MessageRoles.Other, type: MessageTypes.Html }
                } else {

                    message = { content: msg.content, datetime: new Date(), name: '客服', role: MessageRoles.Other, type: MessageTypes.Text };
                }
                receive && receive(message);
            },
            Hangup: (recordId) => {
                console.log(recordId);
                const { humanServiceStarted } = this.state;
                const { receive } = this.props;

                let hangUpNotice = messageService.createNoticeMessage('已退出人工服务');
                receive && receive(hangUpNotice);
                if (humanServiceStarted && recordId > 0) {
                    let evaluation = messageService.createEvaluationMessage((value) => {
                        console.info(value);
                    });
                    receive && receive(evaluation);
                }
                this.setState({ calling: false, humanServiceStarted: false });
            },
            Cancel: () => {
                let cancelNotice = messageService.createNoticeMessage('已取消呼叫人工服务');
                receive && receive(cancelNotice);
                this.setState({ calling: false });
            }
        }

        // userid:'89564548745456456',name:'王五'
        signalrService.connect(ddUser || {}, { ...funcs });
    }

    callTimeout = () => {
        this.setState({ calling: false, humanServiceStarted: false });
        const { receive } = this.props;
        let timeoutNotice = messageService.createNoticeMessage('等待人工服务超时，当前客服正忙');
        receive && receive(timeoutNotice);
    }
    getMenus = () => {
        const { humanServiceStarted, calling } = this.state;
        let menutext = "";
        if (!humanServiceStarted && !calling) {
            menutext = "人工服务";

        } else if (humanServiceStarted) {
            menutext = "退出人工";
        } else if (calling) {
            menutext = "取消接入";
        }
        return [
            {
                icon: 'rengongfuwu1', text: menutext, onClick: () => {
                    if (!this.state.calling && !this.state.humanServiceStarted) {
                        this.startHumanServices();
                    } else {
                        signalrService.hangup(this.state.calling);
                    }
                    return true;
                }
            },
            {
                icon: 'tupian', text: '图片', disabled: !humanServiceStarted, onClick: () => {
                    if (this._fileNode && this._fileNode != null) {
                        this._fileNode.click();
                        console.log('sss');
                    }
                }
            },
            { icon: 'file', text: '文件', disabled: !humanServiceStarted, onClick: () => { alert('文件') } },
            {
                icon: 'feedback', text: '反馈', onClick: () => {
                    this.setState({ showFeedback: true })
                    return true;
                }
            },
            {
                icon: 'zhuanhuan', text: '翻译工具', onClick: () => {
                    this.setState({ showTranslate: true })
                    return true;
                }
            },
        ];
    }

    imageFileChange(e) {
        const { append, sent } = this.props;
        var files = e.target.files;
        if (files && files.length > 0) {
            var file = files[0];
            if (!(/(?:jpg|gif|png|jpeg)$/i.test(file.name))) {
                console.log('文件格式有误');
                return;
            }
            const url = URL.createObjectURL(file);
            const id = guid();
            const $img = (<Zmage src={url} style={{ maxWidth: '100%' }} backdrop="rgba(0,0,0,.3)" />)
            const imgMessage = {
                id, name: '我', status: MessageStatus.Sending, type: MessageTypes.Html, role: MessageRoles.Self, datetime: new Date(), content: $img
            }
            append(imgMessage);
            const formData = new FormData();
            formData.append('file', file);
            request({
                url: `${config.serverUrl}/api/file/upload-image`,
                method: 'POST',
                data: formData
            }).then(response => {
                const imgUrl = `${config.serverUrl}/${response}`;
                signalrService.send({ content: '[图片]', data: imgUrl, type: 4 }, () => {
                    sent(id)
                })
            })
        }
    }

    render() {
        const { showFeedback, showTranslate, loadding } = this.state;
        const { playStart, receive, playEnd } = this.props;
        const feedbackStyle = showFeedback ? { left: '0px' } : { left: '-100vw' };
        const title = (
            <React.Fragment>
                小摩AI <i>v1.0</i>
            </React.Fragment>);

        const inputProps = {
            onSubmit: this.onSubmit.bind(this),
            voiceEnabled: this.state.canRecord,
            onRecordStart: this.startTalk,
            onRecordEnded: this.endTalk
        }

        const messageProps = {
            tts: true,
            onTTS: (message) => {
                this.TTS(message.content).then(url => {
                    console.log(url);
                    this.play(message.id, url, (err) => {
                        console.error(err);
                        receive({ id: guid(), type: MessageTypes.Notice, role: MessageRoles.System, content: "网络错误，暂不可用", datetime: new Date() });
                    }, () => {
                        playStart(message.id);
                    }, () => {
                        playEnd(message.id);
                    });
                });

            },
            feedback: true,
            conversion: true,
            messages: this.props.messages,
            hasNewMessage: false,
            displayTitle: true,
            displayText: false,
            displayStatus: true,
            playRecord: this.playRecord,
            onItemSelected: (message, index) => {
                messageService.feedback(message, index);
                return true;
            }
        }

        // const extendsNode = (
        //     <div style={{ height: '30px', paddingBottom: '4px' }}>
        //         {calling && <Calling onHangUp={this.hangUp}
        //             onTimeout={this.callTimeout}
        //             timeoutSeconds={180} />}
        //     </div>
        // );

        return (
            <React.Fragment>
                <ChatBox
                    id="messages"
                    menus={this.getMenus()}
                    displayHeader={false}
                    headerProps={{ title }}
                    messageProps={messageProps}
                    loading={loadding}
                    inputProps={inputProps}>
                </ChatBox>
                <Feedback style={feedbackStyle}
                    onClosed={() => { this.setState({ showFeedback: false }) }}
                    onSubmit={(value) => { console.info(value); }} />

                <Translate onClosed={() => { this.setState({ showTranslate: false }) }}
                    style={showTranslate ? { left: '0px' } : { left: '-100vw' }} />

                <audio preload="none" style={{ display: 'none' }} controls={false} autoPlay={false} id="voiceplayer">
                    <source src="" type="audio/x-wav" />
                </audio>
                <input ref={node => this._fileNode = node} type="file" id="imageSender" style={{ display: 'none' }} onChange={this.imageFileChange.bind(this)} />

            </React.Fragment>
        );
    }
}
export default connect(
    state => state.chat,
    dispatch => bindActionCreators(actionCreators, dispatch)
)(Home);