import * as React from 'react';

export interface IFlowChartProps {
    data?: {
        nodes: Array<any>,
        edges: Array<any>
    };
    className?: string;
    style?: React.CSSProperties;
    onChange?: (items: any) => void
}

declare class FlowChart extends React.Component<IFlowChartProps, any>{
}

export default FlowChart;