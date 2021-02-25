import React from 'react';

class HangUp extends React.Component {
    static defaultProps = {
        onHangUp: () => { }
    }

    render() {
        const { onHangUp } = this.props;
        return (
            <div className="hang-up"
                onClick={() => { onHangUp && onHangUp(); }}>
                挂断
            </div>
        )
    }
}

export default HangUp;