import React from 'react';
import { Icon } from 'antd-mobile';
import request from "../utils/request";
import config from '../config';
// import { Link } from 'react-router-dom'

export default class Translate extends React.PureComponent {
    constructor(props) {
        super(props);
        this.state = {
            languages: [],
            sourceLang: '',
            targetLang: 'en',
            sourceText: '',
            tragetText: ''
        }
    }
    timer = undefined;

    componentDidMount() {
        request({
            url: `${config.serverUrl}/api/translate/languages`,
            method: 'get'
        }).then(response => {
            this.setState({ languages: response })
        })
    }

    sourceTextChange = (e) => {
        const value = e.target.value;
        this.setState({ sourceText: value });

        if (this.timer && this.timer != null) {
            clearTimeout(this.timer)
        }
        this.setState({ tragetText: '正在翻译...' })
        this.timer = setTimeout(() => {
            const { targetLang, sourceLang } = this.state;
            this.translate(value, targetLang, sourceLang);
            this.timer = undefined;
        }, 300)

    }

    sourceTextKeyDown = (e) => {
        if (e.ctrlKey && e.keyCode === 13) {
            const { sourceLang, targetLang, sourceText } = this.state;
            this.translate(sourceText, targetLang, sourceLang);
        }
    }

    translate = (text, to, from = '') => {

        if (text.trim() !== '') {
            request({
                url: `${config.serverUrl}/api/translate/text`,
                method: 'post',
                data: {
                    text,
                    to,
                    from
                }
            }).then(response => {
                console.log(response);
                const { translations } = response[0];
                const { text: result } = translations[0];
                this.setState({ tragetText: result })
            }).catch(e => {
                this.setState({ tragetText: '翻译失败' })
            })
        } else {
            this.setState({ tragetText: '' })
        }
    }

    targetSelectChange = (e) => {
        const to = e.target.value;
        this.setState({ targetLang: to });
        const { sourceLang, sourceText } = this.state;
        this.translate(sourceText, to, sourceLang);
    }

    sourceSelectChange = (e) => {
        const from = e.target.value;
        this.setState({ sourceLang: from })
        const { targetLang, sourceText } = this.state;
        this.translate(sourceText, targetLang, from);
    }

    copyResult = (e) => {
        e.target.select();
        document.execCommand("copy")
    }

    render() {
        const { style, onClosed } = this.props;
        const { sourceLang, targetLang, sourceText, tragetText, languages } = this.state;

        return (
            <div className="translate" style={style}>
                <div className="translate-header">
                    <a href="javascript:;" onClick={() => onClosed && onClosed()}><Icon style={{ height: '35px' }} type="left" /></a>
                    <h1>翻译工具</h1>
                </div>
                <div className="translate-selects">
                    <select onChange={this.sourceSelectChange} className="lang-select" value={sourceLang}>
                        <option value=''>自动检测</option>
                        {languages.map((item, index) => <option value={item.lang} key={`source_${index}`}>{item.name}</option>)}
                    </select>
                    <Icon className="lang-icon" type="right" />
                    <select onChange={this.targetSelectChange} className="lang-select" value={targetLang}>
                        {languages.map((item, index) => <option value={item.lang} key={`source_${index}`}>{item.name}</option>)}
                    </select>
                </div>
                <div className="translate-text">
                    <textarea maxLength={250}
                        value={sourceText}
                        onChange={this.sourceTextChange}
                        onKeyDown={this.sourceTextKeyDown}
                        placeholder="输入文字（CTRL + ENTER）" />
                    <span>{sourceText.length}/250</span>
                </div>
                <textarea value={tragetText} readOnly onClick={this.copyResult} />
                <p className="text-second">翻译服务由 Azure 提供</p>
            </div>
        )
    }
}