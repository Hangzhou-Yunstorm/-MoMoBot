import React from 'react';
import Debounce from 'lodash-decorators/debounce';
import Bind from 'lodash-decorators/bind';
import autoHeight from '../autoHeight';
import { Chart, Axis, Tooltip, Geom, Legend } from 'bizcharts';
import styles from '../index.less';
import DataSet from '@antv/data-set';

@autoHeight()
class StackedPercentageBar extends React.Component {
    state = {
        autoHideXLabels: false,
    };

    componentDidMount() {
        window.addEventListener('resize', this.resize, { passive: true });
    }

    componentWillUnmount() {
        window.removeEventListener('resize', this.resize);
    }

    handleRoot = n => {
        this.root = n;
    };

    handleRef = n => {
        this.node = n;
    };

    @Bind()
    @Debounce(400)
    resize() {
        if (!this.node) {
            return;
        }
        const canvasWidth = this.node.parentNode.clientWidth;
        const { data = [], autoLabel = true } = this.props;
        if (!autoLabel) {
            return;
        }
        const minWidth = data.length * 30;
        const { autoHideXLabels } = this.state;

        if (canvasWidth <= minWidth) {
            if (!autoHideXLabels) {
                this.setState({
                    autoHideXLabels: true,
                });
            }
        } else if (autoHideXLabels) {
            this.setState({
                autoHideXLabels: false,
            });
        }
    }

    render() {
        const {
            visibleX = true,
            height,
            title,
            forceFit = true,
            data,
            color = 'y',
            padding,
            tooltip,
            tooltipItemTpl
        } = this.props;

        const scale = {
            percent: {
                min: 0,

                formatter(val) {
                    return (val * 100).toFixed(2) + "%";
                }
            }
        };

        const ds = new DataSet();
        const dv = ds
            .createView()
            .source(data)
            .transform({
                type: "percent",
                field: "value",
                dimension: "y",
                groupBy: ["x"],
                as: "percent"
            });

        return (
            <div className={styles.chart} ref={this.handleRoot}>
                <div ref={this.handleRef}>
                    {title && <h4 style={{ marginBottom: 20 }}>{title}</h4>}
                    <Chart height={height} data={dv} scale={scale} forceFit>
                        <Legend />
                        <Axis name="x" visible={visibleX} />
                        <Axis name="percent" />
                        <Tooltip itemTpl={tooltipItemTpl} />
                        <Geom
                            type="intervalStack"
                            position="x*percent"
                            color={color}
                            tooltip={tooltip}
                        />
                    </Chart>
                </div>
            </div>
        )
    }
}

export default StackedPercentageBar;