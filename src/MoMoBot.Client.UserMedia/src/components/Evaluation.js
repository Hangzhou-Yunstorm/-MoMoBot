import React from 'react';
import { StarMarking } from 'react-yc-chatbox';

class Evaluation extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            content: '',
            rateValue: 5,
            submitting: false,
            completed: props.completed
        }
    }

    static defaultProps = {
        completed: false
    }

    shouldComponentUpdate(nextPorps, nextState) {
        return nextState.content !== this.state.content ||
            nextState.title !== this.state.title ||
            nextState.rateValue !== this.state.rateValue ||
            nextState.submitting !== this.state.submitting ||
            nextState.completed !== this.state.completed;
    }

    render() {
        let containerWidth = document.documentElement.clientWidth * 0.6;
        const { onSubmitCallback } = this.props;
        const { content, rateValue, completed, submitting } = this.state;
        return (
            <div className="evaluation" style={{ width: `${containerWidth}px` }}>
                <div className="evaluation-row bold">{completed ? '感谢您的评价！' : '请对本次服务进行评价：'}</div>
                <div className="evaluation-row">
                    <span className="bold">评分：</span>
                    <StarMarking rateValue={rateValue}
                        canClick={!completed}
                        handleSelectRate={(value) => {
                            this.setState({ rateValue: value })
                        }} />
                </div>
                <div className="evaluation-row">
                    {
                        completed ?
                            <p>{content}</p> :
                            <textarea value={content}
                                placeholder="请输入评价"
                                rows={4}
                                onChange={(e) => { this.setState({ content: e.target.value }) }} />
                    }
                </div>
                <div className="evaluation-row">
                    {
                        !completed &&
                        <div className="text-right">
                            <button className="yc-primary-btn"
                                disabled={submitting}
                                onClick={() => {
                                    this.setState({ completed: true, submitting: true });
                                    if (typeof onSubmitCallback === 'function') {
                                        onSubmitCallback({ ...this.state });
                                    }
                                }}>提交</button>
                        </div>
                    }
                </div>
            </div>
        )
    }
}

export default Evaluation;