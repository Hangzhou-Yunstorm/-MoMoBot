import * as React from 'react';
import { Command } from 'gg-editor';
import { Tooltip, Icon } from 'antd';
import { upperFirst } from 'lodash';
import * as classNames from 'classnames';
const styles = require('./index.less');

export interface IToolbarButtonProps {
    disabled?: boolean;
    icon?: string;
    text?: string;
    onClick?: () => void;
}

export interface IToolbarCmdButtonProps extends IToolbarButtonProps {
    command: string;
}

class ToolbarCmdButton extends React.PureComponent<IToolbarCmdButtonProps> {
    render() {
        const { command, icon, text } = this.props;

        return (
            <Command name={command}>
                <Tooltip
                    title={text || upperFirst(command)}
                    placement="bottom"
                    overlayClassName={styles.tooltip}
                >
                    <Icon type={icon} />
                </Tooltip>
            </Command>
        );
    }
}

export class ToolbarButton extends React.PureComponent<IToolbarButtonProps> {
    render() {
        const { icon, text, disabled, onClick } = this.props;

        return (
            <div className={classNames(styles.btnContainer, disabled ? styles.disabled : '')}>
                {
                    disabled ?
                        <Icon type={icon} /> :
                        <Tooltip
                            title={text}
                            placement="bottom"
                            overlayClassName={styles.tooltip}
                        >
                            <Icon type={icon} onClick={onClick} />
                        </Tooltip>
                }

            </div>
        );
    }
}

export default ToolbarCmdButton;
