import * as React from 'react';

export interface ILineProps {
    title: React.ReactNode;
    color?: string;
    padding?: [number, number, number, number];
    height: number;
    data: Array<{
        x: string;
        y: number;
    }>;
    autoLabel?: boolean;
    style?: React.CSSProperties;
    shape?: string
}

export default class Line extends React.Component<ILineProps>{ }