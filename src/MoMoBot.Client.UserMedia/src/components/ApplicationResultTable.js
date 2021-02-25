import React from 'react';

class ApplicationResultTable extends React.PureComponent {
    constructor(props) {
        super(props);
        this.state = {
            data: this.props.data,
            submitting: false,
            submited: false
        }
    }
    static defaultProps = {
        data: []
    }

    render() {
        const { onSubmit } = this.props;
        const { data, submitting, submited } = this.state;
        return (
            <table className="app-result">
                <tbody>
                    {
                        data.map((item, index) =>
                            (<tr key={index}>
                                <td>{item.text}:</td>
                                <td>{submited ? <span className="bold">{item.value}</span> : <input readOnly={submitting} name={item.name} value={item.value} onChange={e => {
                                    this.setState({ data: data.map(d => { if (d.name === item.name) { d.value = e.target.value } return d }) });
                                }} />} </td>
                            </tr>))
                    }
                </tbody>
                <tfoot>
                    <tr><td colSpan={2}>
                        {submited ? <span>申请已提交！</span> : <button className="yc-primary-btn" disabled={submitting} onClick={() => {
                            this.setState({ submitting: true });
                            onSubmit && onSubmit(data, (result,reason='') => {
                                this.setState({ submited: result, submitting: result });
                                // 失败
                                if(!result){
                                    console.error(reason);
                                }
                            });
                        }}>{submitting ? '提交中' : '提交'}</button>}
                    </td></tr>
                </tfoot>
            </table>
        )
    }
}

export default ApplicationResultTable;