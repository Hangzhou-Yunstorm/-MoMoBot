import React from 'react';
import GGEditor, { Flow } from 'gg-editor';
import { withPropsAPI } from 'gg-editor';

class FlowChart extends React.Component {
    componentDidMount() {
        const { propsAPI } = this.props;
        console.log(propsAPI);
    }

    render() {
        const {
            style,
            data,
            className,
            onChange
        } = this.props;

        return (
            <Flow onAfterChange={onChange} className={className} style={style} data={data} />
        )
    }
}

export default withPropsAPI(FlowChart);