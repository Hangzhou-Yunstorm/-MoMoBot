import React from 'react';

class Calling extends React.Component {
    state = {
        timer: 0,
        isTimeout: false
    }

    static defaultProps = {
        timeoutSeconds: 60
    }

    _timer = null;

    componentDidMount() {
        const { onTimeout, timeoutSeconds } = this.props;
        console.log('Mount');
        let _time = 0;
        this.setState({ timer: _time });
        this._timer = setInterval(() => {
            console.log(this.state.timer);
            if (_time >= timeoutSeconds) {
                clearInterval(this._timer);
                this._timer = null;
                onTimeout && onTimeout();
                return;
            }
            _time++;
            this.setState({ timer: _time });
        }, 1000)
    }

    componentWillUnmount() {
        if (this._timer !== null) {
            clearInterval(this._timer);
        }
    }

    hangUp = () => {
        const { onHangUp } = this.props;
        if (this._timer !== null) {
            clearInterval(this._timer);
        }
        onHangUp && onHangUp();
    }

    render() {
        const { timer } = this.state;
        const { style } = this.props;

        return (
            <div className="calling-container" style={style}>
                <button onClick={this.hangUp.bind(this)}>{timer}</button>
            </div>
        )
    }
}

export default Calling;