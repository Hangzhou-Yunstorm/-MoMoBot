import * as React from 'react';
import { Checkbox } from 'antd';
import { CheckboxValueType } from 'antd/lib/checkbox/Group';

const CheckboxGroup = Checkbox.Group;
export interface IRoleCheckGroupProps {
    roles?: Array<{ name: string, id: string }>;
    checkedRoles?: Array<string>;
    value?: any;
    onChange?: (value: any) => void;
}

export default class RoleCheckGroup extends React.PureComponent<IRoleCheckGroupProps, any> {
    constructor(props: IRoleCheckGroupProps) {
        super(props);

        const value = props.value || {};
        this.state = {
            roles: value.roles || [],
            checkedRoles: value.checkedRoles || '',
        };
    }

    static getDerivedStateFromProps(nextProps: IRoleCheckGroupProps) {
        // Should be a controlled component.
        if ('value' in nextProps) {
            return {
                ...(nextProps.value || {}),
            };
        }
        return null;
    }
    onChange(checkedRoles: CheckboxValueType[]) {
        if (!('value' in this.props)) {
            this.setState({ checkedRoles });
        }
        this.triggerChange({ checkedRoles })
    }

    triggerChange = (changedValue: any) => {
        // Should provide an event to pass value to Form.
        const onChange = this.props.onChange;
        if (onChange) {
            const value = Object.assign({}, this.state.checkedRoles, changedValue);
            console.log(this.state.checkedRoles);
            onChange(value);
        }
    }

    render() {
        const { roles, checkedRoles } = this.state;
        const options = roles.map((role: any) => ({ label: role.name, value: role.id }));

        return (
            <CheckboxGroup options={options}
                defaultValue={checkedRoles}
                onChange={this.onChange.bind(this)} />
        )
    }
}