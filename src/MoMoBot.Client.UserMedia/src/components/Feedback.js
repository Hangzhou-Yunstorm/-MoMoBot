import React from 'react';

export default class Feedback extends React.Component {
    state = {
        title: '',
        content: ''
    }

    reset = () => {
        const { onClosed } = this.props;
        onClosed && onClosed();
        this.setState({
            title: '',
            content: ''
        })
    }

    render() {
        const { style, onSubmit } = this.props;
        return (
            <div className="feedback-container" style={style}>
                <div className="feedback-row">
                    <h1>反馈</h1>
                </div>
                <div className="feedback-row">
                    <input type="text" value={this.state.title} placeholder="请输入主题" onChange={e => this.setState({ title: e.target.value })}/>
                </div>
                <div className="feedback-row">
                    <textarea rows={8} placeholder="请输入反馈内容" value={this.state.content} onChange={e => this.setState({ content: e.target.value })} />
                </div>
                <div className="feedback-row text-right">
                    <button className="yc-danger-btn" onClick={this.reset}>关闭</button>
                    <button className="yc-primary-btn" onClick={() => {
                        onSubmit && onSubmit({ ...this.state });
                        this.reset();
                    }}>发送</button>
                </div>
            </div>
        )
    }
}