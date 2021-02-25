import React, { Suspense } from 'react';
import numeral from 'numeral';
import ChartCard from './ChartCard';
import Field from './Field';

const getComponent = Component => props => (
    <Suspense fallback="...">
        <Component {...props} />
    </Suspense>
);

const Bar = getComponent(React.lazy(() => import('./Bar')));
const Pie = getComponent(React.lazy(() => import('./Pie')));
const Radar = getComponent(React.lazy(() => import('./Radar')));
const Gauge = getComponent(React.lazy(() => import('./Gauge')));
const MiniArea = getComponent(React.lazy(() => import('./MiniArea')));
const MiniBar = getComponent(React.lazy(() => import('./MiniBar')));
const MiniProgress = getComponent(React.lazy(() => import('./MiniProgress')));
const WaterWave = getComponent(React.lazy(() => import('./WaterWave')));
const TagCloud = getComponent(React.lazy(() => import('./TagCloud')));
const TimelineChart = getComponent(React.lazy(() => import('./TimelineChart')));
const Line = getComponent(React.lazy(() => import('./Line')));
const FlowChart = getComponent(React.lazy(() => import('./FlowChart')));
const StackedPercentageBar = getComponent(React.lazy(() => import('./StackedPercentageBar')));

const yuan = val => `¥ ${numeral(val).format('0,0')}`;

const Charts = {
    yuan,
    Bar,
    Line,
    Pie,
    Gauge,
    Radar,
    MiniBar,
    MiniArea,
    MiniProgress,
    ChartCard,
    Field,
    WaterWave,
    TagCloud,
    TimelineChart,
    StackedPercentageBar
};

export {
    Charts as default,
    Line,
    yuan,
    Bar,
    Pie,
    Gauge,
    Radar,
    MiniBar,
    MiniArea,
    MiniProgress,
    ChartCard,
    Field,
    WaterWave,
    TagCloud,
    TimelineChart,
    FlowChart,
    StackedPercentageBar
};
