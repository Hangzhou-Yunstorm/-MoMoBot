import * as React from "react";

export interface IStackedPercentageBarProps {
    data?: Array<{
        x: string;
        y: string;
        value: number
    }>;
    visibleX?: boolean;
    height?: number;
    color?: string | [string, string] | [string, string[]];
    tooltip?: boolean | string | [string, (...args: any[]) => { name?: string; value: string }];
    tooltipItemTpl?: string;
}

export default class StackedPercentageBar extends React.Component<IStackedPercentageBarProps> { }