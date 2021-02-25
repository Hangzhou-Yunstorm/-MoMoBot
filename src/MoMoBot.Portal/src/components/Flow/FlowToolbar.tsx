import * as React from 'react';
import { Toolbar, withPropsAPI } from 'gg-editor';
import ToolbarCmdButton, { ToolbarButton } from './ToolbarButton';
import { Divider } from 'antd';
const styles = require('./index.less');

class FlowToolbar extends React.PureComponent<any> {
    save() {
        const data = this.props.propsAPI.save();
        console.log(JSON.stringify(data));
        const { onSave } = this.props;
        onSave && onSave(data);
    }

    render() {
        const { saveEnabled } = this.props;
        return (
            <Toolbar className={styles.toolbar}>
                <ToolbarButton disabled={!saveEnabled} icon="save" text="保存" onClick={this.save.bind(this)} />
                <Divider type="vertical" />
                <ToolbarCmdButton command="zoomIn" icon="zoom-in" text="放大" />
                <ToolbarCmdButton command="zoomOut" icon="zoom-out" text="缩小" />
            </Toolbar>
        )
    }
}


export default withPropsAPI(FlowToolbar);